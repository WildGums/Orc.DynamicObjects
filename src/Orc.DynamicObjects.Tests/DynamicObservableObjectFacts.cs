namespace Orc.DynamicObjects.Tests
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.IO;
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using NUnit.Framework;
    using System.ComponentModel;

    public class DynamicObservableObjectFacts
    {
        public class CustomObject : DynamicObservableObject
        {
        }

        [TestFixture]
        public class TheGetValueAndSetValueProperties
        {
            [TestCase]
            public void CorrectlyReturnsTheRightValue_WhenSetViaDynamicProperty()
            {
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                // Settings value via dynamic property, getting via dynamic property and via GetValue<T> method.
                DateTime dt = DateTime.ParseExact("2016-01-01 01:01:01", "yyyy-MM-dd HH:mm:ss", null);
                dynamicObservableObject.Property1 = "test";
                dynamicObservableObject.Property2 = 100;
                dynamicObservableObject.Property3 = 3.14F;
                dynamicObservableObject.Property4 = 1.2M;
                dynamicObservableObject.Property5 = dt;

                Assert.That("test", Is.EqualTo(dynamicObservableObject.Property1));
                Assert.That(100, Is.EqualTo(dynamicObservableObject.Property2));
                Assert.That(3.14F, Is.EqualTo(dynamicObservableObject.Property3));
                Assert.That(1.2M, Is.EqualTo(dynamicObservableObject.Property4));
                Assert.That(dt, Is.EqualTo(dynamicObservableObject.Property5));

                Assert.That(observableObject.GetValue<string>("Property1"), Is.EqualTo("test"));
                Assert.That(observableObject.GetValue<int>("Property2"), Is.EqualTo(100));
                Assert.That(observableObject.GetValue<float>("Property3"), Is.EqualTo(3.14F));
                Assert.That(observableObject.GetValue<decimal>("Property4"), Is.EqualTo(1.2M));
                Assert.That(observableObject.GetValue<DateTime>("Property5"), Is.EqualTo(dt));
            }

            [TestCase]
            public void CorrectlyReturnsTheRightValue_WhenSetViaSetValueMethod()
            {
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                // Setting value via SetValue method, getting via dynamic property and via GetValue<T> method.
                DateTime dt = DateTime.ParseExact("2016-01-01 01:01:01", "yyyy-MM-dd HH:mm:ss", null);
                observableObject.SetValue("Property1", "test");
                observableObject.SetValue("Property2", 100);
                observableObject.SetValue("Property3", 3.14F);
                observableObject.SetValue("Property4", 1.2M);
                observableObject.SetValue("Property5", dt);

                Assert.That("test", Is.EqualTo(dynamicObservableObject.Property1));
                Assert.That(100, Is.EqualTo(dynamicObservableObject.Property2));
                Assert.That(3.14F, Is.EqualTo(dynamicObservableObject.Property3));
                Assert.That(1.2M, Is.EqualTo(dynamicObservableObject.Property4));
                Assert.That(dt, Is.EqualTo(dynamicObservableObject.Property5));

                Assert.That(observableObject.GetValue<string>("Property1"), Is.EqualTo("test"));
                Assert.That(observableObject.GetValue<int>("Property2"), Is.EqualTo(100));
                Assert.That(observableObject.GetValue<float>("Property3"), Is.EqualTo(3.14F));
                Assert.That(observableObject.GetValue<decimal>("Property4"), Is.EqualTo(1.2M));
                Assert.That(observableObject.GetValue<DateTime>("Property5"), Is.EqualTo(dt));
            }

            [TestCase]
            public void CorrectlyReturnsTheDefaultValue_WhenNotSet()
            {
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                Assert.That(observableObject.GetValue<string>("Property1"), Is.EqualTo(null));
                Assert.That(observableObject.GetValue<int>("Property2"), Is.EqualTo(0));
                Assert.That(observableObject.GetValue<float>("Property3"), Is.EqualTo(0F));
                Assert.That(observableObject.GetValue<decimal>("Property4"), Is.EqualTo(0M));
                Assert.That(observableObject.GetValue<DateTime>("Property5"), Is.EqualTo(DateTime.MinValue));
                //
                Assert.That(observableObject.GetValue<string>("Property1"), Is.EqualTo(null));
                Assert.That(observableObject.GetValue<int?>("Property2"), Is.EqualTo(null));
                Assert.That(observableObject.GetValue<float?>("Property3"), Is.EqualTo(null));
                Assert.That(observableObject.GetValue<decimal?>("Property4"), Is.EqualTo(null));
                Assert.That(observableObject.GetValue<DateTime?>("Property5"), Is.EqualTo(null));
            }

            [TestCase]
            public void RaisesPropertyChangedEvents_WhenSetViaDynamicProperty()
            {
                var counter = 0;
                var propertyName = default(string);
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                // Setting value via dynamic property.
                dynamicObservableObject.Property1 = "oldtest";
                observableObject.PropertyChanged += (sender, e) =>
                {
                    var args = e as PropertyChangedEventArgs;
                    if (args is not null)
                    {
                        counter++;
                        propertyName = args.PropertyName;
                    }
                };
                dynamicObservableObject.Property1 = "newtest";

                Assert.That(counter, Is.EqualTo(1));
                Assert.That(propertyName, Is.EqualTo("Property1"));
            }

            [TestCase]
            public void RaisesPropertyChangedEvents_WhenSetViaSetValueMethod()
            {
                var counter = 0;
                var propertyName = default(string);
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                // Setting value via dynamic property.
                observableObject.SetValue("Property1", "oldtest");
                observableObject.PropertyChanged += (sender, e) =>
                {
                    var args = e as PropertyChangedEventArgs;
                    if (args is not null)
                    {
                        counter++;
                        propertyName = args.PropertyName;
                    }
                };
                observableObject.SetValue("Property1", "newtest");

                Assert.That(counter, Is.EqualTo(1));
                Assert.That(propertyName, Is.EqualTo("Property1"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionWhenPropertyNameIsNullOrWhitespace_WhenSetViaSetValueMethod()
            {
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                Assert.Throws<ArgumentException>(() => observableObject.SetValue(null, "test"));
                Assert.Throws<ArgumentException>(() => observableObject.SetValue(string.Empty, "test"));
                Assert.Throws<ArgumentException>(() => observableObject.SetValue(" ", "test"));
            }
        }

        [TestFixture]
        public class TheGetDynamicMemberNamesMethod
        {
            [TestCase]
            public void CorrectlyReturnsDynamicMemberNames_WhenSetViaDynamicProperty()
            {
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                // Settings value via dynamic property.
                DateTime dt = DateTime.ParseExact("2016-01-01 01:01:01", "yyyy-MM-dd HH:mm:ss", null);
                dynamicObservableObject.Property1 = "test";
                dynamicObservableObject.Property2 = 100;
                dynamicObservableObject.Property3 = 3.14F;
                dynamicObservableObject.Property4 = 1.2M;
                dynamicObservableObject.Property5 = dt;

                // Get dynamic member names and sort (we get keys from dictionary where order is unspecified, so it's better to sort by names).
                var memberNames = observableObject.GetMetaObject(Expression.Constant(observableObject)).GetDynamicMemberNames().ToList();
                memberNames.Sort();
                Assert.That(memberNames.Count, Is.EqualTo(5));
                Assert.That(memberNames[0], Is.EqualTo("Property1"));
                Assert.That(memberNames[1], Is.EqualTo("Property2"));
                Assert.That(memberNames[2], Is.EqualTo("Property3"));
                Assert.That(memberNames[3], Is.EqualTo("Property4"));
                Assert.That(memberNames[4], Is.EqualTo("Property5"));
            }

            [TestCase]
            public void CorrectlyReturnsDynamicMemberNames_WhenSetViaSetValueMethod()
            {
                var observableObject = new CustomObject();
                dynamic dynamicObservableObject = observableObject;

                // Setting value via SetValue method.
                DateTime dt = DateTime.ParseExact("2016-01-01 01:01:01", "yyyy-MM-dd HH:mm:ss", null);
                observableObject.SetValue("Property1", "test");
                observableObject.SetValue("Property2", 100);
                observableObject.SetValue("Property3", 3.14F);
                observableObject.SetValue("Property4", 1.2M);
                observableObject.SetValue("Property5", dt);

                // Get dynamic member names and sort (we get keys from dictionary where order is unspecified, so it's better to sort by names).
                var memberNames = observableObject.GetMetaObject(Expression.Constant(observableObject)).GetDynamicMemberNames().ToList();
                memberNames.Sort();
                Assert.That(memberNames.Count, Is.EqualTo(5));
                Assert.That(memberNames[0], Is.EqualTo("Property1"));
                Assert.That(memberNames[1], Is.EqualTo("Property2"));
                Assert.That(memberNames[2], Is.EqualTo("Property3"));
                Assert.That(memberNames[3], Is.EqualTo("Property4"));
                Assert.That(memberNames[4], Is.EqualTo("Property5"));
            }
        }
    }
}
