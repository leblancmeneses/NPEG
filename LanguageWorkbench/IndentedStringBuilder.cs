using System;
using System.Text;

namespace LanguageWorkbench
{
	public class IndentedStringBuilder : IDisposable
	{
		private StringBuilder sb;
		private string indentationString = "\t";
		private string completeIndentationString = "";
		private int indent = 0;

		/// <summary>
		///  Creates an IndentedStringBuilder
		/// </summary>
		public IndentedStringBuilder()
		{
			sb = new StringBuilder();
		}

		/// <summary>
		/// Appends a string
		/// </summary>
		/// <param name="value"></param>
		public void Append(string value)
		{
			sb.Append(completeIndentationString + value);
		}

		/// <summary>
		/// Appends a line
		/// </summary>
		/// <param name="value"></param>
		public void AppendLine(string value)
		{
			Append(value + Environment.NewLine);
		}

		/// <summary>
		/// The string/chars to use for indentation, \t by default
		/// </summary>
		public string IndentationString
		{
			get { return indentationString; }
			set
			{
				indentationString = value;

				updateCompleteIndentationString();
			}
		}

		/// <summary>
		/// Creates the actual indentation string
		/// </summary>
		private void updateCompleteIndentationString()
		{
			completeIndentationString = "";

			for (int i = 0; i < indent; i++)
				completeIndentationString += indentationString;
		}

		/// <summary>
		/// Increases indentation, returns a reference to an IndentedStringBuilder instance which is only to be used for disposal
		/// </summary>
		/// <returns></returns>
		public IndentedStringBuilder IncreaseIndent()
		{
			indent++;

			updateCompleteIndentationString();

			return this;
		}

		/// <summary>
		/// Decreases indentation, may only be called if indentation > 1
		/// </summary>
		public void DecreaseIndent()
		{
			if (indent > 0)
			{
				indent--;

				updateCompleteIndentationString();
			}
		}

		/// <summary>
		/// Decreases indentation
		/// </summary>
		public void Dispose()
		{
			DecreaseIndent();
		}

		/// <summary>
		/// Returns the text of the internal StringBuilder
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return sb.ToString();
		}
	}
}
