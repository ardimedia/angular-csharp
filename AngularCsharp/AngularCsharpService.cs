using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

namespace AngularCsharp
{
    public class AngularCsharpService
    {
        #region Constructors

        public AngularCsharpService(string template)
        {
            this.Template = template;
            this.Values = new Dictionary<string, string>();
            this.Warnings = new List<string>();
        }

        #endregion

        #region Public Properties

        public string Template { get; private set; }

        public Dictionary<string, string> Values;

        public List<string> Errors { get; private set; }

        public List<string> Warnings { get; private set; }

        public List<string> Informations { get; private set; }

        #endregion

        #region Private Properties

        #endregion

        #region Public Methods

        public string Convert<T>(T objects) where T : class
        {
            ProcessProperty(objects);

            VerifyIfAllHaveBeenProcessed();

            return this.Template;
        }

        public string Convert2<T>(T value) where T : class
        {
            PropertyInfo[] propertyInfos = value.GetType().GetProperties();

            ObjectIDGenerator idGenerator = new ObjectIDGenerator();

            foreach (var propertyInfo in propertyInfos)
            {
                ProcessProperty(propertyInfo.Name, "", propertyInfo.GetValue(value), idGenerator, true);
            }

            this.ReplaceValues();

            return this.Template;
        }

        public static void Dump(object value, string name, TextWriter writer)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            ObjectIDGenerator idGenerator = new ObjectIDGenerator();
            InternalDump(0, name, value, writer, idGenerator, true);
        }

        #endregion

        #region Private Methods

        private void VerifyIfAllHaveBeenProcessed()
        {
            if (this.Template.Contains("{{")
                || this.Template.Contains("}}"))
            {
                this.Warnings.Add("Not all ?tags? have been processed.");
            }
        }

