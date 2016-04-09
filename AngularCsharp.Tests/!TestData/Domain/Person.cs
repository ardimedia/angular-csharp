using System;
using System.Collections.Generic;

namespace AngularCSharp.Tests._TestData.Domain
{
    public class Person
    {
        #region Public Properties

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        #endregion

        #region Public Methods

        public static Person GetRandom()
        {
            Person person = new Person();
            person.FirstName = $"First Name {new Random(DateTime.Now.Millisecond).Next(0, 999)}";
            person.LastName = $"Last Name {new Random(DateTime.Now.Millisecond).Next(0, 999)}";
            person.Email = $"Email.{new Random(DateTime.Now.Millisecond).Next(0, 999)}@domain.test";
            return person;
        }

        public static List<Person> GetRandoms(int count)
        {
            List<Person> persons = new List<Person>();

            for (int i = 0; i < count; i++)
            {
                persons.Add(Person.GetRandom());
            }

            return persons;
        }

        #endregion
    }
}