﻿using System.Globalization;
using System.Windows.Data;
using MaterialForms.Wpf.Resources;

namespace MaterialForms.Wpf.Validation
{
    public class NotNullValidator : FieldValidator
    {
        public NotNullValidator(IStringProxy errorMessage, IValueConverter valueConverter) : base(errorMessage, valueConverter)
        {
        }

        public NotNullValidator(IStringProxy errorMessage) : base(errorMessage)
        {
        }

        protected override bool ValidateValue(object value, CultureInfo cultureInfo)
        {
            return value != null;
        }
    }
}