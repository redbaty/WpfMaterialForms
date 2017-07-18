﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MaterialForms.Wpf.Annotations;
using MaterialForms.Wpf.Resources;

namespace MaterialForms.Wpf.FormBuilding
{
    internal static class TypeUtilities
    {
        public static List<PropertyInfo> GetProperties(Type type, DefaultFields mode)
        {
            if (type == null)
            {
                throw new ArgumentException(nameof(type));
            }

            // First requirement is that properties and getters must be public.
            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetGetMethod(true).IsPublic);

            switch (mode)
            {
                case DefaultFields.AllIncludingReadonly:
                    return properties
                        .Where(p => p.GetCustomAttribute<FieldIgnoreAtribute>() == null)
                        .ToList();
                case DefaultFields.AllExcludingReadonly:
                    return properties.Where(p =>
                    {
                        if (p.GetCustomAttribute<FieldIgnoreAtribute>() != null)
                        {
                            return false;
                        }

                        if (p.GetCustomAttribute<FieldAttribute>() != null)
                        {
                            return true;
                        }

                        return p.CanWrite && p.GetSetMethod(true).IsPublic;
                    }).ToList();
                case DefaultFields.None:
                    return properties.Where(p =>
                    {
                        if (p.GetCustomAttribute<FieldIgnoreAtribute>() != null)
                        {
                            return false;
                        }

                        return p.GetCustomAttribute<FieldAttribute>() != null;
                    }).ToList();
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, "Invalid DefaultFields value.");
            }
        }

        public static IValueProvider GetResource<T>(object value, object defaultValue, Func<string, object> deserializer)
        {
            if (value == null)
            {
                return new LiteralValue(defaultValue);
            }

            if (value is string expression)
            {
                var boundExpression = BoundExpression.Parse(expression);
                switch (boundExpression.Resources.Count)
                {
                    case 0 when deserializer != null:
                        return new LiteralValue(deserializer(expression));
                    case 1 when boundExpression.StringFormat == null:
                        return new CoercedValueProvider<T>(boundExpression.Resources[0], defaultValue);
                    default:
                        throw new ArgumentException(
                            $"The expression '{expression}' is not a valid resource because it does not define a single value source.",
                            nameof(value));
                }
            }

            if (value is T)
            {
                return new LiteralValue(value);
            }

            throw new ArgumentException(
                $"The provided value must be a bound resource or a literal value of type '{typeof(T).FullName}'.",
                nameof(value));
        }

        public static IValueProvider GetStringResource(string expression)
        {
            return expression == null ? new LiteralValue(null) : BoundExpression.ParseSimplified(expression);
        }
    }
}