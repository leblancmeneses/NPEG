using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NPEG.NonTerminals;
using NPEG.Terminals;

namespace NPEG.Algorithms.LanguageWriters
{
	public class CPlusPlusWriter : WriterBase
	{
		private readonly String _folderPath;
		private readonly String _grammarName;

		private readonly Stack<MethodInfo> finalMethods = new Stack<MethodInfo>();
		private readonly Stack<MethodInfo> methods = new Stack<MethodInfo>();
		private Int32 _uniqueFunctionID;
		private String currentFunctionPointerLabel = String.Empty;


		public CPlusPlusWriter(String grammarName, String folderPath)
		{
			_grammarName = grammarName;
			_folderPath = folderPath;
		}


		public override void Write()
		{
			var header = new StringBuilder();
			var src = new StringBuilder();

			src.AppendFormat("#include \"{0}.h\"\n\n", _grammarName);
			src.Append("using namespace RobustHaven::Text;\n\n");


			header.AppendFormat("#ifndef ROBUSTHAVEN_GENERATEDPARSER_{0}\n", _grammarName.ToUpper());
			header.AppendFormat("#define ROBUSTHAVEN_GENERATEDPARSER_{0}\n\n", _grammarName.ToUpper());
			header.Append("#include \"robusthaven/text/InputIterator.h\"\n");
			header.Append("#include \"robusthaven/text/Npeg.h\"\n\n");
			header.Append("using namespace RobustHaven::Text;\n\n");
			header.AppendFormat("class {0} : public Npeg", _grammarName);
			header.Append("\n{\n");

			header.Append("\tpublic:\n");
			header.AppendLine(String.Format("\t\t{0}(InputIterator* inputstream): Npeg(inputstream){{}}\n", _grammarName));

			String startMethodName = _grammarName + @"_impl_0";
			header.Append("\t\tvirtual int isMatch() throw(ParsingFatalTerminalException) { return " + startMethodName +
			              "(); }\n");

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


			DirectoryInfo info = Directory.CreateDirectory(_folderPath);
			using (StreamWriter writer = File.CreateText(Path.Combine(info.FullName, "main.cpp")))
			{
				var main = new StringBuilder();
				main.Append("#include <cstring>\n");

				main.Append("#include \"robusthaven/text/InputIterator.h\"\n");
				main.AppendFormat("#include \"{0}.h\"\n\n", _grammarName);
				main.Append("using namespace RobustHaven::Text;\n");

				main.Append("\n");
				main.AppendLine("int main(int argc, char *argv[])");
				main.AppendLine("{");
				main.Append("\tconst char* stream = \"123-456-7890\";\n");
				main.Append("\tInputIterator* input = new InputIterator(stream, strlen(stream));\n");
				main.AppendFormat("\t{0}* parser = new {0}(input);", _grammarName);
				main.Append("\n");
				main.AppendLine("\tint IsMatch = parser->isMatch();\n");
				main.Append("\tdelete parser;\n");
				main.Append("\tdelete input;\n");
				main.Append("\n\n");
				main.AppendLine("\treturn 0;");
				main.AppendLine("}");
				writer.Write(main.ToString());
			}

			using (StreamWriter writer = File.CreateText(Path.Combine(info.FullName, String.Format("{0}.cpp", _grammarName))))
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
					locals.AppendFormat("\tconst char* {0} = \"{1}\";\n", key, localVariables[key].ToString().Replace(@"""", @"\"""));
				}
			}

			return "int " + _grammarName + "::" + methodName + "()\n" +
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
			methods.Peek().methodData += String.Format("\treturn this->prioritizedChoice(");
		}

		public override void VisitExecute(PrioritizedChoice expression)
		{
			methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1}, ", _grammarName,
			                                           currentFunctionPointerLabel);
		}

		public override void VisitLeave(PrioritizedChoice expression)
		{
			methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1});", _grammarName,
			                                           currentFunctionPointerLabel);

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
			methods.Peek().methodData += String.Format("\treturn this->sequence(");
		}

		public override void VisitExecute(Sequence expression)
		{
			methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1}, ", _grammarName,
			                                           currentFunctionPointerLabel);
		}

		public override void VisitLeave(Sequence expression)
		{
			methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1});", _grammarName,
			                                           currentFunctionPointerLabel);

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
			methods.Peek().methodData += String.Format("\treturn this->andPredicate(");
		}

		public override void VisitExecute(AndPredicate expression)
		{
		}

		public override void VisitLeave(AndPredicate expression)
		{
			methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1});", _grammarName,
			                                           currentFunctionPointerLabel);

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
			methods.Peek().methodData += String.Format("\treturn this->notPredicate(");
		}

		public override void VisitExecute(NotPredicate expression)
		{
		}

		public override void VisitLeave(NotPredicate expression)
		{
			methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1});", _grammarName,
			                                           currentFunctionPointerLabel);

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
			methods.Peek().methodData += String.Format("\treturn this->zeroOrMore(");
		}

		public override void VisitExecute(ZeroOrMore expression)
		{
		}

		public override void VisitLeave(ZeroOrMore expression)
		{
			methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1});", _grammarName,
			                                           currentFunctionPointerLabel);

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
			methods.Peek().methodData += String.Format("\treturn this->oneOrMore(");
		}

		public override void VisitExecute(OneOrMore expression)
		{
		}

		public override void VisitLeave(OneOrMore expression)
		{
			methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1});", _grammarName,
			                                           currentFunctionPointerLabel);

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
			methods.Peek().methodData += String.Format("\treturn this->optional(");
		}

		public override void VisitExecute(Optional expression)
		{
		}

		public override void VisitLeave(Optional expression)
		{
			methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1});", _grammarName,
			                                           currentFunctionPointerLabel);

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
			methods.Peek().methodData += String.Format("\treturn this->capturingGroup(");
		}

		public override void VisitExecute(CapturingGroup expression)
		{
		}

		public override void VisitLeave(CapturingGroup expression)
		{
			String variableName = "_nodeName_0";

			methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1}, {2}, {3}, NULL);",
			                                           _grammarName, currentFunctionPointerLabel, variableName,
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
			methods.Peek().methodData += String.Format("\treturn this->{0}();", currentFunctionPointerLabel);
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
			methods.Peek().methodData += String.Format("\treturn this->limitingRepetition(");
		}

		public override void VisitExecute(LimitingRepetition expression)
		{
			methods.Peek().methodData += String.Format("(Npeg::IsMatchPredicate)&{0}::{1}, ", _grammarName,
			                                           currentFunctionPointerLabel);
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
						"\treturn this->literal({0}, {1}, {2});",
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
			var escapeSequence = new List<string>();
			escapeSequence.Add(@"\a"); // alert bell
			escapeSequence.Add(@"\b"); // backspace
			escapeSequence.Add(@"\f"); // formfeed 
			escapeSequence.Add(@"\n"); // newline
			escapeSequence.Add(@"\r"); // carriage return
			escapeSequence.Add(@"\t"); // horizontal tab
			escapeSequence.Add(@"\v"); // vertical tab
			escapeSequence.Add(@"\\"); // backslash
			escapeSequence.Add(@"\"""); // double quotes
			int foundEscapeSequence = escapeSequence.Where(s => expression.ClassExpression.Contains(s)).Count();
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object> {{variableName, expression.ClassExpression}},
					String.Format(
						"\treturn this->characterClass({0}, {1});",
						variableName, expression.ClassExpression.Length - foundEscapeSequence
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
					String.Format("\treturn this->anyCharacter();")
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
						"\treturn this->recursionCall((Npeg::IsMatchPredicate)&{0}::{1});",
						_grammarName, recursion[expression.FunctionName]
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
						"\treturn this->dynamicBackReference({0}, {1});",
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
						"\treturn this->warn({0});",
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
						"\treturn this->fatal({0});",
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