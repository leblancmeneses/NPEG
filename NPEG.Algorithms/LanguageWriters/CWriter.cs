using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using NPEG.NonTerminals;
using NPEG.Terminals;

namespace NPEG.Algorithms.LanguageWriters
{
	public class CWriter : WriterBase
	{
		private readonly String _folderPath;
		private readonly String _grammarName;

		private readonly Stack<MethodInfo> finalMethods = new Stack<MethodInfo>();
		private readonly Stack<MethodInfo> methods = new Stack<MethodInfo>();
		private Int32 _uniqueFunctionID;
		private String currentFunctionPointerLabel = String.Empty;


		public CWriter(String grammarName, String folderPath)
		{
			_grammarName = grammarName;
			_folderPath = folderPath;
		}


		public override void Write()
		{
			var header = new StringBuilder();
			var src = new StringBuilder();

			src.Append("#include <stdlib.h>\n");
			src.AppendFormat("#include \"{0}.h\"\n\n", _grammarName);


			header.AppendFormat("#ifndef ROBUSTHAVEN_GENERATEDPARSER_{0}\n", _grammarName.ToUpper());
			header.AppendFormat("#define ROBUSTHAVEN_GENERATEDPARSER_{0}\n\n", _grammarName.ToUpper());
			header.Append("#include \"robusthaven/text/npeg_inputiterator.h\"\n");
			header.Append("#include \"robusthaven/text/npeg.h\"\n\n");


			while (finalMethods.Count > 0)
			{
				MethodInfo m = finalMethods.Pop();
				header.AppendLine(String.Format("int {0}(npeg_inputiterator*, npeg_context*);", m.methodName));
				src.AppendLine(m.methodData);
			}

			header.Append("\n#endif\n");


			DirectoryInfo info = Directory.CreateDirectory(_folderPath);
			using (StreamWriter writer = File.CreateText(Path.Combine(info.FullName, "main.c")))
			{
				String startMethodName = _grammarName + @"_impl_0";
				var main = new StringBuilder();
				main.Append("#include <stdio.h>\n");
				main.Append("#include <string.h>\n\n");

				main.Append("#include \"robusthaven/text/npeg.h\"\n");
				main.Append("#include \"robusthaven/text/npeg_inputiterator.h\"\n");
				main.Append("#include \"robusthaven/structures/stack.h\"\n\n");

				main.AppendFormat("#include \"{0}.h\"\n", _grammarName);

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

			using (StreamWriter writer = File.CreateText(Path.Combine(info.FullName, String.Format("{0}.c", _grammarName))))
			{
				writer.Write(src.ToString());
			}

			using (StreamWriter writer = File.CreateText(Path.Combine(info.FullName, String.Format("{0}.h", _grammarName))))
			{
				writer.Write(header.ToString());
			}

			Debug.WriteLine(info.FullName);
		}

		private String CreateMethodName()
		{
			return _grammarName + @"_impl_" + (_uniqueFunctionID++);
		}


		private String CreateMethod(String methodName, Dictionary<String, Object> localVariables, String methodText)
		{
			var locals = new StringBuilder();
			foreach (String key in localVariables.Keys)
			{
				if (localVariables[key] is String)
				{
					locals.AppendFormat("\tchar* {0} = \"{1}\";\n", key, localVariables[key]);
				}
			}

			return "int " + methodName + "(npeg_inputiterator* iterator, npeg_context* context)\n" +
			       "{\n" +
			       ((locals.ToString().Length > 0) ? locals + "\n" : "")
			       +
			       methodText
			       +
			       "\n}";
		}

		#region non terminals

		private readonly Dictionary<String, String> recursion = new Dictionary<string, string>();

		public override void VisitEnter(PrioritizedChoice expression)
		{
			methods.Push(new MethodInfo {methodName = CreateMethodName()});
			methods.Peek().methodData += String.Format("\treturn npeg_PrioritizedChoice(iterator,  context, ");
		}

		public override void VisitExecute(PrioritizedChoice expression)
		{
			methods.Peek().methodData += String.Format("&{0}, ", currentFunctionPointerLabel);
		}

		public override void VisitLeave(PrioritizedChoice expression)
		{
			methods.Peek().methodData += String.Format("&{0});", currentFunctionPointerLabel);

			currentFunctionPointerLabel = methods.Peek().methodName;

			MethodInfo m = methods.Pop();

			finalMethods.Push(m);
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object>(),
					finalMethods.Peek().methodData
					);
		}


		public override void VisitEnter(Sequence expression)
		{
			methods.Push(new MethodInfo {methodName = CreateMethodName()});
			methods.Peek().methodData += String.Format("\treturn npeg_Sequence(iterator,  context, ");
		}

		public override void VisitExecute(Sequence expression)
		{
			methods.Peek().methodData += String.Format("&{0}, ", currentFunctionPointerLabel);
		}

