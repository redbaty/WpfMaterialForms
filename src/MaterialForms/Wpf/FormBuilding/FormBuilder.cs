﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MaterialForms.Wpf.Annotations;
using MaterialForms.Wpf.Fields;
using MaterialForms.Wpf.FormBuilding.Defaults;
using MaterialForms.Wpf.FormBuilding.Defaults.Initializers;
using MaterialForms.Wpf.FormBuilding.Defaults.Properties;
using MaterialForms.Wpf.FormBuilding.Defaults.Types;
using MaterialForms.Wpf.Resources;

namespace MaterialForms.Wpf.FormBuilding
{
    public interface IFormBuilder
    {
        IFormDefinition GetDefinition(Type type);
    }

    /// <summary>
    /// Configurable object responsible for creating form definitions from types.
    /// </summary>
    public class FormBuilder : IFormBuilder
    {
        /// <summary>
        /// Default instance of <see cref="FormBuilder" />.
        /// </summary>
        public static readonly FormBuilder Default = new FormBuilder();

        private readonly Dictionary<Type, FormDefinition> cachedDefinitions;

        public FormBuilder()
        {
            cachedDefinitions = new Dictionary<Type, FormDefinition>();
            PropertyBuilders = new List<IFieldBuilder>
            {
                // Default property builders.
                new SelectFromBuilder()
            };

            List<IFieldBuilder> AsList(params IFieldBuilder[] builders)
            {
                return builders.ToList();
            }

            Primitives = new Dictionary<Type, IFormDefinition>
            {
                [typeof(string)] = Primitive.String(),

                [typeof(DateTime)] = Primitive.DateTime(),
                [typeof(bool)] = Primitive.Boolean(),
                [typeof(char)] = Primitive.Char(),
                [typeof(byte)] = Primitive.Byte(),
                [typeof(sbyte)] = Primitive.SByte(),
                [typeof(short)] = Primitive.Int16(),
                [typeof(int)] = Primitive.Int32(),
                [typeof(long)] = Primitive.Int64(),
                [typeof(ushort)] = Primitive.UInt16(),
                [typeof(uint)] = Primitive.UInt32(),
                [typeof(ulong)] = Primitive.UInt64(),
                [typeof(float)] = Primitive.Single(),
                [typeof(double)] = Primitive.Double(),
                [typeof(decimal)] = Primitive.Decimal(),

                // Nullables will default to non-nullable,
                [typeof(DateTime?)] = Primitive.DateTime(),
                [typeof(bool?)] = Primitive.Boolean(),
                [typeof(char?)] = Primitive.Char(),
                [typeof(byte?)] = Primitive.Byte(),
                [typeof(sbyte?)] = Primitive.SByte(),
                [typeof(short?)] = Primitive.Int16(),
                [typeof(int?)] = Primitive.Int32(),
                [typeof(long?)] = Primitive.Int64(),
                [typeof(ushort?)] = Primitive.UInt16(),
                [typeof(uint?)] = Primitive.UInt32(),
                [typeof(ulong?)] = Primitive.UInt64(),
                [typeof(float?)] = Primitive.Single(),
                [typeof(double?)] = Primitive.Double(),
                [typeof(decimal?)] = Primitive.Decimal()
            };

            TypeBuilders = new Dictionary<Type, List<IFieldBuilder>>
            {
                // Default type builders.

                // Native controls
                [typeof(string)] = AsList(new StringFieldBuilder()),

                // Temporarily converted.
                [typeof(DateTime)] = AsList(new ConvertedFieldBuilder(Deserializers.DateTime)),
                [typeof(DateTime?)] = AsList(new ConvertedFieldBuilder(Deserializers.NullableDateTime)),

                // Boolean
                [typeof(bool)] = AsList(new BooleanFieldBuilder()),
                [typeof(bool?)] = AsList(new BooleanFieldBuilder()),

                // Converted non-nullable
                [typeof(char)] = AsList(new ConvertedFieldBuilder(Deserializers.Char)),
                [typeof(byte)] = AsList(new ConvertedFieldBuilder(Deserializers.Byte)),
                [typeof(sbyte)] = AsList(new ConvertedFieldBuilder(Deserializers.SByte)),
                [typeof(short)] = AsList(new ConvertedFieldBuilder(Deserializers.Int16)),
                [typeof(int)] = AsList(new ConvertedFieldBuilder(Deserializers.Int32)),
                [typeof(long)] = AsList(new ConvertedFieldBuilder(Deserializers.Int64)),
                [typeof(ushort)] = AsList(new ConvertedFieldBuilder(Deserializers.UInt16)),
                [typeof(uint)] = AsList(new ConvertedFieldBuilder(Deserializers.UInt32)),
                [typeof(ulong)] = AsList(new ConvertedFieldBuilder(Deserializers.UInt64)),
                [typeof(float)] = AsList(new ConvertedFieldBuilder(Deserializers.Single)),
                [typeof(double)] = AsList(new ConvertedFieldBuilder(Deserializers.Double)),
                [typeof(decimal)] = AsList(new ConvertedFieldBuilder(Deserializers.Decimal)),
                
                // Converted nullable
                [typeof(char?)] = AsList(new ConvertedFieldBuilder(Deserializers.NullableChar)),
                [typeof(byte?)] = AsList(new ConvertedFieldBuilder(Deserializers.NullableByte)),
                [typeof(sbyte?)] = AsList(new ConvertedFieldBuilder(Deserializers.NullableSByte)),
                [typeof(short?)] = AsList(new ConvertedFieldBuilder(Deserializers.NullableInt16)),
                [typeof(int?)] = AsList(new ConvertedFieldBuilder(Deserializers.NullableInt32)),
                [typeof(long?)] = AsList(new ConvertedFieldBuilder(Deserializers.NullableInt64)),
                [typeof(ushort?)] = AsList(new ConvertedFieldBuilder(Deserializers.NullableUInt16)),
                [typeof(uint?)] = AsList(new ConvertedFieldBuilder(Deserializers.NullableUInt32)),
                [typeof(ulong?)] = AsList(new ConvertedFieldBuilder(Deserializers.NullableUInt64)),
                [typeof(float?)] = AsList(new ConvertedFieldBuilder(Deserializers.NullableSingle)),
                [typeof(double?)] = AsList(new ConvertedFieldBuilder(Deserializers.NullableDouble)),
                [typeof(decimal?)] = AsList(new ConvertedFieldBuilder(Deserializers.NullableDecimal)),
            };

            FieldInitializers = new List<IFieldInitializer>
            {
                // Default initializers.
                new FieldInitializer(),
                new BindingInitializer(),
                new ValidatorInitializer()
            };

            TypeDeserializers = new Dictionary<Type, Func<string, object>>
            {
                // Default deserializers - culture invariant.
                [typeof(object)] = Deserializers.String,
                [typeof(string)] = Deserializers.String,
                [typeof(DateTime)] = Deserializers.DateTime,
                [typeof(bool)] = Deserializers.Boolean,
                [typeof(char)] = Deserializers.Char,
                [typeof(byte)] = Deserializers.Byte,
                [typeof(sbyte)] = Deserializers.SByte,
                [typeof(short)] = Deserializers.Int16,
                [typeof(int)] = Deserializers.Int32,
                [typeof(long)] = Deserializers.Int64,
                [typeof(ushort)] = Deserializers.UInt16,
                [typeof(uint)] = Deserializers.UInt32,
                [typeof(ulong)] = Deserializers.UInt64,
                [typeof(float)] = Deserializers.Single,
                [typeof(double)] = Deserializers.Double,
                [typeof(decimal)] = Deserializers.Decimal,
                [typeof(DateTime?)] = Deserializers.NullableDateTime,
                [typeof(bool?)] = Deserializers.NullableBoolean,
                [typeof(char?)] = Deserializers.NullableChar,
                [typeof(byte?)] = Deserializers.NullableByte,
                [typeof(sbyte?)] = Deserializers.NullableSByte,
                [typeof(short?)] = Deserializers.NullableInt16,
                [typeof(int?)] = Deserializers.NullableInt32,
                [typeof(long?)] = Deserializers.NullableInt64,
                [typeof(ushort?)] = Deserializers.NullableUInt16,
                [typeof(uint?)] = Deserializers.NullableUInt32,
                [typeof(ulong?)] = Deserializers.NullableUInt64,
                [typeof(float?)] = Deserializers.NullableSingle,
                [typeof(double?)] = Deserializers.NullableDouble,
                [typeof(decimal?)] = Deserializers.NullableDecimal
            };
        }

