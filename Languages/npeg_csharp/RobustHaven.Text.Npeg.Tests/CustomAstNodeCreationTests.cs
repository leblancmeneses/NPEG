using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RobustHaven.Text.Npeg;
using NUnit.Framework;

namespace RobustHaven.Text.Npeg.Tests
{
    [TestFixture]
    public class CustomAstNodeCreationTests
    {
        [Test]
        [ExpectedException(typeof(InfiniteLoopDetectedException))]
        public void IntermediateRepresentation_001()
        {
            throw new NotImplementedException();
        }
    }
}