		public override void VisitLeave(Sequence expression)
		{
			methods.Peek().methodData += String.Format("&{0});", currentFunctionPointerLabel);

			currentFunctionPointerLabel = methods.Peek().methodName;

			MethodInfo m = methods.Pop();

			finalMethods.Push(m);
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object>(),
					finalMethods.Peek().methodData
					);
		}


		public override void VisitEnter(AndPredicate expression)
		{
			methods.Push(new MethodInfo {methodName = CreateMethodName()});
			methods.Peek().methodData += String.Format("\treturn npeg_AndPredicate(iterator,  context, ");
		}

		public override void VisitExecute(AndPredicate expression)
		{
		}

		public override void VisitLeave(AndPredicate expression)
		{
			methods.Peek().methodData += String.Format("&{0});", currentFunctionPointerLabel);

			currentFunctionPointerLabel = methods.Peek().methodName;

			MethodInfo m = methods.Pop();

			finalMethods.Push(m);
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object>(),
					finalMethods.Peek().methodData
					);
		}


		public override void VisitEnter(NotPredicate expression)
		{
			methods.Push(new MethodInfo {methodName = CreateMethodName()});
			methods.Peek().methodData += String.Format("\treturn npeg_NotPredicate(iterator,  context, ");
		}

		public override void VisitExecute(NotPredicate expression)
		{
		}

		public override void VisitLeave(NotPredicate expression)
		{
			methods.Peek().methodData += String.Format("&{0});", currentFunctionPointerLabel);

			currentFunctionPointerLabel = methods.Peek().methodName;

			MethodInfo m = methods.Pop();

			finalMethods.Push(m);
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object>(),
					finalMethods.Peek().methodData
					);
		}


		public override void VisitEnter(ZeroOrMore expression)
		{
			methods.Push(new MethodInfo {methodName = CreateMethodName()});
			methods.Peek().methodData += String.Format("\treturn npeg_ZeroOrMore(iterator,  context, ");
		}

		public override void VisitExecute(ZeroOrMore expression)
		{
		}

		public override void VisitLeave(ZeroOrMore expression)
		{
			methods.Peek().methodData += String.Format("&{0});", currentFunctionPointerLabel);

			currentFunctionPointerLabel = methods.Peek().methodName;

			MethodInfo m = methods.Pop();

			finalMethods.Push(m);
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object>(),
					finalMethods.Peek().methodData
					);
		}


		public override void VisitEnter(OneOrMore expression)
		{
			methods.Push(new MethodInfo {methodName = CreateMethodName()});
			methods.Peek().methodData += String.Format("\treturn npeg_OneOrMore(iterator,  context, ");
		}

		public override void VisitExecute(OneOrMore expression)
		{
		}

		public override void VisitLeave(OneOrMore expression)
		{
			methods.Peek().methodData += String.Format("&{0});", currentFunctionPointerLabel);

			currentFunctionPointerLabel = methods.Peek().methodName;

			MethodInfo m = methods.Pop();

			finalMethods.Push(m);
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object>(),
					finalMethods.Peek().methodData
					);
		}


		public override void VisitEnter(Optional expression)
		{
			methods.Push(new MethodInfo {methodName = CreateMethodName()});
			methods.Peek().methodData += String.Format("\treturn npeg_Optional(iterator,  context, ");
		}

		public override void VisitExecute(Optional expression)
		{
		}

		public override void VisitLeave(Optional expression)
		{
			methods.Peek().methodData += String.Format("&{0});", currentFunctionPointerLabel);

			currentFunctionPointerLabel = methods.Peek().methodName;

			MethodInfo m = methods.Pop();

			finalMethods.Push(m);
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object>(),
					finalMethods.Peek().methodData
					);
		}


		public override void VisitEnter(CapturingGroup expression)
		{
			methods.Push(new MethodInfo {methodName = CreateMethodName()});
			methods.Peek().methodData += String.Format("\treturn npeg_CapturingGroup(iterator,  context, ");
		}

		public override void VisitExecute(CapturingGroup expression)
		{
		}

		public override void VisitLeave(CapturingGroup expression)
		{
			String variableName = "_nodeName_0";

			methods.Peek().methodData += String.Format("&{0}, {1}, {2}, NULL);",
			                                           currentFunctionPointerLabel, variableName,
			                                           expression.DoReplaceBySingleChildNode ? "1" : "0");

			currentFunctionPointerLabel = methods.Peek().methodName;

			MethodInfo m = methods.Pop();

			finalMethods.Push(m);
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object> {{variableName, expression.Name}},
					finalMethods.Peek().methodData
					);
		}


		public override void VisitEnter(RecursionCreate expression)
		{
			String _methodName = CreateMethodName();
			methods.Push(new MethodInfo {methodName = _methodName});
			recursion.Add(expression.FunctionName, _methodName);
		}

		public override void VisitExecute(RecursionCreate expression)
		{
		}

		public override void VisitLeave(RecursionCreate expression)
		{
			methods.Peek().methodData += String.Format("\treturn {0}(iterator, context);", currentFunctionPointerLabel);
			currentFunctionPointerLabel = methods.Peek().methodName;

			MethodInfo m = methods.Pop();

			finalMethods.Push(m);
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object>(),
					finalMethods.Peek().methodData
					);
		}


		public override void VisitEnter(LimitingRepetition expression)
		{
			methods.Push(new MethodInfo {methodName = CreateMethodName()});
			methods.Peek().methodData += String.Format("\treturn npeg_LimitingRepetition(iterator,  context, ");
		}

		public override void VisitExecute(LimitingRepetition expression)
		{
			methods.Peek().methodData += String.Format("&{0}, ", currentFunctionPointerLabel);
		}

		public override void VisitLeave(LimitingRepetition expression)
		{
			methods.Peek().methodData += String.Format("{0}, {1});\n",
			                                           (expression.Min == null) ? "NULL" : "0",
			                                           (expression.Max == null) ? "NULL" : "0");

			currentFunctionPointerLabel = methods.Peek().methodName;

			MethodInfo m = methods.Pop();

			finalMethods.Push(m);
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object>(),
					finalMethods.Peek().methodData
					);
		}

		#endregion

		#region terminals

		public override void Visit(Literal expression)
		{
			String variableName = "_literal_0";
			finalMethods.Push(new MethodInfo {methodName = CreateMethodName()});
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object> {{variableName, expression.MatchText}},
					String.Format(
						"\treturn npeg_Literal(iterator,  context, {0}, {1}, {2});",
						variableName,
						expression.MatchText.Length,
						expression.IsCaseSensitive ? 1 : 0
						)
					);
			currentFunctionPointerLabel = finalMethods.Peek().methodName;
		}

		public override void Visit(CharacterClass expression)
		{
			String variableName = "_classExpression_0";
			finalMethods.Push(new MethodInfo {methodName = CreateMethodName()});
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object> {{variableName, expression.ClassExpression}},
					String.Format(
						"\treturn npeg_CharacterClass(iterator,  context, {0}, {1});",
						variableName, expression.ClassExpression.Length
						)
					);
			currentFunctionPointerLabel = finalMethods.Peek().methodName;
		}

		public override void Visit(AnyCharacter expression)
		{
			finalMethods.Push(new MethodInfo {methodName = CreateMethodName()});
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object>(),
					String.Format("\treturn npeg_AnyCharacter(iterator,  context);")
					);
			currentFunctionPointerLabel = finalMethods.Peek().methodName;
		}

		public override void Visit(RecursionCall expression)
		{
			finalMethods.Push(new MethodInfo {methodName = CreateMethodName()});
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object>(),
					String.Format(
						"\treturn npeg_RecursionCall(iterator,  context, &{0});",
						recursion[expression.FunctionName]
						)
					);
			currentFunctionPointerLabel = finalMethods.Peek().methodName;
		}

		public override void Visit(DynamicBackReference expression)
		{
			String variableName = "_dynamicBackReference_0";
			finalMethods.Push(new MethodInfo {methodName = CreateMethodName()});
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object> {{variableName, expression.BackReferenceName}},
					String.Format(
						"\treturn npeg_DynamicBackReference(iterator,  context, {0}, {1});",
						variableName,
						expression.IsCaseSensitive ? 1 : 0
						)
					);
			currentFunctionPointerLabel = finalMethods.Peek().methodName;
		}

		public override void Visit(CodePoint expression)
		{
			throw new NotImplementedException("CodePoint");
		}

		public override void Visit(Warn expression)
		{
			String variableName = "_warning_0";
			finalMethods.Push(new MethodInfo {methodName = CreateMethodName()});
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object> {{variableName, expression.Message}},
					String.Format(
						"\treturn npeg_Warning(iterator,  context, {0});",
						variableName
						)
					);
			currentFunctionPointerLabel = finalMethods.Peek().methodName;
		}

		public override void Visit(Fatal expression)
		{
			String variableName = "_fatal_0";
			finalMethods.Push(new MethodInfo {methodName = CreateMethodName()});
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object> {{variableName, expression.Message}},
					String.Format(
						"\treturn npeg_Fatal(iterator,  context, {0});",
						variableName
						)
					);
			currentFunctionPointerLabel = finalMethods.Peek().methodName;
		}

		#endregion

		#region Nested type: MethodInfo

		private class MethodInfo
		{
			public String methodData = String.Empty;
			public String methodName = String.Empty;
		}

		#endregion
	}
}