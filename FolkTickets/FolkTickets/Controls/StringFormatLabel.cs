using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace FolkTickets.Controls
{
    /// <summary>
    /// Label with string.Format support
    /// </summary>
    public class StringFormatLabel : Label
    {
        /// <summary>
        /// Format pattern
        /// </summary>
        public string StringFormat
        {
            get
            {
                return (string)GetValue(StringFormatProperty);
            }
            set
            {
                SetValue(StringFormatProperty, value);
            }
        }

        /// <summary>
        /// Format values
        /// </summary>
        public ObservableCollection<TextValue> FormatValues
        {
            get
            {
                return (ObservableCollection<TextValue>)GetValue(FormatValuesProperty);
            }
            set
            {
                SetValue(FormatValuesProperty, value);
            }
        }

        /// <summary>
        /// Format values property
        /// </summary>
        public BindableProperty FormatValuesProperty = BindableProperty.Create(
                nameof(FormatValues),
                typeof(ObservableCollection<TextValue>),
                typeof(StringFormatLabel),
                new ObservableCollection<TextValue>());

        /// <summary>
        /// String format property
        /// </summary>
        public BindableProperty StringFormatProperty = BindableProperty.Create(
                nameof(StringFormat),
                typeof(string),
                typeof(StringFormatLabel));

        /// <summary>
        /// Format label text
        /// </summary>
        /// <returns></returns>
        private string FormatText()
        {
            if(string.IsNullOrWhiteSpace(StringFormat))
            {
                return string.Empty;
            }

            if(FormatValues == null)
            {
                return StringFormat;
            }

            return string.Format(StringFormat, FormatValues.Select(v => v.Text).ToArray());
        }

        /// <summary>
        /// Update format values binding
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            foreach(var formatValue in FormatValues)
            {
                formatValue.BindingContext = BindingContext;
            }
            Text = FormatText();
        }

        /// <inheritdoc/>
        public StringFormatLabel() : base()
        {
            Text = FormatText();
        }
    }
}
