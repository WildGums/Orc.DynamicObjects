namespace Orc.DynamicObjects.Tests
{
    using System.IO;

    using NUnit.Framework;

    public class DynamicModelBaseFacts
    {
        public class DynamicModel : DynamicModelBase
        {
        }

        [TestFixture]
        public class The_GetValue_Properties
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
        public class The_SetValue_Properties
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
    }
}
