using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LanguageWorkbench.Converters
{
	public class RemoveNameSpaceConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;

			string data = value.ToString();
			if (String.IsNullOrEmpty(data))
			{
				return value;
			}
			string[] split = data.Split('.');
			return split.Count() > 1 ? split[split.Count() - 1] : data;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}