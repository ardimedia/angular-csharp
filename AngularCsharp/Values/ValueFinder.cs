using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace AngularCsharp.Values
{
    /// <summary>
    /// Finds an value in the variables dictionary based on Angular expression syntax,
    /// see https://angular.io/docs/ts/latest/guide/template-syntax.html#!#template-expressions
    /// </summary>
    public class ValueFinder
    {
        #region Public Methods

        public string GetString(string key, Dictionary<string, object> lookup)
        {
            // TODO: Verify key (must not be empty)
            // TODO: Verify lookup (must not be empty)

            var result = GetObject(key, lookup);

            return result.ToString();
        }

        public IEnumerable GetList(string key, Dictionary<string, object> lookup)
        {
            var list = GetObject(key, lookup);
            if (!(list is IEnumerable))
            {
                return (IEnumerable) list;
            }

            // TODO: Add warning about wrong variable

            // Return object array with found item
            return new Object[1] { list };
        }

        #endregion

        #region Private Methods

        private object GetObject(string key, Dictionary<string, object> lookup)
        {
            string[] keySplitted = key.Split('.');

            foreach (var item in lookup)
            {
                if (item.Key == keySplitted[0])
                {
                    return item.Value.GetType().GetProperty(keySplitted[1]).GetValue(item.Value).ToString();

                    //PropertyInfo[] propertyInfos = item.GetType().GetProperties();

                    //foreach (var propertyInfo in propertyInfos)
                    //{
                    //    ProcessProperty(propertyInfo.Name, "", propertyInfo.GetValue(item));
                    //}
                }
            }

            // TODO: Return exception, since key could not by found
            return null;
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
                //this.Values.Add(name, value.ToString());
            }
            else
            {
                //this.Values.Add(parentName + name, value.ToString());
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

            //PropertyInfo[] properties = (from property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public) // | BindingFlags.NonPublic
            //                             where property.GetIndexParameters().Length == 0
            //                                   && property.CanRead
            //                             select property).ToArray();

            //FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public).ToArray(); // | BindingFlags.NonPublic

            //if (properties.Length == 0 && fields.Length == 0)
            //{
            //    return;
            //}

            //if (properties.Length > 0)
            //{
            //    foreach (PropertyInfo pi in properties)
            //    {
            //        try
            //        {
            //            object propertyValue = pi.GetValue(value, null);
            //            ProcessProperty(pi.Name, newParentName, propertyValue);
            //        }
            //        catch (TargetInvocationException ex)
            //        {
            //            ProcessProperty(pi.Name, newParentName, ex);
            //        }
            //        catch (ArgumentException ex)
            //        {
            //            ProcessProperty(pi.Name, newParentName, ex);
            //        }
            //        catch (RemotingException ex)
            //        {
            //            ProcessProperty(pi.Name, newParentName, ex);
            //        }
            //    }
            //}

            //if (fields.Length > 0)
            //{
            //    foreach (FieldInfo field in fields)
            //    {
            //        try
            //        {
            //            object fieldValue = field.GetValue(value);
            //            ProcessProperty(field.Name, newParentName, fieldValue);
            //        }
            //        catch (TargetInvocationException ex)
            //        {
            //            ProcessProperty(field.Name, newParentName, ex);
            //        }
            //    }
            //}
        }

        #endregion
    }
}