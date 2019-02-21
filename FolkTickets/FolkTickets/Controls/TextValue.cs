using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace FolkTickets.Controls
{
    /// <summary>
    /// Text value
    /// </summary>
    public class TextValue : BindableObject
    {
        /// <summary>
        /// Text
        /// </summary>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Text property
        /// </summary>
        public static BindableProperty TextProperty = BindableProperty.Create(
               nameof(Text),
               typeof(string),
               typeof(TextValue));

        /// <inheritdoc/>
        public TextValue() : base()
        {
        }
    }
}
