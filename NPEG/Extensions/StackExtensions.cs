using System.Collections.Generic;

namespace NPEG.Extensions
{
	public static class StackExtensions
	{
		public static Stack<T> Reverse<T>(this Stack<T> original)
		{
			var newStack = new Stack<T>();
			while (original.Count != 0)
			{
				newStack.Push(original.Pop());
			}

			return newStack;
		}
	}
}
