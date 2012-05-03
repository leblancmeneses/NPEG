using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPEG.Algorithms.LanguageWriters
{
    public class CWriter : WriterBase
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

        




        public CWriter(String grammarName, String folderPath)
        {
            this._grammarName = grammarName;
            this._folderPath = folderPath;
        }


        public override void Write()
        {
            StringBuilder header = new StringBuilder();
            StringBuilder src = new StringBuilder();

            src.Append("#include <stdlib.h>\n");
            src.AppendFormat("#include \"{0}.h\"\n\n", this._grammarName);

            
            header.AppendFormat("#ifndef ROBUSTHAVEN_GENERATEDPARSER_{0}\n", this._grammarName.ToUpper());
            header.AppendFormat("#define ROBUSTHAVEN_GENERATEDPARSER_{0}\n\n", this._grammarName.ToUpper());
            header.Append("#include \"robusthaven/text/npeg_inputiterator.h\"\n");
            header.Append("#include \"robusthaven/text/npeg.h\"\n\n");


            while (finalMethods.Count > 0)
            {
                MethodInfo m = finalMethods.Pop();
                header.AppendLine(String.Format("int {0}(npeg_inputiterator*, npeg_context*);", m.methodName));
                src.AppendLine(m.methodData);
            }

            header.Append("\n#endif\n");


            System.IO.DirectoryInfo info = System.IO.Directory.CreateDirectory(this._folderPath);
            using (System.IO.StreamWriter writer = System.IO.File.CreateText(System.IO.Path.Combine(info.FullName, "main.c")))
            {
                String startMethodName = this._grammarName + @"_impl_0";
                StringBuilder main = new StringBuilder();
                main.Append("#include <stdio.h>\n");
                main.Append("#include <string.h>\n\n");

                main.Append("#include \"robusthaven/text/npeg.h\"\n");
                main.Append("#include \"robusthaven/text/npeg_inputiterator.h\"\n");
                main.Append("#include \"robusthaven/structures/stack.h\"\n\n");

                main.AppendFormat("#include \"{0}.h\"\n", this._grammarName);

                main.Append("\n");
                main.AppendLine("int main(int argc, char *argv[])");
                main.AppendLine("{");
                main.AppendLine("\trh_stack_instance disableBackReferenceStack;");
                main.AppendLine("\trh_stackstack_instance sandbox;");
                main.AppendLine("\trh_hashmap_instance backreference_lookup;");
                main.AppendLine("\trh_list_instance warnings;");
                main.AppendLine("\trh_stack_instance errors;");
                main.Append("\n");
                main.AppendLine("\tnpeg_context context = {");
                main.AppendLine("\t\t&disableBackReferenceStack,");
                main.AppendLine("\t\t&sandbox,");
                main.AppendLine("\t\t&backreference_lookup,");
                main.AppendLine("\t\t&warnings,");
                main.AppendLine("\t\t&errors");
                main.AppendLine("\t};");
                main.Append("\n");
                main.AppendLine("\tnpeg_inputiterator iterator;");
                main.AppendLine("\tnpeg_astnode* ast;");
                main.AppendLine("\tchar* text1 = \"\";");
                main.Append("\n");
                main.AppendFormat("\tint (*parsetree)(npeg_inputiterator*, npeg_context*) = &{0};\n", startMethodName);
                main.AppendLine("\tint IsMatch = 0;");
                main.Append("\n");
                main.AppendLine("\t// load npeg managed memory");
                main.AppendLine("\tnpeg_inputiterator_constructor(&iterator, text1, strlen(text1));");
                main.AppendLine("\tnpeg_constructor(&context);");
                main.Append("\n");
                main.AppendFormat("\tIsMatch = parsetree(&iterator, &context);\n");
                main.Append("\n");
                main.AppendLine("\tnpeg_destructor(&context);");
                main.AppendLine("\tnpeg_inputiterator_destructor(&iterator);");
                main.AppendLine("\t// unload npeg managed memory");             
                main.Append("\n");
                main.AppendLine("\treturn 0;");
                main.AppendLine("}");
                writer.Write(main.ToString());
            }

            using(System.IO.StreamWriter writer = System.IO.File.CreateText( System.IO.Path.Combine( info.FullName, String.Format("{0}.c", this._grammarName) )))
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
            this.methods.Peek().methodData += String.Format("\treturn npeg_PrioritizedChoice(iterator,  context, ");
        }
        public override void VisitExecute(NPEG.NonTerminals.PrioritizedChoice expression)
        {
            this.methods.Peek().methodData += String.Format("&{0}, ", this.currentFunctionPointerLabel);
        }
        public override void VisitLeave(NPEG.NonTerminals.PrioritizedChoice expression)
        {
            this.methods.Peek().methodData += String.Format("&{0});", this.currentFunctionPointerLabel);

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
            this.methods.Peek().methodData += String.Format("\treturn npeg_Sequence(iterator,  context, ");
        }
        public override void VisitExecute(NPEG.NonTerminals.Sequence expression)
        {
            this.methods.Peek().methodData += String.Format("&{0}, ", this.currentFunctionPointerLabel);
        }
        public override void VisitLeave(NPEG.NonTerminals.Sequence expression)
        {
            this.methods.Peek().methodData += String.Format("&{0});", this.currentFunctionPointerLabel);

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
            this.methods.Peek().methodData += String.Format("\treturn npeg_AndPredicate(iterator,  context, ");
        }
        public override void VisitExecute(NPEG.NonTerminals.AndPredicate expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.AndPredicate expression)
        {
            this.methods.Peek().methodData += String.Format("&{0});", this.currentFunctionPointerLabel);

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
            this.methods.Peek().methodData += String.Format("\treturn npeg_NotPredicate(iterator,  context, ");
        }
        public override void VisitExecute(NPEG.NonTerminals.NotPredicate expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.NotPredicate expression)
        {
            this.methods.Peek().methodData += String.Format("&{0});", this.currentFunctionPointerLabel);

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
            this.methods.Peek().methodData += String.Format("\treturn npeg_ZeroOrMore(iterator,  context, ");
        }
        public override void VisitExecute(NPEG.NonTerminals.ZeroOrMore expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.ZeroOrMore expression)
        {
            this.methods.Peek().methodData += String.Format("&{0});", this.currentFunctionPointerLabel);

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
            this.methods.Peek().methodData += String.Format("\treturn npeg_OneOrMore(iterator,  context, ");
        }
        public override void VisitExecute(NPEG.NonTerminals.OneOrMore expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.OneOrMore expression)
        {
            this.methods.Peek().methodData += String.Format("&{0});", this.currentFunctionPointerLabel);

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
            this.methods.Peek().methodData += String.Format("\treturn npeg_Optional(iterator,  context, ");
        }
        public override void VisitExecute(NPEG.NonTerminals.Optional expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.Optional expression)
        {
            this.methods.Peek().methodData += String.Format("&{0});", this.currentFunctionPointerLabel);

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
            this.methods.Peek().methodData += String.Format("\treturn npeg_CapturingGroup(iterator,  context, ");
        }
        public override void VisitExecute(NPEG.NonTerminals.CapturingGroup expression)
        {
        }
        public override void VisitLeave(NPEG.NonTerminals.CapturingGroup expression)
        {
            String variableName = "_nodeName_0";

            this.methods.Peek().methodData += String.Format("&{0}, {1}, {2}, NULL);", 
                this.currentFunctionPointerLabel, variableName, expression.DoReplaceBySingleChildNode? "1":"0");

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
            this.methods.Peek().methodData += String.Format("\treturn {0}(iterator, context);", this.currentFunctionPointerLabel);
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
            this.methods.Peek().methodData += String.Format("\treturn npeg_LimitingRepetition(iterator,  context, ");
        }
        public override void VisitExecute(NPEG.NonTerminals.LimitingRepetition expression)
        {
            this.methods.Peek().methodData += String.Format("&{0}, ", this.currentFunctionPointerLabel);
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
                        "\treturn npeg_Literal(iterator,  context, {0}, {1}, {2});",
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
            this.finalMethods.Peek().methodData =
                this.CreateMethod(
                    this.finalMethods.Peek().methodName,
                    new Dictionary<string, object>() { { variableName, expression.ClassExpression } },
                    String.Format(
                        "\treturn npeg_CharacterClass(iterator,  context, {0}, {1});",
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
                    String.Format("\treturn npeg_AnyCharacter(iterator,  context);")
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
                        "\treturn npeg_RecursionCall(iterator,  context, &{0});",
                        this.recursion[expression.FunctionName]
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
                        "\treturn npeg_DynamicBackReference(iterator,  context, {0}, {1});",
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
                        "\treturn npeg_Warning(iterator,  context, {0});",
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
                        "\treturn npeg_Fatal(iterator,  context, {0});",
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
                    locals.AppendFormat("\tchar* {0} = \"{1}\";\n", key, localVariables[key]);
                }
            }

            return "int " + methodName + "(npeg_inputiterator* iterator, npeg_context* context)\n" +
                   "{\n"+
                   ((locals.ToString().Length > 0)? locals.ToString() + "\n" : "") 
                   +
                    methodText
                   +
                   "\n}";
        }
    }
}
