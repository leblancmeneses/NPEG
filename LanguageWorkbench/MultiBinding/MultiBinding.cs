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
using System.Windows.Markup;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using System.Xaml;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Reflection;

namespace SilverlightMarkupExtensions
{
    
   
    public class BindingCollection : Collection<Object>
    {
        public bool IsSealed { get; private set; }

        public void Seal()
        {
            IsSealed = true;
        }

        public void CheckSealed()
        {
            if (IsSealed) throw new InvalidOperationException("This BindingCollection cannot be changed when it has been sealed.");
        }

        protected override void InsertItem(int index, Object item)
        {
            CheckSealed();
            base.InsertItem(index, item);
        }

        protected override void ClearItems()
        {
            CheckSealed();
            base.ClearItems();
        }

        protected override void SetItem(int index, object item)
        {
            CheckSealed();
            base.SetItem(index, item);
        }
    }

    [ContentProperty("Bindings")]
    public class MultiBinding :  DependencyObject, IMarkupExtension<Object>, ISupportInitialize
    {       
        
        
        protected bool IsSealed { get; private set; }

        public void Seal()
        {
            IsSealed = true;
            if( Bindings != null ) Bindings.Seal();
        }

        public void CheckSealed()
        {
            if (IsSealed) throw new InvalidOperationException("Properties on MultiBinding cannot be changed after it has been applied.");
        }

        private BindingCollection  m_bindings;

        public BindingCollection  Bindings
        {
            get { return m_bindings; }
            set { CheckSealed();  m_bindings = value; }
        }

        private BindingMode m_bindingMode = BindingMode.OneWay;

        /// <summary>
        /// Gets or sets the binding mode.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        public BindingMode Mode
        {
            get { return m_bindingMode; }
            set { CheckSealed(); m_bindingMode = value; }
        }


        private object m_targetNullValue;

        /// <summary>
        /// Gets or sets the value to use when the converted value is null.
        /// </summary>
        /// <value>
        /// The target null value.
        /// </value>
        public object TargetNullValue
        {
            get { return m_targetNullValue; }
            set { CheckSealed(); m_targetNullValue = value; }
        }

        private CultureInfo m_converterCulture;

        /// <summary>
        /// Gets or sets the converter culture.
        /// </summary>
        /// <value>
        /// The converter culture.
        /// </value>
        public CultureInfo ConverterCulture
        {
            get { return m_converterCulture; }
            set { CheckSealed(); m_converterCulture = value; }
        }

        private bool m_validatesOnExceptions;

        /// <summary>
        /// Gets or sets a value indicating whether exceptions thrown during conversion is to cause validation errors or not.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [validates on exceptions]; otherwise, <c>false</c>.
        /// </value>
        public bool ValidatesOnExceptions
        {
            get { return m_validatesOnExceptions; }
            set { CheckSealed(); m_validatesOnExceptions = value; }
        }

        private bool m_notifyOnValidationError;

        /// <summary>
        /// Gets or sets a value indicating whether an event should be raised on validation errors.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [notify on validation error]; otherwise, <c>false</c>.
        /// </value>
        public bool NotifyOnValidationError
        {
            get { return m_notifyOnValidationError; }
            set { CheckSealed();  m_notifyOnValidationError = value; }
        }

        private object m_fallbackValue;

        /// <summary>
        /// Gets or sets the fallback value,i .e. the value to use when no value is available.
        /// </summary>
        /// <value>
        /// The fallback value to use when the Converter returns a DependencyProperty.UnsetValue.
        /// </value>
        public object FallbackValue
        {
            get { return m_fallbackValue; }
            set { CheckSealed(); m_fallbackValue = value; }
        }
        


        private UpdateSourceTrigger m_updateSourceTrigger = UpdateSourceTrigger.Default;

        /// <summary>
        /// Gets or sets the update source trigger determing when the sources are updated in a two-way binding. 
        /// </summary>
        /// <value>
        /// The update source trigger.
        /// </value>
        public UpdateSourceTrigger UpdateSourceTrigger
        {
            get { return m_updateSourceTrigger; }
            set { CheckSealed(); m_updateSourceTrigger = value; }
        }
        

        
        /// <summary>
        /// Maximum number of multibindings that can be used in a single Style. As the styleMultiBindingProperties will be
        /// used in a round-robbin fashion using more multibindings in a a single Style will result in the first bindings
        /// will be overwritten by later bindings.
        /// </summary>
        private const int MaxStyleMultiBindingsCount = 8;

        
        static MultiBinding()
        {
            
            // Create Style Multi Binding attached properties
            styleMultBindingProperties = new DependencyProperty[MaxStyleMultiBindingsCount];
            for (int i = 0; i < MaxStyleMultiBindingsCount; i++)
            {
                styleMultBindingProperties[i] = DependencyProperty.RegisterAttached("StyleMultiBinding"+i, typeof(MultiBinding), typeof(MultiBinding), new PropertyMetadata(null, OnStyleMultiBindingChanged));
   
            }
            
        }

