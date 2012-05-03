using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NPEG.Algorithms.LanguageWriters
{
    public class CPlusPlusWriter : WriterBase
    {
        
        private String _grammarName;
        private String _folderPath;

        private String currentFunctionPointerLabel = String.Empty;

        private class MethodInfo
        {
            public String methodName = String.Empty;
            public String methodData = String.Empty;
        }
        private Stack<MethodInfo> methods = new Stack<MethodInfo>();
        private Stack<MethodInfo> finalMethods = new Stack<MethodInfo>();






        public CPlusPlusWriter(String grammarName, String folderPath)
        {
            this._grammarName = grammarName;
            this._folderPath = folderPath;
        }


        public override void Write()
        {
            StringBuilder header = new StringBuilder();
            StringBuilder src = new StringBuilder();

            src.AppendFormat("#include \"{0}.h\"\n\n", this._grammarName);
            src.Append("using namespace RobustHaven::Text;\n\n");

            
            header.AppendFormat("#ifndef ROBUSTHAVEN_GENERATEDPARSER_{0}\n", this._grammarName.ToUpper());
            header.AppendFormat("#define ROBUSTHAVEN_GENERATEDPARSER_{0}\n\n", this._grammarName.ToUpper());
            header.Append("#include \"robusthaven/text/InputIterator.h\"\n");
            header.Append("#include \"robusthaven/text/Npeg.h\"\n\n");
            header.Append("using namespace RobustHaven::Text;\n\n");
            header.AppendFormat("class {0} : public Npeg", this._grammarName);
            header.Append("\n{\n");

            header.Append("\tpublic:\n");
            header.AppendLine(String.Format("\t\t{0}(InputIterator* inputstream): Npeg(inputstream){{}}\n", this._grammarName));

            String startMethodName = this._grammarName + @"_impl_0";
            header.Append("\t\tvirtual int isMatch() throw(ParsingFatalTerminalException) { return " + startMethodName + "(); }\n");
            
            header.Append("\n\n");
            header.Append("\tprivate:\n");

            while (finalMethods.Count > 0)
            {
                MethodInfo m = finalMethods.Pop();
                header.AppendLine(String.Format("\t\tint {0}();", m.methodName));
                src.AppendLine(m.methodData);
            }

            header.Append("};\n");
            header.Append("\n#endif\n");


            System.IO.DirectoryInfo info = System.IO.Directory.CreateDirectory(this._folderPath);
            using (System.IO.StreamWriter writer = System.IO.File.CreateText(System.IO.Path.Combine(info.FullName, "main.cpp")))
            {
                StringBuilder main = new StringBuilder();
                main.Append("#include <cstring>\n");

                main.Append("#include \"robusthaven/text/InputIterator.h\"\n");
                main.AppendFormat("#include \"{0}.h\"\n\n", this._grammarName);
                main.Append("using namespace RobustHaven::Text;\n");

                main.Append("\n");
                main.AppendLine("int main(int argc, char *argv[])");
                main.AppendLine("{");
                main.Append("\tconst char* stream = \"123-456-7890\";\n");
                main.Append("\tInputIterator* input = new InputIterator(stream, strlen(stream));\n");
                main.AppendFormat("\t{0}* parser = new {0}(input);", this._grammarName);
                main.Append("\n");
                main.AppendLine("\tint IsMatch = parser->isMatch();\n");
                main.Append("\tdelete parser;\n");
                main.Append("\tdelete input;\n");
                main.Append("\n\n");
                main.AppendLine("\treturn 0;");
                main.AppendLine("}");
                writer.Write(main.ToString());
            }

            using(System.IO.StreamWriter writer = System.IO.File.CreateText( System.IO.Path.Combine( info.FullName, String.Format("{0}.cpp", this._grammarName) )))
            {
                writer.Write(src.ToString());
            }

            using(System.IO.StreamWriter writer = System.IO.File.CreateText( System.IO.Path.Combine( info.FullName, String.Format("{0}.h", this._grammarName) )))
            {
                writer.Write(header.ToString());
            }

            System.Diagnostics.Debug.WriteLine(info.FullName);
        }







        #region non terminals
        public override void VisitEnter(NPEG.NonTerminals.PrioritizedChoice expression)
        {
            this.methods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.methods.Peek().methodData += String.Format("\treturn this->prioritizedChoice(");
        }
        public override void VisitExecute(NPEG.NonTerminals.PrioritizedChoice expression)
        {
            this.methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1}, ", this._grammarName, this.currentFunctionPointerLabel);
        }
        public override void VisitLeave(NPEG.NonTerminals.PrioritizedChoice expression)
        {
            this.methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1});", this._grammarName, this.currentFunctionPointerLabel);

            this.currentFunctionPointerLabel = this.methods.Peek().methodName;

            MethodInfo m = this.methods.Pop();

            this.finalMethods.Push(m);
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>(),
                    this.finalMethods.Peek().methodData
                );
        }




        public override void VisitEnter(NPEG.NonTerminals.Sequence expression)
        {
            this.methods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.methods.Peek().methodData += String.Format("\treturn this->sequence(");
        }
        public override void VisitExecute(NPEG.NonTerminals.Sequence expression)
        {
            this.methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1}, ", this._grammarName, this.currentFunctionPointerLabel);
        }
        public override void VisitLeave(NPEG.NonTerminals.Sequence expression)
        {
            this.methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1});", this._grammarName, this.currentFunctionPointerLabel);

            this.currentFunctionPointerLabel = this.methods.Peek().methodName;

            MethodInfo m = this.methods.Pop();

            this.finalMethods.Push(m);
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>(),
                    this.finalMethods.Peek().methodData
                );
        }





        public override void VisitEnter(NPEG.NonTerminals.AndPredicate expression)
        {
            this.methods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.methods.Peek().methodData += String.Format("\treturn this->andPredicate(");
        }
        public override void VisitExecute(NPEG.NonTerminals.AndPredicate expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.AndPredicate expression)
        {
            this.methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1});", this._grammarName, this.currentFunctionPointerLabel);

            this.currentFunctionPointerLabel = this.methods.Peek().methodName;

            MethodInfo m = this.methods.Pop();

            this.finalMethods.Push(m);
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>(),
                    this.finalMethods.Peek().methodData
                );
        }





        public override void VisitEnter(NPEG.NonTerminals.NotPredicate expression)
        {
            this.methods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.methods.Peek().methodData += String.Format("\treturn this->notPredicate(");
        }
        public override void VisitExecute(NPEG.NonTerminals.NotPredicate expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.NotPredicate expression)
        {
            this.methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1});", this._grammarName, this.currentFunctionPointerLabel);

            this.currentFunctionPointerLabel = this.methods.Peek().methodName;

            MethodInfo m = this.methods.Pop();

            this.finalMethods.Push(m);
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>(),
                    this.finalMethods.Peek().methodData
                );
        }





        public override void VisitEnter(NPEG.NonTerminals.ZeroOrMore expression)
        {
            this.methods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.methods.Peek().methodData += String.Format("\treturn this->zeroOrMore(");
        }
        public override void VisitExecute(NPEG.NonTerminals.ZeroOrMore expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.ZeroOrMore expression)
        {
            this.methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1});", this._grammarName, this.currentFunctionPointerLabel);

            this.currentFunctionPointerLabel = this.methods.Peek().methodName;

            MethodInfo m = this.methods.Pop();

            this.finalMethods.Push(m);
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>(),
                    this.finalMethods.Peek().methodData
                );
        }





        public override void VisitEnter(NPEG.NonTerminals.OneOrMore expression)
        {
            this.methods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.methods.Peek().methodData += String.Format("\treturn this->oneOrMore(");
        }
        public override void VisitExecute(NPEG.NonTerminals.OneOrMore expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.OneOrMore expression)
        {
            this.methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1});", this._grammarName, this.currentFunctionPointerLabel);

            this.currentFunctionPointerLabel = this.methods.Peek().methodName;

            MethodInfo m = this.methods.Pop();

            this.finalMethods.Push(m);
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>(),
                    this.finalMethods.Peek().methodData
                );
        }





        public override void VisitEnter(NPEG.NonTerminals.Optional expression)
        {
            this.methods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.methods.Peek().methodData += String.Format("\treturn this->optional(");
        }
        public override void VisitExecute(NPEG.NonTerminals.Optional expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.Optional expression)
        {
            this.methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1});", this._grammarName, this.currentFunctionPointerLabel);

            this.currentFunctionPointerLabel = this.methods.Peek().methodName;

            MethodInfo m = this.methods.Pop();

            this.finalMethods.Push(m);
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>(),
                    this.finalMethods.Peek().methodData
                );
        }



        public override void VisitEnter(NPEG.NonTerminals.CapturingGroup expression)
        {
            this.methods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.methods.Peek().methodData += String.Format("\treturn this->capturingGroup(");
        }
        public override void VisitExecute(NPEG.NonTerminals.CapturingGroup expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.CapturingGroup expression)
        {
            String variableName = "_nodeName_0";

            this.methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1}, {2}, {3}, NULL);", 
                this._grammarName, this.currentFunctionPointerLabel, variableName, expression.DoReplaceBySingleChildNode? "1":"0");

            this.currentFunctionPointerLabel = this.methods.Peek().methodName;

            MethodInfo m = this.methods.Pop();

            this.finalMethods.Push(m);
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>() { { variableName, expression.Name } },
                    this.finalMethods.Peek().methodData
                );
        }





        private Dictionary<String, String> recursion = new Dictionary<string, string>();
        public override void VisitEnter(NPEG.NonTerminals.RecursionCreate expression)
        {
            String _methodName = this.CreateMethodName();
            this.methods.Push(new MethodInfo() { methodName = _methodName });
            recursion.Add(expression.FunctionName, _methodName);
        }
        public override void VisitExecute(NPEG.NonTerminals.RecursionCreate expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.RecursionCreate expression)
        {
            this.methods.Peek().methodData += String.Format("\treturn this->{0}();", this.currentFunctionPointerLabel);
            this.currentFunctionPointerLabel = this.methods.Peek().methodName;

            MethodInfo m = this.methods.Pop();

            this.finalMethods.Push(m);
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>(),
                    this.finalMethods.Peek().methodData
                );
        }





        public override void VisitEnter(NPEG.NonTerminals.LimitingRepetition expression)
        {
            this.methods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.methods.Peek().methodData += String.Format("\treturn this->limitingRepetition(");
        }
        public override void VisitExecute(NPEG.NonTerminals.LimitingRepetition expression)
        {
            this.methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1}, ", this._grammarName, this.currentFunctionPointerLabel);
        }
        public override void VisitLeave(NPEG.NonTerminals.LimitingRepetition expression)
        {
            this.methods.Peek().methodData += String.Format("{0}, {1});\n",
                        (expression.Min == null) ? "NULL" : "0",
                        (expression.Max == null) ? "NULL" : "0");

            this.currentFunctionPointerLabel = this.methods.Peek().methodName;

            MethodInfo m = this.methods.Pop();

            this.finalMethods.Push(m);
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>(),
                    this.finalMethods.Peek().methodData
                );
        }
        #endregion





        #region terminals
        public override void Visit(NPEG.Terminals.Literal expression)
        {
            String variableName = "_literal_0";
            this.finalMethods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>() { { variableName, expression.MatchText } },
                    String.Format(
                        "\treturn this->literal({0}, {1}, {2});",
                        variableName,
                        expression.MatchText.Length,
                        expression.IsCaseSensitive ? 1 : 0
                    )
                );
            this.currentFunctionPointerLabel = this.finalMethods.Peek().methodName;
        }

        public override void Visit(NPEG.Terminals.CharacterClass expression)
        {
            String variableName = "_classExpression_0";
            this.finalMethods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            List<String> escapeSequence = new List<string>();
            escapeSequence.Add(@"\a"); // alert bell
            escapeSequence.Add(@"\b"); // backspace
            escapeSequence.Add(@"\f"); // formfeed 
            escapeSequence.Add(@"\n"); // newline
            escapeSequence.Add(@"\r"); // carriage return
            escapeSequence.Add(@"\t"); // horizontal tab
            escapeSequence.Add(@"\v"); // vertical tab
            escapeSequence.Add(@"\\"); // backslash
            escapeSequence.Add(@"\"""); // double quotes
            var foundEscapeSequence = escapeSequence.Where(s => expression.ClassExpression.Contains(s)).Count();
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>() { { variableName, expression.ClassExpression } },
                    String.Format(
                        "\treturn this->characterClass({0}, {1});",
                        variableName, expression.ClassExpression.Length - foundEscapeSequence
                    )
                );
            this.currentFunctionPointerLabel = this.finalMethods.Peek().methodName;
        }

        public override void Visit(NPEG.Terminals.AnyCharacter expression)
        {
            this.finalMethods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>(),
                    String.Format("\treturn this->anyCharacter();")
                );
            this.currentFunctionPointerLabel = this.finalMethods.Peek().methodName;
        }

        public override void Visit(NPEG.Terminals.RecursionCall expression)
        {
            this.finalMethods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>(),
                    String.Format(
                        "\treturn this->recursionCall((Npeg::IsMatchPredicate)&{0}::{1});", 
                        this._grammarName, this.recursion[expression.FunctionName]
                    )
                );
            this.currentFunctionPointerLabel = this.finalMethods.Peek().methodName;
        }

        public override void Visit(NPEG.Terminals.DynamicBackReference expression)
        {
            String variableName = "_dynamicBackReference_0";
            this.finalMethods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>() { { variableName, expression.BackReferenceName } },
                    String.Format(
                        "\treturn this->dynamicBackReference({0}, {1});",
                        variableName,
                        expression.IsCaseSensitive ? 1 : 0
                    )
                );
            this.currentFunctionPointerLabel = this.finalMethods.Peek().methodName;
        }

        public override void Visit(NPEG.Terminals.CodePoint expression)
        {
            throw new NotImplementedException("CodePoint");
        }

        public override void Visit(NPEG.Terminals.Warn expression)
        {
            String variableName = "_warning_0";
            this.finalMethods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>() { { variableName, expression.Message } },
                    String.Format(
                        "\treturn this->warn({0});",
                        variableName
                    )
                );
            this.currentFunctionPointerLabel = this.finalMethods.Peek().methodName;
        }

        public override void Visit(NPEG.Terminals.Fatal expression)
        {
            String variableName = "_fatal_0";
            this.finalMethods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>() { { variableName, expression.Message } },
                    String.Format(
                        "\treturn this->fatal({0});",
                        variableName
                    )
                );
            this.currentFunctionPointerLabel = this.finalMethods.Peek().methodName;
        }
        #endregion





        private Int32 _uniqueFunctionID = 0;
        private String CreateMethodName()
        {
            return this._grammarName + @"_impl_" + (this._uniqueFunctionID++).ToString();
        }



        private String CreateMethod(String methodName, Dictionary<String, Object> localVariables, String methodText)
        {
            StringBuilder locals = new StringBuilder();
            foreach (String key in localVariables.Keys)
            {
                if (localVariables[key] is String)
                {
                    locals.AppendFormat("\tconst char* {0} = \"{1}\";\n", key, localVariables[key].ToString().Replace(@"""", @"\"""));
                }
            }

            return "int " + this._grammarName + "::" + methodName + "()\n" +
                   "{\n"+
                   ((locals.ToString().Length > 0)? locals.ToString() + "\n" : "") 
                   +
                    methodText
                   +
                   "\n}";
        }
    }
}
