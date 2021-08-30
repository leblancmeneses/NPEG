/* 
 * 
 * Copyright Henrik Jonsson 2011
 * 
 * This code is licenced under the The Code Project Open Licence 1.02 (see Licence.htm and http://www.codeproject.com/info/cpol10.aspx). 
 *
 */

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Globalization;

namespace SilverlightMarkupExtensions
{
    /// <summary>
    /// Interface to implement for <see cref="MultiBinding"/> converters.
    /// </summary>
    public interface IMultiValueConverter
    {

        /// <summary>
        /// Converts the specified source values to a single target value.
        /// </summary>
        /// <param name="values">The source values</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The converter parameter.</param>
        /// <param name="culture">The culture to be used when doing culture-sensitive conversion.</param>
        /// <returns></returns>
        Object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);


        /// <summary>
        /// Converts the specified target value back to source values in two-way bindings.
        /// </summary>
        /// <param name="value">The target value to be converted.</param>
        /// <param name="targetTypes">Indicates the types of the source values to be set.</param>
        /// <param name="parameter">The parameter passed to the MultiBinding.</param>
        /// <param name="culture">The culture to be used when doing culture-sensitive conversion.</param>
        /// <returns>The values of individual source values.</returns>
        Object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);
    }
}
