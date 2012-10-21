using System;
using NUnit.Framework;

namespace RobustHaven.Text.Npeg.Tests
{
	[TestFixture]
	public class CustomAstNodeCreationTests
	{
		[Test]
		[ExpectedException(typeof (InfiniteLoopDetectedException))]
		public void IntermediateRepresentation_001()
		{
			throw new NotImplementedException();
		}
	}
}