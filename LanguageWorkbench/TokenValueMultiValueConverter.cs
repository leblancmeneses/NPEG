using System;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using NPEG;
using SilverlightMarkupExtensions;

namespace LanguageWorkbench
{
	public class TokenValueMultiValueConverter : MarkupExtension, IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType,
			object parameter, System.Globalization.CultureInfo culture)

		{
			if (values.Length != 2 ||
				values[0] == null ||
				values[1] == null)
				return null;
 
			var node = values[0] as AstNode;
			var iterator = values[1] as IInputIterator;
			if (node == null || iterator == null) return string.Empty;

			try
			{

				if (node.Children.Any())
				{
					var leftStart = node.Token.Start;
					var leftEnd = node.Children.Select(x => x.Token.Start).Min() - 1;
					var left = iterator.Text(leftStart, leftEnd);

					var rightStart = node.Children.Select(x => x.Token.End).Max() + 1;
					var rightEnd = node.Token.End;
					if (rightStart > rightEnd)
					{
						return string.Format("{0}", Encoding.UTF8.GetString(left, 0, left.Length));
					}

					var right = iterator.Text(rightStart, rightEnd);
					return string.Format("{0}...{1}", Encoding.UTF8.GetString(left, 0, left.Length),
					                     Encoding.UTF8.GetString(right, 0, right.Length));
				}

				var result = iterator.Text(node.Token.Start, node.Token.End);
				return Encoding.UTF8.GetString(result, 0, result.Length);
			}
			catch
			{
				return string.Empty;
			}
		}



		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}


		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}
