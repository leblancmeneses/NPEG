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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NPEG;
using NPEG.Terminals;
using NPEG.NonTerminals;

namespace NPEG.Tests
{
    [TestClass]
    public class PredicateTests
    {
        public PredicateTests()
        {
        }

        public TestContext TestContext
        {
            get;
            set;
        }

        [TestMethod]
        public void NonTerminal_Predicate_And()
        {
            // predicates should not adjust the 
            // iterator once the expression is evaluated.
            AExpression Digit = new CharacterClass() { ClassExpression = "[0-9]" };

            // regex expression: \d+
            var input = new StringInputIterator("01234567890123456789");
            AExpression andPredicate = new OneOrMore(Digit).And();
            var visitor = new NpegParserVisitor(input);
            andPredicate.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            Assert.IsTrue(input.Index == 0);
        }


        [TestMethod]
        public void NonTerminal_Predicate_Or()
        {
            // predicates should not adjust the 
            // iterator once the expression is evaluated.
            AExpression Digit = new CharacterClass() { ClassExpression = "[0-9]" };

            // equivalent to: regex '^' '$'
            // regex expression: ^\d+$
            var input = new StringInputIterator("0123456abcdefg");
            AExpression notPredicate = new OneOrMore(Digit).And().Sequence(new NotPredicate(new AnyCharacter()));
            var visitor = new NpegParserVisitor(input);
            notPredicate.Accept(visitor);
            Assert.IsFalse(visitor.IsMatch);    // should fail
            Assert.IsTrue(input.Index == 0);
        }
    }
}
