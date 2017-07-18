using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using HtmlAgilityPack;

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

        #region Properties

        private string result;

        public string Template { get; private set; }

        public Dictionary<string, string> Values;

        public List<string> Errors { get; private set; }

        public List<string> Warnings { get; private set; }

        public List<string> Informations { get; private set; }

        #endregion

        #region Public Methods

        public string Convert<T>(T value) where T : class
        {
            this.result = this.Template;

            PropertyInfo[] propertyInfos = value.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                ProcessProperty(propertyInfo.Name, "", propertyInfo.GetValue(value));
            }

            this.result = AngularCsharpOperation.ReplaceValues(this.result, this.Values);
            this.result = this.ProcessNgFor();

            this.Warnings.AddRange(AngularCsharpOperation.VerifyIfAllHaveBeenProcessed(this.result));

            return this.result;
        }

        #endregion

        #region Private Methods

        int arrayCount = 0;

        private void ProcessProperty(string name, string parentName, object value)
        {
            if (value == null)
            {
                return;
            }

            string newParentName = parentName + name + ".";

            Type type = value.GetType();

            // work out how a simple dump of the value should be done
            bool isString = type == typeof(string) ? true : false;
            bool isArray = type.IsArray;

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

            if (isArray)
            {
                Array array = value as Array;
                for (int i = 0; i < array.Length; i++)
                {
                    this.arrayCount = this.arrayCount + 1;
                    string newParentNameArray = parentName + name + "." + this.arrayCount + ".";
                    ProcessProperty(name, newParentNameArray, array.GetValue(i));
                }
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

            // Get all properties for an object
            PropertyInfo[] properties = (from property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public) // | BindingFlags.NonPublic
                                         where property.GetIndexParameters().Length == 0
                                               && property.CanRead
                                         select property).ToArray();

            // Get all fields for an object
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public).ToArray(); // | BindingFlags.NonPublic

            if (properties.Length == 0 && fields.Length == 0)
            {
                return;
            }

            if (properties.Length > 0)
            {
                foreach (PropertyInfo pi in properties)
                {
                    if (pi.Name == "SyncRoot")
                    {
                        continue; // AnonymousType ?!?
                    }

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

        private string ProcessNgFor()
        {
            HtmlAgilityPack.HtmlNodeCollection htmlNodes = HtmlOperation.GetNodesNgFor(this.result);

            if (htmlNodes == null)
            {
                // Nothing found to process
                return this.result;
            }

            foreach (HtmlNode htmlNode in htmlNodes)
            {
                string htmlNgFor = htmlNode.OuterHtml;
                string collectionName = NgForOperation.GetParameterCollectionName(htmlNgFor);
                string itemName = NgForOperation.GetParameterItemName(htmlNgFor);

                Dictionary<string, string> valueRows = new Dictionary<string, string>();
                bool isNewDataRowFound = false;

                this.arrayCount = 0;
                foreach (KeyValuePair<string, string> item in this.Values)
                {
                    if (!item.Key.StartsWith(collectionName, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    if (item.Key == collectionName + "." + (this.arrayCount + 1) + "." + collectionName)
                    {
                        if (valueRows.Count != 0)
                        {
                            htmlNgFor = AngularCsharpOperation.ReplaceValues(htmlNgFor, valueRows);

                            HtmlNode parentNode = htmlNode.ParentNode;
                            HtmlNode newNode = HtmlNode.CreateNode(htmlNgFor);
                            parentNode.AppendChild(newNode);

                        }
                        isNewDataRowFound = true;
                        valueRows = new Dictionary<string, string>();
                        this.arrayCount = this.arrayCount + 1;
                    }

                    if (isNewDataRowFound || this.arrayCount >= 1)
                    {
                        if (item.Key == collectionName + "." + this.arrayCount + "." + collectionName)
                        {
                            if (!isNewDataRowFound)
                            {
                                this.arrayCount = this.arrayCount + 1;
                            }
                            isNewDataRowFound = false;
                            continue;
                        }

                        string newKey = item.Key.Replace(collectionName + "." + this.arrayCount + ".", "");
                        newKey = newKey.Replace(collectionName, itemName);
                        valueRows.Add(newKey, item.Value);
                    }

                }
            }

            return this.result;
        }

        #endregion
    }
}