        /// <summary>
        /// Contains prebuilt definitions for primitive types.
        /// Types stored here will bypass form generation process.
        /// Initially, value types and strings are stored here.
        /// </summary>
        public Dictionary<Type, IFormDefinition> Primitives { get; }

        /// <summary>
        /// Gets the list of registered field builders which are queried first.
        /// </summary>
        public List<IFieldBuilder> PropertyBuilders { get; }

        /// <summary>
        /// Gets the mapping of types to respective field builders.
        /// These are queried if no field builder from <see cref="PropertyBuilders" /> succeeds in creating an element.
        /// </summary>
        public Dictionary<Type, List<IFieldBuilder>> TypeBuilders { get; }

        /// <summary>
        /// Stores functions to parse string representations of types.
        /// </summary>
        public Dictionary<Type, Func<string, object>> TypeDeserializers { get; }

        public List<IFieldInitializer> FieldInitializers { get; }

        /// <summary>
        /// Gets the <see cref="FormDefinition" /> for the provided type.
        /// The cache is looked up first before generating the form.
        /// </summary>
        /// <remarks>
        /// If current <see cref="FormBuilder" /> configuration has changed,
        /// clearing the cache using <see cref="ClearCache" /> may be necessary.
        /// </remarks>
        public IFormDefinition GetDefinition(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (Primitives.TryGetValue(type, out var primitiveForm))
            {
                return primitiveForm;
            }

            if (cachedDefinitions.TryGetValue(type, out var formDefinition))
            {
                return formDefinition;
            }

            formDefinition = BuildDefinition(type);
            cachedDefinitions[type] = formDefinition;
            return formDefinition;
        }

