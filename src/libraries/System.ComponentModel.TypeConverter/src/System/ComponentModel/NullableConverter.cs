// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel
{
    /// <summary>
    /// TypeConverter to convert Nullable types to and from strings or the underlying simple type.
    /// </summary>
    public class NullableConverter : TypeConverter
    {
        private Type _nullableType;
        private Type _simpleType;
        private TypeConverter _simpleTypeConverter;

        /// <summary>
        /// Nullable converter is initialized with the underlying simple type.
        /// </summary>
        public NullableConverter(Type type)
        {
            _nullableType = type;

            _simpleType = Nullable.GetUnderlyingType(type);
            if (_simpleType == null)
            {
                throw new ArgumentException(SR.NullableConverterBadCtorArg, nameof(type));
            }

            _simpleTypeConverter = TypeDescriptor.GetConverter(_simpleType);
        }

        /// <summary>
        ///    <para>Gets a value indicating whether this converter can convert an object in the
        ///       given source type to the underlying simple type or a null.</para>
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == _simpleType)
            {
                return true;
            }
            else if (_simpleTypeConverter != null)
            {
                return _simpleTypeConverter.CanConvertFrom(context, sourceType);
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }

        /// <summary>
        ///    Converts the given value to the converter's underlying simple type or a null.
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null || value.GetType() == _simpleType)
            {
                return value;
            }
            else if (value is string && string.IsNullOrEmpty(value as string))
            {
                return null;
            }
            else if (_simpleTypeConverter != null)
            {
                return _simpleTypeConverter.ConvertFrom(context, culture, value);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this converter can convert a value object to the destination type.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == _simpleType)
            {
                return true;
            }
            else if (_simpleTypeConverter != null)
            {
                return _simpleTypeConverter.CanConvertTo(context, destinationType);
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        /// <summary>
        /// Converts the given value object to the destination type.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == _simpleType && value != null && _nullableType.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
            {
                return value;
            }
            else if (value == null)
            {
                // Handle our own nulls here
                if (destinationType == typeof(string))
                {
                    return string.Empty;
                }
            }
            else if (_simpleTypeConverter != null)
            {
                return _simpleTypeConverter.ConvertTo(context, culture, value, destinationType);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <include file='doc\NullableConverter.uex' path='docs/doc[@for="NullableConverter.CreateInstance"]/*' />
        /// <summary>
        /// </summary>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (_simpleTypeConverter != null)
            {
                object instance = _simpleTypeConverter.CreateInstance(context, propertyValues);
                return instance;
            }

            return base.CreateInstance(context, propertyValues);
        }

        /// <summary>
        ///    <para>
        ///        Gets a value indicating whether changing a value on this object requires a call to
        ///        <see cref='System.ComponentModel.TypeConverter.CreateInstance'/> to create a new value,
        ///        using the specified context.
        ///    </para>
        /// </summary>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            if (_simpleTypeConverter != null)
            {
                return _simpleTypeConverter.GetCreateInstanceSupported(context);
            }

            return base.GetCreateInstanceSupported(context);
        }

#if !NETSTANDARD10
        /// <summary>
        ///    <para>
        ///        Gets a collection of properties for the type of array specified by the value
        ///        parameter using the specified context and attributes.
        ///    </para>
        /// </summary>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (_simpleTypeConverter != null)
            {
                object unwrappedValue = value;
                return _simpleTypeConverter.GetProperties(context, unwrappedValue, attributes);
            }

            return base.GetProperties(context, value, attributes);
        }
#endif // !NETSTANDARD10

        /// <summary>
        ///    <para>Gets a value indicating whether this object supports properties using the specified context.</para>
        /// </summary>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            if (_simpleTypeConverter != null)
            {
                return _simpleTypeConverter.GetPropertiesSupported(context);
            }

            return base.GetPropertiesSupported(context);
        }

        /// <summary>
        ///    <para>Gets a collection of standard values for the data type this type converter is designed for.</para>
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (_simpleTypeConverter != null)
            {
                StandardValuesCollection values = _simpleTypeConverter.GetStandardValues(context);
                if (GetStandardValuesSupported(context) && values != null)
                {
                    // Create a set of standard values around nullable instances.  
                    object[] wrappedValues = new object[values.Count + 1];
                    int idx = 0;

                    wrappedValues[idx++] = null;
                    foreach (object value in values)
                    {
                        wrappedValues[idx++] = value;
                    }

                    return new StandardValuesCollection(wrappedValues);
                }
            }

            return base.GetStandardValues(context);
        }

        /// <summary>
        ///    <para>
        ///        Gets a value indicating whether the collection of standard values returned from
        ///        <see cref='System.ComponentModel.TypeConverter.GetStandardValues'/> is an exclusive 
        ///        list of possible values, using the specified context.
        ///    </para>
        /// </summary>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            if (_simpleTypeConverter != null)
            {
                return _simpleTypeConverter.GetStandardValuesExclusive(context);
            }

            return base.GetStandardValuesExclusive(context);
        }

        /// <summary>
        ///    <para>
        ///        Gets a value indicating whether this object supports a standard set of values that can
        ///        be picked from a list using the specified context.
        ///    </para>
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            if (_simpleTypeConverter != null)
            {
                return _simpleTypeConverter.GetStandardValuesSupported(context);
            }

            return base.GetStandardValuesSupported(context);
        }

        /// <summary>
        ///    <para>Gets a value indicating whether the given value object is valid for this type.</para>
        /// </summary>
        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            if (_simpleTypeConverter != null)
            {
                object unwrappedValue = value;
                if (unwrappedValue == null)
                {
                    return true; // null is valid for nullable.
                }
                else
                {
                    return _simpleTypeConverter.IsValid(context, unwrappedValue);
                }
            }

            return base.IsValid(context, value);
        }

        /// <summary>
        /// The type this converter was initialized with.
        /// </summary>
        public Type NullableType
        {
            get
            {
                return _nullableType;
            }
        }

        /// <summary>
        /// The simple type that is represented as a nullable.
        /// </summary>
        public Type UnderlyingType
        {
            get
            {
                return _simpleType;
            }
        }

        /// <summary>
        /// Converter associated with the underlying simple type.
        /// </summary>
        public TypeConverter UnderlyingTypeConverter
        {
            get
            {
                return _simpleTypeConverter;
            }
        }
    }
}
