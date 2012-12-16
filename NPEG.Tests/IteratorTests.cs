#region License

/* **********************************************************************************
 * Copyright (c) Leblanc Meneses
 * This source code is subject to terms and conditions of the MIT License
 * for NPEG. A copy of the license can be found in the License.txt file
 * at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/

#endregion

using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPEG.ApplicationExceptions;

namespace NPEG.Tests
{
	[TestClass]
	public class IteratorTests
	{
		public TestContext TestContext { get; set; }


		[TestMethod]
		public void Iterator_Initialization()
		{
			var input = "01234567890123456789";
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);

			// tests that iterator begins at zero based index
			Assert.IsTrue(iterator.Index == 0);
			Assert.IsTrue(iterator.Length == 20);
			Assert.IsTrue(iterator.Current() == '0');
			Assert.IsTrue(iterator.Next() == '1');
			Assert.IsTrue(iterator.Previous() == '0');
			Assert.IsTrue(bytes.SequenceEqual(iterator.Text(0, 19)), "Text unable to return complete input.");
		}

		[TestMethod]
		public void Iterator_GetText_Limit()
		{
			var bytes = Encoding.UTF8.GetBytes("01234567890123456789");
			var iterator = new ByteInputIterator(bytes);
			Assert.IsTrue(Encoding.ASCII.GetBytes("0").SequenceEqual(iterator.Text(0, 0)), "Text unable to return first character.");
			Assert.IsTrue(Encoding.ASCII.GetBytes("9").SequenceEqual(iterator.Text(19, 19)), "Text unable to return last character.");
			Assert.IsTrue(Encoding.ASCII.GetBytes("01").SequenceEqual(iterator.Text(0, 1)),
			              "Text unable to return specified start and end characters inclusive.");

			try
			{
				iterator.Text(19, 0);
				Assert.Fail("Start must be <= End");
			}
			catch (IteratorUsageException e)
			{
			}
		}

		[TestMethod]
		public void Iterator_OutofRange()
		{
			var bytes = Encoding.UTF8.GetBytes("");
			var iterator = new ByteInputIterator(bytes);
			Assert.IsTrue(iterator.Index == 0);
			Assert.IsTrue(iterator.Length == 0);
			Assert.IsTrue(iterator.Current() == -1);
			Assert.IsTrue(iterator.Index == 0);
			Assert.IsTrue(iterator.Next() == -1);
			Assert.IsTrue(iterator.Index == 0);
			Assert.IsTrue(iterator.Previous() == -1);
			Assert.IsTrue(iterator.Index == 0);
		}

		[TestMethod]
		public void Iterator_Index()
		{
			var bytes = Encoding.UTF8.GetBytes("01234567890123456789");
			var iterator = new ByteInputIterator(bytes);
			Assert.IsTrue(iterator.Index == 0);
			Assert.IsTrue(iterator.Length == bytes.Length);

			for (int i = 0; i < bytes.Length; i++)
			{
				Assert.IsTrue(iterator.Index == i);
				Assert.IsTrue(iterator.Current() == bytes[i]);
				if (i < bytes.Length - 1)
				{
					Assert.IsTrue(iterator.Next() == bytes[i + 1]);
				}
			}

			for (int i = bytes.Length - 1; i >= 0; i--)
			{
				Assert.IsTrue(iterator.Index == i);
				Assert.IsTrue(iterator.Current() == bytes[i]);
				if (i > 0)
				{
					Assert.IsTrue(iterator.Previous() == bytes[i - 1]);
				}
			}
		}
	}
}