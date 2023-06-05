using System;
using System.ComponentModel;
using System.Globalization;

namespace BlazingFlame.UniversalBanList.Database;
internal class UtcDateTimeOffsetConverter : TypeConverter
{
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(DateTime) || destinationType == typeof(long) || destinationType == typeof(string);
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (value is DateTimeOffset offset)
        {
            if (destinationType == typeof(DateTime))
                return offset.UtcDateTime;
            if (destinationType == typeof(long))
                return offset.UtcTicks;
            if (destinationType == typeof(string))
                return offset.ToString("O");
        }
        
        throw GetConvertToException(value, destinationType);
    }
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is DateTimeOffset)
            return value;

        if (value is DateTime dateTime)
            return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));

        if (value is long ticks)
            return new DateTimeOffset(new DateTime(ticks, DateTimeKind.Utc));

        if (value is string str && DateTimeOffset.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset offset2))
            return offset2;

        throw GetConvertFromException(value);
    }
}

internal class NullableUtcDateTimeOffsetConverter : UtcDateTimeOffsetConverter
{
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => base.CanConvertTo(context, destinationType) || destinationType == typeof(DateTime?) || destinationType == typeof(long?) || destinationType == typeof(string);
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        // ReSharper disable once RedundantExplicitNullableCreation
        return value.Equals(null) ? default(DateTimeOffset?)! : new DateTimeOffset?((DateTimeOffset)base.ConvertFrom(context, culture, value));
    }
}