        private static readonly DependencyProperty[] styleMultBindingProperties;

        private static int currentStyleMultiBindingIndex = 0;

        private static void OnStyleMultiBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            MultiBinding mb = (MultiBinding)args.NewValue;
            if (mb != null)
            {
                // Only apply multibinding from Style if no local value has been set.
                object existingValue = d.ReadLocalValue(mb.m_styleProperty);
                if (existingValue == DependencyProperty.UnsetValue)
                {
                    // Apply binding to target by creating MultiBindingExpression and attached data properties.
                    Binding resultBinding = mb.ApplyToTarget(d);
                    BindingOperations.SetBinding(d, mb.m_styleProperty, resultBinding);
                    
                }
            }
        }

        /// <summary>
        /// Gets or sets the converter to use be used to convert between source values and target value (and vice versa).
        /// </summary>
        /// <value>
        /// The converter.
        /// </value>
        /// <remarks>
        /// The converter must either implement the <see cref="IMultiValueConverter"/> interface or the IValueConverter interface.
        /// 
        /// This property is bindable, i.e. you may specify it as a Binding relative to the MultiBinding target element.
        /// </remarks>
        public Object Converter
        {
            get { return GetValue(ConverterProperty); }
            set { SetValue(ConverterProperty, value); }
        }

        /// <summary>
        /// Represents Converter dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register("Converter", typeof(Object), typeof(MultiBinding), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// Gets or sets the parameter to pass in to the Converter.
        /// </summary>
        /// <value>
        /// The converter parameter.
        /// </value>
        /// <remarks>
        /// This property is bindable, i.e. you may specify it as a Bindingin relative to the MultiBinding target element.
        /// </remarks>
        public object ConverterParameter
        {
            get { return (object)GetValue(ConverterParameterProperty); }
            set { SetValue(ConverterParameterProperty, value); }
        }

        /// <summary>
        /// Represents the ConverterParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterParameterProperty =
            DependencyProperty.Register("ConverterParameter", typeof(object), typeof(MultiBinding), new PropertyMetadata(null, OnDependencyPropertyChanged));



