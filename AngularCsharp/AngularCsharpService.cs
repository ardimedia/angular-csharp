using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;

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

        private string result;

        #endregion

        #region Public Methods

        public string Convert<T>(T value) where T : class
        {
            this.result = this.Template;

            PropertyInfo[] propertyInfos = value.GetType().GetProperties();

            foreach (var propertyInfo in propertyInfos)
            {
                ProcessProperty(propertyInfo.Name, "", propertyInfo.GetValue(value));
            }

            this.ReplaceValues();

            VerifyIfAllHaveBeenProcessed();

            return this.result;
        }

        #endregion

        #region Private Methods

        private void VerifyIfAllHaveBeenProcessed()
        {
            if (this.result.Contains("{{")
                || this.result.Contains("}}"))
            {
                this.Warnings.Add("Not all ?tags? have been processed.");
            }
        }

        private void ProcessProperty(string name, string parentName, object value)
        {
            if (value == null)
            {
                return;
            }

            string newParentName = parentName + name + ".";

            Type type = value.GetType();

            // work out how a simple dump of the value should be done
            bool isString = value is string;
            string typeName = value.GetType().FullName;
            Exception exception = value as Exception;

            if (string.IsNullOrWhiteSpace(parentName))
            {
                this.Values.Add(name, value.ToString());
            }
            else
            {
                this.Values.Add(parentName + name, value.ToString());
            }

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
                        ProcessProperty(pi.Name, newParentName, propertyValue);
                    }
                    catch (TargetInvocationException ex)
                    {
                        ProcessProperty(pi.Name, newParentName, ex);
                    }
                    catch (ArgumentException ex)
                    {
                        ProcessProperty(pi.Name, newParentName, ex);
                    }
                    catch (RemotingException ex)
                    {
                        ProcessProperty(pi.Name, newParentName, ex);
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
                        ProcessProperty(field.Name, newParentName, fieldValue);
                    }
                    catch (TargetInvocationException ex)
                    {
                        ProcessProperty(field.Name, newParentName, ex);
                    }
                }
            }
        }

        private void ReplaceValues()
        {
            foreach (var value in this.Values)
            {
                this.result = this.result.Replace("{{" + value.Key + "}}", value.Value);
            }
        }

        #endregion
    }
}