        private void ProcessProperty<T>(T objects) where T : class
        {

            Type type = objects.GetType();
            PropertyInfo[] properties = (from property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                         where property.GetIndexParameters().Length == 0
                                               && property.CanRead
                                         select property).ToArray();

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                Console.WriteLine(propertyInfo.Name + " : " + propertyInfo.PropertyType.FullName);
                object objectValue = propertyInfo.GetValue(objects);

                if (propertyInfo.PropertyType.FullName == "System.String")
                {
                    this.Template = this.Template.Replace("{{" + propertyInfo.Name + "}}", objectValue.ToString());
                }
                else
                {
                    //Type type = propertyInfo.PropertyType;
                    //foreach (var propertyInfo2 in type.GetProperties())
                    //{
                    //    var objectValue2 = propertyInfo2.GetValue(objects);
                    //    this.ProcessProperty(new { objectValue2 });
                    //}
                }
            }
        }

        private void ProcessProperty(string name, string parentName, object value, ObjectIDGenerator idGenerator, bool recursiveDump)
        {
            if (value == null)
            {
                return;
            }

            string newParentName = parentName + name + ".";
            //if (string.IsNullOrWhiteSpace(parentName))
            //{
            //    newParentName = name + ".";
            //}
            //else {
            //    newParentName = parentName + name + ".";
            //}

            Type type = value.GetType();

            // figure out if this is an object that has already been dumped, or is currently being dumped
            string keyRef = string.Empty;
            string keyPrefix = string.Empty;
            if (!type.IsValueType)
            {
                bool firstTime;
                long key = idGenerator.GetId(value, out firstTime);
                if (!firstTime)
                {
                    keyRef = string.Format(CultureInfo.InvariantCulture, " (see #{0})", key);
                }
                else {
                    keyPrefix = string.Format(CultureInfo.InvariantCulture, "#{0}: ", key);
                }
            }

            // work out how a simple dump of the value should be done
            bool isString = value is string;
            string typeName = value.GetType().FullName;
            string formattedValue = value.ToString();

            Exception exception = value as Exception;
            if (exception != null)
            {
                formattedValue = exception.GetType().Name + ": " + exception.Message;
            }

            if (formattedValue == typeName)
            {
                formattedValue = string.Empty;
            }
            else
            {
                // escape tabs and line feeds
                formattedValue = formattedValue.Replace("\t", "\\t").Replace("\n", "\\n").Replace("\r", "\\r");

                // chop at 80 characters
                int length = formattedValue.Length;
                if (length > 80)
                {
                    formattedValue = formattedValue.Substring(0, 80);
                }

                if (isString)
                {
                    if (string.IsNullOrWhiteSpace(parentName))
                    {
                        this.Values.Add(name, value.ToString());
                    }
                    else {
                        this.Values.Add(parentName + name, value.ToString());
                    }
                    formattedValue = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", formattedValue);
                }
                else {
                    if (string.IsNullOrWhiteSpace(parentName))
                    {
                        this.Values.Add(name, value.ToString());
                    }
                    else {
                        this.Values.Add(parentName + name, value.ToString());
                    }
                }

                if (length > 80)
                {
                    formattedValue += " (+" + (length - 80) + " chars)";
                }

                formattedValue = " = " + formattedValue;
            }

            // Avoid dumping objects we've already dumped, or is already in the process of dumping
            if (keyRef.Length > 0)
            {
                return;
            }

            // don't dump strings, we already got at around 80 characters of those dumped
            if (isString)
            {
                return;
            }

            // don't dump value-types in the System namespace
            if (type.IsValueType && type.FullName == "System." + type.Name)
            {
                return;
            }

            // Avoid certain types that will result in endless recursion
            if (type.FullName == "System.Reflection." + type.Name)
            {
                return;
            }

            if (value is System.Security.Principal.SecurityIdentifier)
            {
                return;
            }

            if (!recursiveDump)
            {
                return;
            }

            PropertyInfo[] properties = (from property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public) // | BindingFlags.NonPublic
                                         where property.GetIndexParameters().Length == 0
                                               && property.CanRead
                                         select property).ToArray();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public).ToArray(); // | BindingFlags.NonPublic

            if (properties.Length == 0 && fields.Length == 0)
            {
                return;
            }

            if (properties.Length > 0)
            {
                foreach (PropertyInfo pi in properties)
                {
                    try
                    {
                        object propertyValue = pi.GetValue(value, null);
                        ProcessProperty(pi.Name, newParentName, propertyValue, idGenerator, true);
                    }
                    catch (TargetInvocationException ex)
                    {
                        ProcessProperty(pi.Name, newParentName, ex, idGenerator, false);
                    }
                    catch (ArgumentException ex)
                    {
                        ProcessProperty(pi.Name, newParentName, ex, idGenerator, false);
                    }
                    catch (RemotingException ex)
                    {
                        ProcessProperty(pi.Name, newParentName, ex, idGenerator, false);
                    }
                }
            }
            if (fields.Length > 0)
            {
                foreach (FieldInfo field in fields)
                {
                    try
                    {
                        object fieldValue = field.GetValue(value);
                        ProcessProperty(field.Name, newParentName, fieldValue, idGenerator, true);
                    }
                    catch (TargetInvocationException ex)
                    {
                        ProcessProperty(field.Name, newParentName, ex, idGenerator, false);
                    }
                }
            }
        }

        private static void InternalDump(int indentationLevel, string name, object value, TextWriter writer, ObjectIDGenerator idGenerator, bool recursiveDump)
        {
            string indentation = new string(' ', indentationLevel * 3);

            if (value == null)
            {
                writer.WriteLine("{0}{1} = <null>", indentation, name);
                return;
            }

            Type type = value.GetType();

            // figure out if this is an object that has already been dumped, or is currently being dumped
            string keyRef = string.Empty;
            string keyPrefix = string.Empty;
            if (!type.IsValueType)
            {
                bool firstTime;
                long key = idGenerator.GetId(value, out firstTime);
                if (!firstTime)
                {
                    keyRef = string.Format(CultureInfo.InvariantCulture, " (see #{0})", key);
                }
                else {
                    keyPrefix = string.Format(CultureInfo.InvariantCulture, "#{0}: ", key);
                }
            }

            // work out how a simple dump of the value should be done
            bool isString = value is string;
            string typeName = value.GetType().FullName;
            string formattedValue = value.ToString();

            Exception exception = value as Exception;
            if (exception != null)
            {
                formattedValue = exception.GetType().Name + ": " + exception.Message;
            }

            if (formattedValue == typeName)
            {
                formattedValue = string.Empty;
            }
            else
            {
                // escape tabs and line feeds
                formattedValue = formattedValue.Replace("\t", "\\t").Replace("\n", "\\n").Replace("\r", "\\r");

                // chop at 80 characters
                int length = formattedValue.Length;
                if (length > 80)
                {
                    formattedValue = formattedValue.Substring(0, 80);
                }

                if (isString)
                {
                    formattedValue = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", formattedValue);
                }

                if (length > 80)
                {
                    formattedValue += " (+" + (length - 80) + " chars)";
                }

                formattedValue = " = " + formattedValue;
            }

            writer.WriteLine("{0}{1}{2}{3} [{4}]{5}", indentation, keyPrefix, name, formattedValue, value.GetType(), keyRef);

            // Avoid dumping objects we've already dumped, or is already in the process of dumping
            if (keyRef.Length > 0)
            {
                return;
            }

            // don't dump strings, we already got at around 80 characters of those dumped
            if (isString)
            {
                return;
            }

            // don't dump value-types in the System namespace
            if (type.IsValueType && type.FullName == "System." + type.Name)
            {
                return;
            }

            // Avoid certain types that will result in endless recursion
            if (type.FullName == "System.Reflection." + type.Name)
            {
                return;
            }

            if (value is System.Security.Principal.SecurityIdentifier)
            {
                return;
            }

            if (!recursiveDump)
            {
                return;
            }

            PropertyInfo[] properties =
                (from property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                 where property.GetIndexParameters().Length == 0
                       && property.CanRead
                 select property).ToArray();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToArray();

            if (properties.Length == 0 && fields.Length == 0)
            {
                return;
            }

            writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}{{", indentation));
            if (properties.Length > 0)
            {
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}   properties {{", indentation));
                foreach (PropertyInfo pi in properties)
                {
                    try
                    {
                        object propertyValue = pi.GetValue(value, null);
                        InternalDump(indentationLevel + 2, pi.Name, propertyValue, writer, idGenerator, true);
                    }
                    catch (TargetInvocationException ex)
                    {
                        InternalDump(indentationLevel + 2, pi.Name, ex, writer, idGenerator, false);
                    }
                    catch (ArgumentException ex)
                    {
                        InternalDump(indentationLevel + 2, pi.Name, ex, writer, idGenerator, false);
                    }
                    catch (RemotingException ex)
                    {
                        InternalDump(indentationLevel + 2, pi.Name, ex, writer, idGenerator, false);
                    }
                }
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}   }}", indentation));
            }
            if (fields.Length > 0)
            {
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}   fields {{", indentation));
                foreach (FieldInfo field in fields)
                {
                    try
                    {
                        object fieldValue = field.GetValue(value);
                        InternalDump(indentationLevel + 2, field.Name, fieldValue, writer, idGenerator, true);
                    }
                    catch (TargetInvocationException ex)
                    {
                        InternalDump(indentationLevel + 2, field.Name, ex, writer, idGenerator, false);
                    }
                }
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}   }}", indentation));
            }
            writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}}}", indentation));
        }

        private void ReplaceValues()
        {
            foreach (var value in this.Values)
            {
                this.Template = this.Template.Replace("{{" + value.Key + "}}", value.Value);
            }
        }

        #endregion
    }

    internal static class StringEx
    {
        /// <summary>
        /// Compares the <paramref name="value"/> against <c>null</c> and checks if the
        /// string contains only whitespace.
        /// </summary>
        /// <param name="value">
        /// The string value to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if the string <paramref name="value"/> is <c>null</c>, <see cref="string.Empty"/>,
        /// or contains only whitespace; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrWhiteSpace(string value)
        {
            return value == null || value.Trim().Length == 0;
        }
    }
}
