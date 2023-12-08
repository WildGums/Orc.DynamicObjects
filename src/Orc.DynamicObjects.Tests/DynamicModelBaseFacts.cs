namespace Orc.DynamicObjects.Tests
{
    using System;
    using System.IO;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    using NUnit.Framework;

    public class DynamicModelBaseFacts
    {
        public class DynamicModel : DynamicModelBase
        {
        }

        [TestFixture]
        public class TheGetValueProperties
        {
            [TestCase]
            public void CorrectlyReturnsTheRightValue()
            {
                dynamic model = new DynamicModel();
                var dynamicModel = (DynamicModel)model;

                model.NonExistingGetProperty = "test";

                Assert.That(dynamicModel.IsPropertyRegistered("NonExistingGetProperty"), Is.True);

                Assert.That("test", Is.EqualTo(model.NonExistingGetProperty));
            }
        }

        [TestFixture]
        public class TheSetValueProperties
        {
            [TestCase]
            public void AutomaticallyRegistersNonExistingProperty()
            {
                dynamic model = new DynamicModel();
                var dynamicModel = (DynamicModel)model;

                Assert.That(dynamicModel.IsPropertyRegistered("NonExistingSetProperty"), Is.False);

                model.NonExistingSetProperty = "test";

                Assert.That(dynamicModel.IsPropertyRegistered("NonExistingSetProperty"), Is.True);
            }
        }

        [TestFixture]
        public class TheModelBaseFunctionality
        {
            [TestCase]
            public void SupportsSerialization()
            {
                dynamic model = new DynamicModel();
                model.NonExistingProperty = "a dynamic value";

                var serializer = SerializationFactory.GetXmlSerializer();

                using (var memoryStream = new MemoryStream())
                {
                    var dynamicModel = (DynamicModel)model;
                    serializer.Serialize(dynamicModel, memoryStream, null);

                    memoryStream.Position = 0L;

                    dynamic deserializedModel = serializer.Deserialize(typeof(DynamicModel), memoryStream, null);
                    var deserializedDynamicModel = (DynamicModel) deserializedModel;

                    Assert.That(deserializedDynamicModel.IsPropertyRegistered("NonExistingProperty"), Is.True);
                    Assert.That("a dynamic value", Is.EqualTo(deserializedModel.NonExistingProperty));
                }
            }
        }
    }
}
