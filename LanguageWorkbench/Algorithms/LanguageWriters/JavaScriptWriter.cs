using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NPEG.NonTerminals;
using NPEG.Terminals;
using Telerik.Windows.Zip;

namespace LanguageWorkbench.Algorithms.LanguageWriters
{
	public class JavaScriptWriter : WriterBase
	{
		private readonly string _grammarName;
		private readonly string _grammarInput;
		private readonly ZipPackage _zipPackage;


		private readonly Stack<MethodInfo> finalMethods = new Stack<MethodInfo>();
		private readonly Stack<MethodInfo> methods = new Stack<MethodInfo>();
		private String _nameSpace = "LanguageWorkbench.Parsers";
		private Int32 _uniqueFunctionID;
		private String currentFunctionPointerLabel = String.Empty;

		public JavaScriptWriter(String grammarName, String grammarInput, ZipPackage zipPackage)
		{
			_grammarName = grammarName;
			_grammarInput = grammarInput;
			_zipPackage = zipPackage;
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
					locals.AppendFormat("\tvar {0} = \"{1}\";\n", key, localVariables[key].ToString().Replace("\"", @"\"""));
				}
			}

			return "var " + methodName + " = new Expression();\n" +
			       methodName + ".npeg = this;\n" +
			       methodName + ".evaluate = function() \n" +
			       "{\n" +
			       ((locals.ToString().Length > 0) ? locals + "\n" : "")
			       +
			       methodText
			       +
			       "\n}";
		}

		public override void Write()
		{
			var src = new StringBuilder();
			src.AppendLine("if(!LanguageWorkbench) var LanguageWorkbench={};");
			src.AppendLine("if(!LanguageWorkbench.Parsers) LanguageWorkbench.Parsers={};");
			src.AppendLine("");

			src.AppendLine("");
			src.AppendLine("");

			src.AppendFormat("/**************\n{0}\n**************/", _grammarInput);

			src.AppendLine("");
			src.AppendLine("");

			src.AppendFormat("{0}.{1} = function(iterator)\n", _nameSpace, _grammarName);
			src.Append("{\n");
			src.AppendFormat("\t{0}.{1}.superclass.constructor.call(this, iterator);\n", _nameSpace, _grammarName);


			src.Append("\tthis.isMatch = function()\n");
			src.Append("\t{\n");
			src.Append("\t\treturn " + _grammarName + @"_impl_0" + ".evaluate();\n");
			src.Append("\t}\n");

			src.Append("\n");

			while (finalMethods.Count > 0)
			{
				src.Append("\n");
				MethodInfo m = finalMethods.Pop();
				String[] lines = Regex.Split(m.methodData, @"(.*)$", RegexOptions.Multiline);
				foreach (string line in lines)
				{
					if (!String.IsNullOrEmpty(line.Trim()))
						src.AppendLine("\t" + line);
				}
			}

			src.Append("}\n");
			src.AppendFormat("RobustHaven.Text.Npeg.extend({0}.{1}, RobustHaven.Text.Npeg.Npeg);\n", _nameSpace, _grammarName);


			byte[] bytes = Encoding.UTF8.GetBytes(src.ToString());
			var memoryStream = new MemoryStream();
			memoryStream.Write(bytes, 0, bytes.Length);
			memoryStream.Seek(0, SeekOrigin.Begin);
			_zipPackage.AddStream(memoryStream, String.Format("{0}.js", _grammarName));
		}


		#region non terminals

		private readonly Dictionary<String, String> recursion = new Dictionary<string, string>();

		public override void VisitEnter(PrioritizedChoice expression)
		{
			methods.Push(new MethodInfo { methodName = CreateMethodName() });
			methods.Peek().methodData += String.Format("\treturn this.npeg.prioritizedChoice(");
		}

		public override void VisitExecute(PrioritizedChoice expression)
		{
			methods.Peek().methodData += String.Format("{0}, ", currentFunctionPointerLabel);
		}

		public override void VisitLeave(PrioritizedChoice expression)
		{
			methods.Peek().methodData += String.Format("{0});", currentFunctionPointerLabel);

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
			methods.Push(new MethodInfo { methodName = CreateMethodName() });
			methods.Peek().methodData += String.Format("\treturn this.npeg.sequence(");
		}

		public override void VisitExecute(Sequence expression)
		{
			methods.Peek().methodData += String.Format("{0}, ", currentFunctionPointerLabel);
		}

		public override void VisitLeave(Sequence expression)
		{
			methods.Peek().methodData += String.Format("{0});", currentFunctionPointerLabel);

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
			methods.Push(new MethodInfo { methodName = CreateMethodName() });
			methods.Peek().methodData += String.Format("\treturn this.npeg.andPredicate(");
		}

		public override void VisitExecute(AndPredicate expression)
		{
		}

