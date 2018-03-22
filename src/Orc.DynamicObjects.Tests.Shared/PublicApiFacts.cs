// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicApiFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DynamicObjects.Tests
{
    using ApiApprover;
    using NUnit.Framework;

    [TestFixture]
    public class PublicApiFacts
    {
        [Test]
        public void Orc_DynamicObjects_HasNoBreakingChanges()
        {
            var assembly = typeof(DynamicModelBase).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }
    }
}