        /// <summary>
        /// Clears cached form definitions.
        /// This may be necessary when current configuration has changed.
        /// </summary>
        public void ClearCache()
        {
            cachedDefinitions.Clear();
        }

        /// <summary>
        /// Removes a single type from the form definition cache.
        /// </summary>
        public bool ClearCached<T>()
        {
            return cachedDefinitions.Remove(typeof(T));
        }

        private FormDefinition BuildDefinition(Type type)
        {
            // Only classes are allowed.
            // Primitives should be retrieved from prebuilt definitions.
            if (!type.IsClass || typeof(MulticastDelegate).IsAssignableFrom(type.BaseType))
            {
                return null;
            }

            var formDefinition = new FormDefinition(type);
            var mode = DefaultFields.AllExcludingReadonly;
            var grid = new[] { 1d };
            var beforeFormContent = new List<FormContentAttribute>();
            var afterFormContent = new List<FormContentAttribute>();
            foreach (var attribute in type.GetCustomAttributes())
            {
                switch (attribute)
                {
                    case ResourceAttribute resource:
                        formDefinition.Resources.Add(resource.Name, resource.Value is string expr
                            ? (IValueProvider)BoundExpression.Parse(expr)
                            : new LiteralValue(resource.Value));
                        break;
                    case FormAttribute form:
                        mode = form.Mode;
                        grid = form.Grid;
                        if (grid == null || grid.Length < 1)
                        {
                            grid = new[] { 1d };
                        }

                        break;
                    case FormContentAttribute contentAttribute:
                        if (contentAttribute.InsertAfter)
                        {
                            afterFormContent.Add(contentAttribute);
                        }
                        else
                        {
                            beforeFormContent.Add(contentAttribute);
                        }

                        break;
                }
            }

            beforeFormContent.Sort((a, b) => a.Position.CompareTo(b.Position));
            afterFormContent.Sort((a, b) => a.Position.CompareTo(b.Position));

            var gridLength = grid.Length;

            // Pass one - get list of valid properties.
            var properties = Utilities.GetProperties(type, mode);

            // Pass two - build form elements.
            var elements = new List<ElementWrapper>();
            foreach (var property in properties)
            {
                var deserializer = TryGetDeserializer(property.PropertyType);
                // Query property builders.
                var element = Build(property, deserializer, PropertyBuilders);
                if (element == null && TypeBuilders.TryGetValue(property.PropertyType, out var builders))
                {
                    // Query type builders if no property builder succeeds.
                    element = Build(property, deserializer, builders);
                }

                if (element == null)
                {
                    // Unhandled properties are ignored.
                    continue;
                }

                // Pass three - initialize elements.
                foreach (var initializer in FieldInitializers)
                {
                    initializer.Initialize(element, property, deserializer);
                }

                var wrapper = new ElementWrapper(element, property);
                // Set layout.
                var attr = property.GetCustomAttribute<FieldAttribute>();
                if (attr != null)
                {
                    wrapper.Position = attr.Position;
                    wrapper.Row = attr.Row;
                    wrapper.Column = attr.Column;
                    wrapper.ColumnSpan = attr.ColumnSpan;
                }

                elements.Add(wrapper);
            }

            // Pass four - order elements.
            elements = elements.OrderBy(element => element.Position).ToList();

            // Pass five - group rows and calculate layout.
            var layout = PerformLayout(grid, elements);

            // Pass six - add attached elements.
            var rows = new List<FormRow>();

            // Before form.
            rows.AddRange(beforeFormContent.Select(attr => CreateRow(attr.GetElement(type), gridLength)));

            for (var i = 0; i < layout.Count; i++)
            {
                var row = layout[i];
                var before = new List<(FormElement element, int position)>();
                var after = new List<(FormElement element, int position)>();
                foreach (var element in row.Elements)
                {
                    var property = element.PropertyInfo;
                    foreach (var attr in property.GetCustomAttributes<FormContentAttribute>())
                    {
                        var content = (attr.GetElement(property), attr.Position);
                        if (attr.InsertAfter)
                        {
                            after.Add(content);
                        }
                        else
                        {
                            before.Add(content);
                        }
                    }
                }

                before.Sort((a, b) => a.position.CompareTo(b.position));
                after.Sort((a, b) => a.position.CompareTo(b.position));

                // Before element.
                rows.AddRange(before.Select(t => CreateRow(t.element, gridLength)));

                // Field row.
                var formRow = new FormRow();
                formRow.Elements.AddRange(
                    row.Elements.Select(w => new FormElementContainer(w.Column, w.ColumnSpan, w.Element)));
                rows.Add(formRow);

                // After element.
                rows.AddRange(after.Select(t => CreateRow(t.element, gridLength)));
            }

            // After form.
            rows.AddRange(afterFormContent.Select(attr => CreateRow(attr.GetElement(type), gridLength)));

            // Wrap up everything.
            formDefinition.Grid = grid;
            formDefinition.FormRows = rows;
            formDefinition.Freeze();
            foreach (var element in formDefinition.FormRows.SelectMany(r => r.Elements).Select(c => c.Element))
            {
                element.Freeze();
            }

            return formDefinition;
        }