		public override void VisitLeave(AndPredicate expression)
		{
			methods.Peek().methodData += String.Format("{0});", currentFunctionPointerLabel);
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
			methods.Push(new MethodInfo { methodName = CreateMethodName() });
			methods.Peek().methodData += String.Format("\treturn this.npeg.notPredicate(");
		}

		public override void VisitExecute(NotPredicate expression)
		{
		}

		public override void VisitLeave(NotPredicate expression)
		{
			methods.Peek().methodData += String.Format("{0});", currentFunctionPointerLabel);

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
			methods.Push(new MethodInfo { methodName = CreateMethodName() });
			methods.Peek().methodData += String.Format("\treturn this.npeg.zeroOrMore(");
		}

		public override void VisitExecute(ZeroOrMore expression)
		{
		}

		public override void VisitLeave(ZeroOrMore expression)
		{
#warning need to revert peg composite to string grammar rules ... so I can send second argument.
			methods.Peek().methodData += String.Format("{0}, \"\");", currentFunctionPointerLabel);

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
			methods.Push(new MethodInfo { methodName = CreateMethodName() });
			methods.Peek().methodData += String.Format("\treturn this.npeg.oneOrMore(");
		}

		public override void VisitExecute(OneOrMore expression)
		{
		}

		public override void VisitLeave(OneOrMore expression)
		{
			methods.Peek().methodData += String.Format("{0}), \"\");", currentFunctionPointerLabel);

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
			methods.Push(new MethodInfo { methodName = CreateMethodName() });
			methods.Peek().methodData += String.Format("\treturn this.npeg.optional(");
		}

		public override void VisitExecute(Optional expression)
		{
		}

		public override void VisitLeave(Optional expression)
		{
			methods.Peek().methodData += String.Format("{0});", currentFunctionPointerLabel);

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
			methods.Push(new MethodInfo { methodName = CreateMethodName() });
			methods.Peek().methodData += String.Format("\treturn this.npeg.capturingGroup(");
		}

		public override void VisitExecute(CapturingGroup expression)
		{
		}

		public override void VisitLeave(CapturingGroup expression)
		{
			String variableName = "_nodeName_0";

			methods.Peek().methodData += String.Format("{0}, {1}, {2}, false);",
													   currentFunctionPointerLabel, variableName,
													   expression.DoReplaceBySingleChildNode ? "true" : "false");

			currentFunctionPointerLabel = methods.Peek().methodName;

			MethodInfo m = methods.Pop();

			finalMethods.Push(m);
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object> { { variableName, expression.Name } },
					finalMethods.Peek().methodData
					);
		}


		public override void VisitEnter(RecursionCreate expression)
		{
			String _methodName = CreateMethodName();
			methods.Push(new MethodInfo { methodName = _methodName });
			recursion.Add(expression.FunctionName, _methodName);
		}

		public override void VisitExecute(RecursionCreate expression)
		{
		}

		public override void VisitLeave(RecursionCreate expression)
		{
			methods.Peek().methodData += String.Format("\treturn {0}();", currentFunctionPointerLabel);
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
			methods.Push(new MethodInfo { methodName = CreateMethodName() });
			methods.Peek().methodData += String.Format("\treturn this.npeg.limitingRepetition(");
		}

		public override void VisitExecute(LimitingRepetition expression)
		{
		}

		public override void VisitLeave(LimitingRepetition expression)
		{
			methods.Peek().methodData += String.Format("{0}, ", currentFunctionPointerLabel);
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
						"\treturn this.npeg.Literal({0}, {1});",
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
						"\treturn this.npeg.characterClass({0}, {1});",
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
					String.Format("\treturn this.npeg.anyCharacter();")
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
						"\treturn this.npeg.recursionCall({0}));", recursion[expression.FunctionName]
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
						"\treturn this.npeg.dynamicBackReference({0}, {1});",
						variableName,
						expression.IsCaseSensitive ? "true" : "false"
						)
					);
			currentFunctionPointerLabel = finalMethods.Peek().methodName;
		}

		public override void Visit(CodePoint expression)
		{
			String variableName = "_codepoint_0";
			finalMethods.Push(new MethodInfo {methodName = CreateMethodName()});
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object> {{variableName, expression.Match}},
					String.Format(
						"\treturn this.npeg.codePoint({0});",
						variableName
						)
					);
			currentFunctionPointerLabel = finalMethods.Peek().methodName;
		}

		public override void Visit(Warn expression)
		{
			String variableName = "_warn_0";
			finalMethods.Push(new MethodInfo {methodName = CreateMethodName()});
			finalMethods.Peek().methodData =
				CreateMethod(
					finalMethods.Peek().methodName,
					new Dictionary<string, object> {{variableName, expression.Message}},
					String.Format(
						"\treturn this.npeg.warn({0});",
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
						"\treturn this.npeg.fatal({0});",
						variableName
						)
					);
			currentFunctionPointerLabel = finalMethods.Peek().methodName;
		}

		#endregion
	}
}