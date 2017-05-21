﻿using System;

namespace MaterialForms.Wpf.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MessagesAttribute : Attribute
    {
        public string Title { get; set; }

        public string Details { get; set; }

        public string Create { get; set; }

        public string Edit { get; set; }

        public string Delete { get; set; }
    }
}