        private Func<string, object> TryGetDeserializer(Type type)
        {
            if (TypeDeserializers.TryGetValue(type, out var deserializer))
            {
                return deserializer;
            }

            return type.IsEnum ? Deserializers.Enum(type) : null;
        }

        private static FormRow CreateRow(FormElement element, int gridLength)
        {
            var row = new FormRow();
            row.Elements.Add(new FormElementContainer(0, gridLength, element));
            return row;
        }

        private static List<ElementRow> PerformLayout(double[] grid, List<ElementWrapper> elements)
        {
            var gridLength = grid.Length;
            var layout = new List<ElementRow>();
            foreach (var element in elements)
            {
                if (element.Column < 0)
                {
                    element.Column = 0;
                }
                else if (element.Column >= gridLength)
                {
                    element.Column = gridLength - 1;
                }

                if (element.ColumnSpan < 1)
                {
                    element.ColumnSpan = 1;
                }
                else if (element.ColumnSpan > gridLength)
                {
                    element.ColumnSpan = gridLength;
                }

                if (element.Row == null)
                {
                    layout.Add(new ElementRow(null, element));
                }
                else
                {
                    var rowName = element.Row;
                    var added = false;
                    for (var i = 0; i < layout.Count; i++)
                    {
                        var row = layout[i];
                        if (row.RowName != rowName)
                        {
                            continue;
                        }

                        if (row.Sealed)
                        {
                            // Bad scenario - too many elements in one row.
                            // Search for next row with the same name.
                            // If none exists, insert a new one in the cluster.
                            var found = false;
                            int j;
                            for (j = i + 1; j < layout.Count; j++)
                            {
                                var nextRow = layout[j];
                                if (nextRow.RowName != rowName)
                                {
                                    break;
                                }

                                if (nextRow.Sealed)
                                {
                                    continue;
                                }

                                row = nextRow;
                                found = true;
                                break;
                            }

                            if (!found)
                            {
                                row = new ElementRow(rowName);
                                layout.Insert(j, row);
                            }
                        }

                        row.Elements.Add(element);
                        row.ColumnSpan += element.ColumnSpan;
                        if (row.ColumnSpan >= gridLength)
                        {
                            row.Sealed = true;
                        }

                        added = true;
                        break;
                    }

                    if (!added)
                    {
                        var row = new ElementRow(rowName, element);
                        layout.Add(row);
                        if (row.ColumnSpan >= gridLength)
                        {
                            row.Sealed = true;
                        }
                    }
                }
            }

            foreach (var row in layout)
            {
                if (row.RowName == null)
                {
                    var element = row.Elements[0];
                    if (element.Column >= gridLength)
                    {
                        element.Column = gridLength - 1;
                    }

                    row.Elements[0].ColumnSpan = gridLength - element.Column;
                }
                else
                {
                    row.Elements.Sort((a, b) => a.Column.CompareTo(b.Column));
                    var rowElements = row.Elements;
                    var count = 0;
                    var gaps = new int[rowElements.Count];
                    for (var i = 0; i < rowElements.Count; i++)
                    {
                        var element = rowElements[i];
                        if (element.Column > count)
                        {
                            gaps[i] = element.Column - count;
                            count = element.Column;
                        }

                        if (count > element.Column)
                        {
                            element.Column = count;
                        }

                        count += element.ColumnSpan;
                    }

                    if (count > gridLength)
                    {
                        // Overflow - close gaps starting from right.
                        var delta = count - gridLength;
                        for (var i = gaps.Length - 1; i >= 0; i--)
                        {
                            if (delta == 0)
                            {
                                break;
                            }

                            var gap = gaps[i];
                            if (gap == 0)
                            {
                                continue;
                            }

                            var subtraction = Math.Min(delta, gap);
                            for (var j = i; j < gaps.Length; j++)
                            {
                                rowElements[j].Column -= subtraction;
                            }

                            delta -= subtraction;
                        }
                    }
                }
            }

            return layout;
        }

        private static FormElement Build(PropertyInfo property, Func<string, object> deserializer,
            List<IFieldBuilder> builders)
        {
            foreach (var builder in builders)
            {
                var element = builder.TryBuild(property, deserializer);
                if (element != null)
                {
                    return element;
                }
            }

            return null;
        }

        private class ElementWrapper
        {
            public readonly FormElement Element;
            public readonly PropertyInfo PropertyInfo;
            public int Column;
            public int ColumnSpan;
            public int Position;
            public string Row;

            public ElementWrapper(FormElement element, PropertyInfo propertyInfo)
            {
                Element = element;
                PropertyInfo = propertyInfo;
            }
        }

        private class ElementRow
        {
            public readonly List<ElementWrapper> Elements;
            public readonly string RowName;
            public int ColumnSpan;
            public bool Sealed;

            public ElementRow(string rowName = null, ElementWrapper element = null)
            {
                Elements = new List<ElementWrapper>();
                if (element != null)
                {
                    Elements.Add(element);
                    ColumnSpan = element.ColumnSpan;
                }

                RowName = rowName;
            }
        }
    }
}
