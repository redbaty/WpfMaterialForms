﻿using System.Windows.Controls;
using SchemaPrompt.Launcher.Controls;
using SchemaPrompt.Launcher.ViewModels;

namespace SchemaPrompt.Launcher.ViewModels
{
    internal class NumberRangeSchemaViewModel : BaseSchemaViewModel
    {
        private int value;
        private int minValue;
        private int maxValue;

        public int Value
        {
            get { return value; }
            set
            {
                if (value == this.value) return;
                this.value = value;
                OnPropertyChanged();
            }
        }

        public int MinValue
        {
            get { return minValue; }
            set
            {
                if (value == minValue) return;
                minValue = value;
                OnPropertyChanged();
            }
        }

        public int MaxValue
        {
            get { return maxValue; }
            set
            {
                if (value == maxValue) return;
                maxValue = value;
                OnPropertyChanged();
            }
        }

        public override UserControl CreateView()
        {
            return new SliderControl
            {
                DataContext = this
            };
        }
    }
}
