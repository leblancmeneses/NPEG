using System;
using System.Collections.Generic;

namespace NPEG.Algorithms
{
	public class AstWriterDefaultData
	{
		public Int32 Indent;
		public bool IsIgnoring;
		public List<string> TopLevelIgnoredCommands { get; private set; }

		public AstWriterDefaultData()
		{
			TopLevelIgnoredCommands = new List<string> { };
		}
	}

	public class AstWriter<T> : IAstNodeReplacement
	{
		private readonly Action<T, AstNode> _onEnter;
		private readonly Action<T, AstNode> _onLeave;

		private T Data { get; set; }

		public AstWriter(T data, Action<T, AstNode> onEnter, Action<T, AstNode> onLeave)
		{
			_onEnter = onEnter;
			_onLeave = onLeave;
			Data = data;
		}

		public override void VisitEnter(AstNode node)
		{
			_onEnter(Data, node);
		}

		public override void VisitLeave(AstNode node)
		{
			_onLeave(Data, node);
		}
	}
}