        /// <summary>
        /// Gets or sets a formatting string to be applied to result of the conversion. This property is bindable.
        /// </summary>
        /// <remarks>
        /// The String format follows the convention for String.Format formatting. Optionally, individual source values can be 
        /// referred to with %n-syntax where n is the zero-based index of the source value.
        /// 
        /// In case a Converter is specified the StringFormat is applied to result of the conversion.
        /// </remarks>
        public String StringFormat
        {
            get { return (String)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        /// <summary>
        /// Represents the StringFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register("StringFormat", typeof(String), typeof(MultiBinding), new PropertyMetadata(null, OnDependencyPropertyChanged));



        /// <summary>
        /// Gets or sets the first source.
        /// </summary>
        /// <value>
        /// The first source.
        /// </value>
        public object Source1
        {
            get { return (object)GetValue(Source1Property); }
            set { SetValue(Source1Property, value); }
        }

        /// <summary>
        /// Represents the Source1 dependency property.
        /// </summary>
        public static readonly DependencyProperty Source1Property =
            DependencyProperty.Register("Source1", typeof(object), typeof(MultiBinding), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// Gets or sets the second source.
        /// </summary>
        /// <value>
        /// The second source.
        /// </value>
        public object Source2
        {
            get { return (object)GetValue(Source2Property); }
            set { SetValue(Source2Property, value); }
        }

        /// <summary>
        /// Represents the Source2 dependency property.
        /// </summary>
        public static readonly DependencyProperty Source2Property =
            DependencyProperty.Register("Source2", typeof(object), typeof(MultiBinding), new PropertyMetadata(null, OnDependencyPropertyChanged));


        /// <summary>
        /// Gets or sets the third source.
        /// </summary>
        /// <value>
        /// The third source.
        /// </value>
        public object Source3
        {
            get { return (object)GetValue(Source3Property); }
            set { SetValue(Source3Property, value); }
        }

        /// <summary>
        /// Represents the Source3 property.
        /// </summary>
        public static readonly DependencyProperty Source3Property =
            DependencyProperty.Register("Source3", typeof(object), typeof(MultiBinding), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// Gets or sets the forth source.
        /// </summary>
        /// <value>
        /// The forth source.
        /// </value>
        public object Source4
        {
            get { return (object)GetValue(Source4Property); }
            set { SetValue(Source4Property, value); }
        }

        /// <summary>
        /// Represents the Source4 property.
        /// </summary>
        public static readonly DependencyProperty Source4Property =
            DependencyProperty.Register("Source4", typeof(object), typeof(MultiBinding), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// Gets or sets the fifth source.
        /// </summary>
        /// <value>
        /// The fifth source.
        /// </value>
        public object Source5
        {
            get { return (object)GetValue(Source5Property); }
            set { SetValue(Source5Property, value); }
        }

        /// <summary>
        /// Represents the Source5 dependency property.
        /// </summary>
        public static readonly DependencyProperty Source5Property =
            DependencyProperty.Register("Source5", typeof(object), typeof(MultiBinding), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// Array of all source dependency properties.
        /// </summary>
        public static readonly DependencyProperty[] SourceProperties = 
            new DependencyProperty[] { Source1Property, Source2Property, Source3Property, Source4Property, Source5Property };

        private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            MultiBinding mb = (MultiBinding)d;
            mb.CheckSealed();
        }


        /// <summary>
        /// The dependency property this MultiBinding is to be applied to when using in a Style setter.
        /// </summary>
        private DependencyProperty m_styleProperty;

        private static readonly PropertyInfo SetterValueProperty = typeof(Setter).GetProperty("Value");

        public object ProvideValue(IServiceProvider serviceProvider)
        {
           
            IProvideValueTarget pvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (pvt == null)
            {
                throw new InvalidOperationException("MultiBinding cannot determine the binding target object since the IProviderValueTarget service is unavailable.");
            }
            if (pvt.TargetObject is Setter && pvt.TargetProperty == SetterValueProperty)
            {
                Setter setter = (Setter)pvt.TargetObject;
                ApplyToStyle(setter);
                return this;
            }
            DependencyObject target = pvt.TargetObject as DependencyObject;
            if (target == null)
            {
                throw new InvalidOperationException("MultiBinding can only be applied to DependencyObjects.");
            }
            Binding resultBinding = ApplyToTarget(target);
            
            return resultBinding.ProvideValue(serviceProvider);
        
            
        }

        private void ApplyToStyle(Setter setter)
        {
            m_styleProperty = setter.Property;
            if (m_styleProperty == null)
            {
                throw new InvalidOperationException("When a MultiBinding is applied to a Style setter the Property must be set first.");
            }
            setter.Property = styleMultBindingProperties[currentStyleMultiBindingIndex];
            currentStyleMultiBindingIndex = (currentStyleMultiBindingIndex + 1) % MaxStyleMultiBindingsCount;
            Seal();
        }

        /// <summary>
        /// Applies this MultiBinding to a target object.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="targetType">The Type of the target dependency property.</param>
        /// <returns>A Binding specific for this target</returns>
        public Binding ApplyToTarget(DependencyObject target)
        {
            Seal();
            
            // Create new MultiBindingInfo to hold information about this multibinding
            MultiBindingExpression newInfo = new MultiBindingExpression(target, this);
            
            // Create new binding to expressions's SourceValues property
            PropertyPath path = new PropertyPath("SourceValues");
            Binding resultBinding = new Binding();
            resultBinding.Converter = newInfo;
            resultBinding.Path = path;
            resultBinding.Source = newInfo;
            resultBinding.Mode = Mode;
            resultBinding.UpdateSourceTrigger = UpdateSourceTrigger;
            resultBinding.TargetNullValue = TargetNullValue;
            resultBinding.ValidatesOnExceptions = ValidatesOnExceptions;
            resultBinding.ValidatesOnNotifyDataErrors = true;
            resultBinding.NotifyOnValidationError = NotifyOnValidationError;
            resultBinding.ConverterCulture = ConverterCulture;
            resultBinding.FallbackValue = FallbackValue;
            return resultBinding;
        }

        

        void ISupportInitialize.BeginInit()
        {
            // Nothing to do
        }

        void ISupportInitialize.EndInit()
        {
            bool isBindingsSet = Bindings != null && Bindings.Count > 0;
            bool unsetSourcePropertyFound = false; 
            foreach (DependencyProperty sourceProperty in SourceProperties)
            {
                object sourceValue = ReadLocalValue(sourceProperty);
                if (sourceValue != DependencyProperty.UnsetValue)
                {
                    if (isBindingsSet) throw new InvalidOperationException("Source properties and Bindings cannot be used at the same time.");
                    if (unsetSourcePropertyFound) throw new InvalidOperationException("Source1, Source2, etc. properties must be used in order.");
                }
                else
                {
                    unsetSourcePropertyFound = true;
                }
            }
        }

        /*
         Tried to implement IList to allow direct XAML content but this failed in VS Designer when using Silverlight 5 RC. 

        int IList.Add(object value)
        {
            CheckSealed(); 
            Bindings.Add(value);
            return Bindings.Count - 1;
        }

        void IList.Clear()
        {
            CheckSealed();
            Bindings.Clear();
        }

        bool IList.Contains(object value)
        {
            return Bindings.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return Bindings.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            CheckSealed();
            Bindings.Insert(index, value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return IsSealed; }
        }

        void IList.Remove(object value)
        {
            CheckSealed();
            Bindings.Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            CheckSealed();
            Bindings.RemoveAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                return Bindings[index];
            }
            set
            {
                CheckSealed();
                Bindings[index] = value;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)Bindings).CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return Bindings.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return Bindings; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Bindings.GetEnumerator();
        }  
         
         */
    }
}
