using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NPEG.NonTerminals;
using NPEG.Terminals;

namespace NPEG.Algorithms.LanguageWriters
{
#warning java and csharp versions have an interface pointing to create custom astnode factory.  We need a flag in base writer to signal EnableCustomAstNodeFactory
	public class CSharpWriter : WriterBase
	{
		private readonly String _folderPath;
		private readonly String _grammarName;
		private readonly String _nameSpace;

		private readonly Stack<MethodInfo> finalMethods = new Stack<MethodInfo>();
		private readonly Stack<MethodInfo> methods = new Stack<MethodInfo>();
		private Int32 _uniqueFunctionID;
		private String currentFunctionPointerLabel = String.Empty;


		public CSharpWriter(String grammarName, String nameSpace, String folderPath)
		{
			_grammarName = grammarName;
			_folderPath = folderPath;
			_nameSpace = nameSpace;
		}


		public override void Write()
		{
			DirectoryInfo info = Directory.CreateDirectory(_folderPath);


			String startMethodName = _grammarName + @"_impl_0";
			//this._grammarName

			using (StreamWriter writer = File.CreateText(Path.Combine(info.FullName, String.Format("{0}.cs", _grammarName))))
			{
				var src = new StringBuilder();
				src.Append("using System;\n");
				src.Append("using System.Text;\n");
				src.Append("using RobustHaven.Text.Npeg;\n\n");

				src.AppendFormat("namespace {0}\n", _nameSpace);
				src.Append("{\n");

				src.AppendFormat("\tpublic class {0} : NpegParser\n", _grammarName);
				src.Append("\t{\n");

				src.AppendLine(String.Format("\t\tpublic {0}(InputIterator iterator): base(iterator){{}}\n", _grammarName));

				src.Append("\t\tpublic override Boolean IsMatch()\n");
				src.Append("\t\t{\n");
				src.Append("\t\t\treturn " + _grammarName + @"_impl_0" + "();\n");
				src.Append("\t\t}\n");

				src.Append("\n");

				while (finalMethods.Count > 0)
				{
					src.Append("\n");
					MethodInfo m = finalMethods.Pop();
					String[] lines = Regex.Split(m.methodData, @"(.*)$", RegexOptions.Multiline);
					foreach (string line in lines)
					{
						if (!String.IsNullOrEmpty(line.Trim()))
							src.AppendLine("\t\t" + line);
					}
				}

				src.Append("\t}\n"); // class
				src.Append("}\n"); // namespace

				writer.Write(src.ToString());
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
					locals.AppendFormat("\tString {0} = \"{1}\";\n", key, localVariables[key].ToString().Replace("\"", @"\"""));
				}
			}

			return "protected Boolean " + methodName + "()\n" +
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
			methods.Peek().methodData += String.Format("\treturn this.PrioritizedChoice(");
		}

		public override void VisitExecute(PrioritizedChoice expression)
		{
			methods.Peek().methodData += String.Format("new IsMatchPredicate(this.{1}), ", _grammarName,
			                                           currentFunctionPointerLabel);
		}

		public override void VisitLeave(PrioritizedChoice expression)
		{
			methods.Peek().methodData += String.Format("new IsMatchPredicate(this.{1}));", _grammarName,
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
			methods.Peek().methodData += String.Format("\treturn this.Sequence(");
		}

		public override void VisitExecute(Sequence expression)
		{
			methods.Peek().methodData += String.Format("new IsMatchPredicate(this.{1}), ", _grammarName,
			                                           currentFunctionPointerLabel);
		}

		public override void VisitLeave(Sequence expression)
		{
			methods.Peek().methodData += String.Format("new IsMatchPredicate(this.{1}));", _grammarName,
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
			methods.Peek().methodData += String.Format("\treturn this.AndPredicate(");
		}

		public override void VisitExecute(AndPredicate expression)
		{
		}

		public override void VisitLeave(AndPredicate expression)
		{
			methods.Peek().methodData += String.Format("new IsMatchPredicate(this.{1}));", _grammarName,
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
			methods.Peek().methodData += String.Format("\treturn this.NotPredicate(");
		}

		public override void VisitExecute(NotPredicate expression)
		{
		}

		public override void VisitLeave(NotPredicate expression)
		{
			methods.Peek().methodData += String.Format("new IsMatchPredicate(this.{1}));", _grammarName,
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
			methods.Peek().methodData += String.Format("\treturn this.ZeroOrMore(");
		}

		public override void VisitExecute(ZeroOrMore expression)
		{
		}

		public override void VisitLeave(ZeroOrMore expression)
		{
#warning need to revert peg composite to string grammar rules ... so I can send second argument.
			methods.Peek().methodData += String.Format("new IsMatchPredicate(this.{1}), \"\");", _grammarName,
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
			methods.Peek().methodData += String.Format("\treturn this.OneOrMore(");
		}

		public override void VisitExecute(OneOrMore expression)
		{
		}

		public override void VisitLeave(OneOrMore expression)
		{
			methods.Peek().methodData += String.Format("new IsMatchPredicate(this.{1}), \"\");", _grammarName,
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
			methods.Peek().methodData += String.Format("\treturn this.Optional(");
		}

		public override void VisitExecute(Optional expression)
		{
		}

		public override void VisitLeave(Optional expression)
		{
			methods.Peek().methodData += String.Format("new IsMatchPredicate(this.{1}));", _grammarName,
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
			methods.Peek().methodData += String.Format("\treturn this.CapturingGroup(");
		}

		public override void VisitExecute(CapturingGroup expression)
		{
		}

		public override void VisitLeave(CapturingGroup expression)
		{
			String variableName = "_nodeName_0";

			methods.Peek().methodData += String.Format("new IsMatchPredicate(this.{1}), {2}, {3}, false);",
			                                           _grammarName, currentFunctionPointerLabel, variableName,
			                                           expression.DoReplaceBySingleChildNode ? "true" : "false");

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
			methods.Peek().methodData += String.Format("\treturn this.{0}();", currentFunctionPointerLabel);
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
			methods.Peek().methodData += String.Format("\treturn this.LimitingRepetition(");
		}

		public override void VisitExecute(LimitingRepetition expression)
		{
		}

		public override void VisitLeave(LimitingRepetition expression)
		{
			methods.Peek().methodData += String.Format("new IsMatchPredicate(this.{0}), ", currentFunctionPointerLabel);
			methods.Peek().methodData += String.Format("{0}, {1}, \"\");\n",
			                                           (expression.Min == null) ? "null" : expression.Min.ToString(),
			                                           (expression.Max == null) ? "null" : expression.Max.ToString());

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
						"\treturn this.Literal({0}, {1});",
						variableName,
						expression.IsCaseSensitive ? "true" : "false"
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
						"\treturn this.CharacterClass({0}, {1});",
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
					String.Format("\treturn this.AnyCharacter();")
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
						"\treturn this.RecursionCall(new IsMatchPredicate(this.{1}));",
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
						"\treturn this.DynamicBackReference({0}, {1});",
						variableName,
						expression.IsCaseSensitive ? "true" : "false"
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
						"\treturn this.Warn({0});",
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
						"\treturn this.Fatal({0});",
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