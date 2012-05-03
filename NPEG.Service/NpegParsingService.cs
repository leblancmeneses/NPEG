using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace NPEG.Service
{
    public class NpegParsingService : IParsingService
    {
        #region IParsingService Members

        public NpegData Parse(String rules, Byte[] input)
        {
            NpegData result = new NpegData() { Ast = null, ParseTree = null };

            var parseTree = GrammarInterpreter.PEGrammar.Load(rules);
            var visitor = new NpegParserVisitor(
                new StringInputIterator(input)
            );
            parseTree.Accept(visitor);

            if (!visitor.IsMatch)
            {
                result.Warnings = visitor.Warnings;
            }

            result.Warnings = visitor.Warnings;
            result.Ast = visitor.AST;
            result.ParseTree = parseTree;

            return result;
        }

        #endregion
    }
}
