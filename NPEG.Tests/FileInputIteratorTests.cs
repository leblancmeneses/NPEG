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
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPEG.ApplicationExceptions;

namespace NPEG.Tests
{
	[TestClass]
	public class FileInputIteratorTests
	{
		public TestContext TestContext { get; set; }


		[TestMethod]
		public void Iterator_Initialization()
		{
			var value = "01234567890123456789";
			var bytes = Encoding.ASCII.GetBytes(value);
			var memoryStream = new MemoryStream();
			memoryStream.Write(bytes, 0, bytes.Length);
			var input = new FileInputIterator(memoryStream);

			// tests that iterator begins at zero based index
			Assert.IsTrue(input.Index == 0);
			Assert.IsTrue(input.Length == 20);
			Assert.IsTrue(input.Current() == '0');
			Assert.IsTrue(input.Next() == '1');
			Assert.IsTrue(input.Previous() == '0');
			Assert.IsTrue(Encoding.ASCII.GetBytes(value).SequenceEqual(input.Text(0, 19)),
			              "Text unable to return complete input.");
		}

		[TestMethod]
		public void Iterator_GetText_Limit()
		{
			var value = "01234567890123456789";
			var bytes = Encoding.ASCII.GetBytes(value);
			var memoryStream = new MemoryStream();
			memoryStream.Write(bytes, 0, bytes.Length);
			var input = new FileInputIterator(memoryStream);
			Assert.IsTrue(Encoding.ASCII.GetBytes("0").SequenceEqual(input.Text(0, 0)), "Text unable to return first character.");
			Assert.IsTrue(Encoding.ASCII.GetBytes("9").SequenceEqual(input.Text(19, 19)), "Text unable to return last character.");
			Assert.IsTrue(Encoding.ASCII.GetBytes("01").SequenceEqual(input.Text(0, 1)),
			              "Text unable to return specified start and end characters inclusive.");

			try
			{
				input.Text(19, 0);
				Assert.Fail("Start must be <= End");
			}
			catch (IteratorUsageException e)
			{
			}
		}

		[TestMethod]
		public void Iterator_OutofRange()
		{
			var memoryStream = new MemoryStream();
			var input = new FileInputIterator(memoryStream);
			Assert.IsTrue(input.Index == 0);
			Assert.IsTrue(input.Length == 0);
			Assert.IsTrue(input.Current() == -1);
			Assert.IsTrue(input.Index == 0);
			Assert.IsTrue(input.Next() == -1);
			Assert.IsTrue(input.Index == 0);
			Assert.IsTrue(input.Previous() == -1);
			Assert.IsTrue(input.Index == 0);
		}

		[TestMethod]
		public void Iterator_Index()
		{
			var value = "01234567890123456789";
			var bytes = Encoding.ASCII.GetBytes(value);
			var memoryStream = new MemoryStream();
			memoryStream.Write(bytes, 0, bytes.Length);
			var input = new FileInputIterator(memoryStream);
			Assert.IsTrue(input.Index == 0);
			Assert.IsTrue(input.Length == value.Length);

			for (int i = 0; i < value.Length; i++)
			{
				Assert.IsTrue(input.Index == i);
				Assert.IsTrue(input.Current() == value[i]);
				if (i < value.Length - 1)
				{
					Assert.IsTrue(input.Next() == value[i + 1]);
				}
			}

			for (int i = value.Length - 1; i >= 0; i--)
			{
				Assert.IsTrue(input.Index == i);
				Assert.IsTrue(input.Current() == value[i]);
				if (i > 0)
				{
					Assert.IsTrue(input.Previous() == value[i - 1]);
				}
			}
		}
	}
}