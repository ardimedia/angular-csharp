using System;
using System.Collections;
using System.Collections.Generic;
using AngularCSharp.Exceptions;
using System.Reflection;

namespace AngularCSharp.Helpers
{
    /// <summary>
    /// Finds an value in the variables dictionary based on Angular expression syntax,
    /// see https://angular.io/docs/ts/latest/guide/template-syntax.html#!#template-expressions
    /// </summary>
    public class ValueFinder
    {
        #region Public Methods

        /// <summary>
        /// Returns string representation of key
        /// </summary>
        /// <param name="key">Angular2 expression</param>
        /// <param name="lookup">Available variables</param>
        /// <returns></returns>
        public virtual string GetString(string key, IDictionary<string, object> lookup)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key is null or empty");
            }

            if (lookup == null)
            {
                throw new ArgumentException("Lookup dictionary is null");
            }

            object result = GetObject(key, lookup);

            return result.ToString();
        }

        /// <summary>
        /// Returns enumerable list of key
        /// </summary>
        /// <param name="key">Angular2 expression</param>
        /// <param name="lookup">Available variables</param>
        /// <returns></returns>
        public virtual IEnumerable GetList(string key, IDictionary<string, object> lookup)
        {
            var list = GetObject(key, lookup);
            if (list is IEnumerable)
            {
                return (IEnumerable) list;
            }

            throw new InvalidCastException("List object doesn't implement IEnumerable");
        }

        /// <summary>
        /// Returns object of key
        /// </summary>
        /// <param name="key">Angular2 expression</param>
        /// <param name="lookup">Available variables</param>
        /// <returns></returns>
        public virtual object GetObject(string key, IDictionary<string, object> lookup)
        {
            object model = lookup;

            foreach (string keyPart in key.Split('.'))
            {
                if (model == null)
                {
                    throw new ValueNotFoundException();
                }

                model = FindProperty(model, keyPart);
            }

            return model;
        }

        /// <summary>
        /// Returns all properties of specified object
        /// </summary>
        /// <param name="model">Any object</param>
        /// <returns></returns>
        public Dictionary<string, object> GetAllProperties(object model)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                dictionary.Add(propertyInfo.Name, propertyInfo.GetValue(model));
            }

            return dictionary;
        }

        #endregion

        #region Private Methods

        private object FindProperty(object container, string key)
        {
            if (container == null)
            {
                throw new ValueNotFoundException();
            }

            if (container is IDictionary)
            {
                return GetItemFromDictionary((IDictionary)container, key);
            }

            return GetPropertyValueByReflection(container, key);
        }

        private object GetItemFromDictionary(IDictionary dictionary, string key)
        {
            if (!dictionary.Contains(key))
            {
                throw new ValueNotFoundException();
            }

            return dictionary[key];
        }

        private object GetPropertyValueByReflection(object model, string key)
        {
            var property = model.GetType().GetProperty(key);
            if (property == null)
            {
                throw new ValueNotFoundException();
            }

            return property.GetValue(model);
        }

        #endregion
    }
}