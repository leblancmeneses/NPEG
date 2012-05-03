using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NPEG.Algorithms.LanguageWriters
{
    public class JavaWriter : WriterBase
    {
        private String _grammarName;
        private String _folderPath;
        private String _packagename;

        private String currentFunctionPointerLabel = String.Empty;

        private class MethodInfo
        {
            public String methodName = String.Empty;
            public String methodData = String.Empty;
        }
        private Stack<MethodInfo> methods = new Stack<MethodInfo>();
        private Stack<MethodInfo> finalMethods = new Stack<MethodInfo>();






        public JavaWriter(String grammarName, String package, String folderPath)
        {
            this._grammarName = grammarName;
            this._folderPath = folderPath;
            this._packagename = package;
        }


        public override void Write()
        {
            System.IO.DirectoryInfo info = System.IO.Directory.CreateDirectory(this._folderPath);



            String startMethodName = this._grammarName + @"_impl_0";
            //this._grammarName

            using (System.IO.StreamWriter writer = System.IO.File.CreateText(System.IO.Path.Combine(info.FullName, String.Format("{0}.java", this._grammarName))))
            {
                StringBuilder src = new StringBuilder();
                src.AppendFormat("package {0};\n", this._packagename);
                src.Append("import java.io.IOException;\n");
                src.Append("import robusthaven.text.*;\n");
                src.Append("\n");
                src.AppendFormat("public class {0} extends Npeg\n", this._grammarName);
                src.Append("{\n");

                src.AppendLine(String.Format("\tpublic {0}(InputIterator iterator)\n\t{{\n\t\tsuper(iterator);\n\t}}\n", this._grammarName));

                src.Append("\tpublic boolean isMatch() throws IOException, ParsingFatalTerminalException\n");
                src.Append("\t{\n");
                src.Append("\t\treturn new " + this._grammarName + @"_impl_0" + "().evaluate();\n");
                src.Append("\t}\n");

                src.Append("\n");

                while (finalMethods.Count > 0)
                {
                    src.Append("\n");
                    MethodInfo m = finalMethods.Pop();
                    String[] lines = Regex.Split(m.methodData, @"(.*)$", RegexOptions.Multiline);
                    foreach (var line in lines)
                    {
                        if (!String.IsNullOrEmpty(line.Trim()))
                            src.AppendLine("\t" + line);
                    }
                }

                src.Append("}\n"); // class

                writer.Write(src.ToString());
            }

            System.Diagnostics.Debug.WriteLine(info.FullName);
        }







        #region non terminals
        public override void VisitEnter(NPEG.NonTerminals.PrioritizedChoice expression)
        {
            this.methods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.methods.Peek().methodData += String.Format("\treturn prioritizedChoice(");
        }
        public override void VisitExecute(NPEG.NonTerminals.PrioritizedChoice expression)
        {
            this.methods.Peek().methodData += String.Format("new {1}(), ", this._grammarName, this.currentFunctionPointerLabel);
        }
        public override void VisitLeave(NPEG.NonTerminals.PrioritizedChoice expression)
        {
            this.methods.Peek().methodData += String.Format("new {1}());", this._grammarName, this.currentFunctionPointerLabel);

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
            this.methods.Peek().methodData += String.Format("\treturn sequence(");
        }
        public override void VisitExecute(NPEG.NonTerminals.Sequence expression)
        {
            this.methods.Peek().methodData += String.Format("new {1}(), ", this._grammarName, this.currentFunctionPointerLabel);
        }
        public override void VisitLeave(NPEG.NonTerminals.Sequence expression)
        {
            this.methods.Peek().methodData += String.Format("new {1}());", this._grammarName, this.currentFunctionPointerLabel);

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
            this.methods.Peek().methodData += String.Format("\treturn andPredicate(");
        }
        public override void VisitExecute(NPEG.NonTerminals.AndPredicate expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.AndPredicate expression)
        {
            this.methods.Peek().methodData += String.Format("new {1}());", this._grammarName, this.currentFunctionPointerLabel);

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
            this.methods.Peek().methodData += String.Format("\treturn notPredicate(");
        }
        public override void VisitExecute(NPEG.NonTerminals.NotPredicate expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.NotPredicate expression)
        {
            this.methods.Peek().methodData += String.Format("new {1}());", this._grammarName, this.currentFunctionPointerLabel);

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
            this.methods.Peek().methodData += String.Format("\treturn zeroOrMore(");
        }
        public override void VisitExecute(NPEG.NonTerminals.ZeroOrMore expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.ZeroOrMore expression)
        {
#warning need to revert peg composite to string grammar rules ... so I can send second argument.
            this.methods.Peek().methodData += String.Format("new {1}(), \"\");", this._grammarName, this.currentFunctionPointerLabel);

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
            this.methods.Peek().methodData += String.Format("\treturn oneOrMore(");
        }
        public override void VisitExecute(NPEG.NonTerminals.OneOrMore expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.OneOrMore expression)
        {
            this.methods.Peek().methodData += String.Format("new {1}(), \"\");", this._grammarName, this.currentFunctionPointerLabel);

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
            this.methods.Peek().methodData += String.Format("\treturn optional(");
        }
        public override void VisitExecute(NPEG.NonTerminals.Optional expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.Optional expression)
        {
            this.methods.Peek().methodData += String.Format("new {1}());", this._grammarName, this.currentFunctionPointerLabel);

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
            this.methods.Peek().methodData += String.Format("\treturn capturingGroup(");
        }
        public override void VisitExecute(NPEG.NonTerminals.CapturingGroup expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.CapturingGroup expression)
        {
            String variableName = "_nodeName_0";

            this.methods.Peek().methodData += String.Format("new {1}(), {2}, {3}, false);",
                this._grammarName, this.currentFunctionPointerLabel, variableName, expression.DoReplaceBySingleChildNode ? "true" : "false");

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
            this.methods.Peek().methodData += String.Format("\treturn {0}();", this.currentFunctionPointerLabel);
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
            this.methods.Peek().methodData += String.Format("\treturn limitingRepetition(");
        }
        public override void VisitExecute(NPEG.NonTerminals.LimitingRepetition expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.LimitingRepetition expression)
        {
            this.methods.Peek().methodData += String.Format("new {0}(), ", this.currentFunctionPointerLabel);
            this.methods.Peek().methodData += String.Format("{0}, {1}, \"\");\n",
                        (expression.Min == null) ? "null" : expression.Min.ToString(),
                        (expression.Max == null) ? "null" : expression.Max.ToString());

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
                        "\treturn literal({0}, {1});",
                        variableName,
                        expression.IsCaseSensitive ? "true" : "false"
                    )
                );
            this.currentFunctionPointerLabel = this.finalMethods.Peek().methodName;
        }

        public override void Visit(NPEG.Terminals.CharacterClass expression)
        {
            String variableName = "_classExpression_0";
            this.finalMethods.Push(new MethodInfo() { methodName = this.CreateMethodName() });
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>() { { variableName, expression.ClassExpression } },
                    String.Format(
                        "\treturn characterClass({0}, {1});",
                        variableName, expression.ClassExpression.Length
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
                    String.Format("\treturn anyCharacter();")
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
                        "\treturn recursionCall(new {1}());",
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
                        "\treturn dynamicBackReference({0}, {1});",
                        variableName,
                        expression.IsCaseSensitive ? "true" : "false"
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
                        "\treturn warn({0});",
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
                        "\treturn fatal({0});",
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
                    locals.AppendFormat("\t\tString {0} = \"{1}\";\n", key, localVariables[key]);
                }
            }

            return "protected class " + methodName + " implements IsMatchPredicate\n"+
                   "{\n"+
                        "\tpublic boolean evaluate() throws ParsingFatalTerminalException, InfiniteLoopException, IOException\n" +
                       "\t{\n" +
                            ((locals.ToString().Length > 0) ? locals.ToString() + "\n" : "")
                        +
                        "\t"+    methodText
                        +
                        "\n\t}\n" +
                  "}\n";
        }


    }
}
