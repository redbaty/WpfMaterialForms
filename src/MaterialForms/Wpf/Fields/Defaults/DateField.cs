﻿using System;
using System.Collections.Generic;
using MaterialForms.Wpf.Resources;

namespace MaterialForms.Wpf.Fields.Defaults
{
    public class DateField : DataFormField
    {
        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        public DateField(string key) : base(key, typeof(DateTime))
        {
        }

        protected internal override IBindingProvider CreateBindingProvider(IResourceContext context,
            IDictionary<string, IValueProvider> formResources)
        {
            var datePresenter = new DatePresenter(context, Resources, formResources);
            return datePresenter;
        }
    }
}