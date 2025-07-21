namespace QbeGenerator;

public enum QbePrimitiveEnum
{
    Int32,
    Int64,
    Float,
    Double,
    Byte,
    Int16,
    Pointer
}

public record QbePrimitive : IQbeTypeDefinition
{
    public QbePrimitiveEnum PrimitiveEnum { get; }

    public QbePrimitive(QbePrimitiveEnum primitiveEnum)
    {
        PrimitiveEnum = primitiveEnum;
    }

    public string ToQbeString(bool is32bit)
    {
        return PrimitiveEnum.ToQbeString(is32bit);
    }

    public bool IsInteger()
    {
        return PrimitiveEnum.IsInteger();
    }

    public bool IsFloat()
    {
        return PrimitiveEnum.IsFloat();
    }

    public long ByteSize(bool is32Bit)
    {
        return PrimitiveEnum.ByteSize(is32Bit);
    }

    public static QbePrimitive Int32 => new QbePrimitive(QbePrimitiveEnum.Int32);
    public static QbePrimitive Int64 => new QbePrimitive(QbePrimitiveEnum.Int64);
    public static QbePrimitive Float => new QbePrimitive(QbePrimitiveEnum.Float);
    public static QbePrimitive Double => new QbePrimitive(QbePrimitiveEnum.Double);
    public static QbePrimitive Byte => new QbePrimitive(QbePrimitiveEnum.Byte);
    public static QbePrimitive Int16 => new QbePrimitive(QbePrimitiveEnum.Int16);
    public static QbePrimitive Pointer => new QbePrimitive(QbePrimitiveEnum.Pointer);
    public static QbePrimitive FromString(string str)
    {
        return new QbePrimitive(str.ToQbePrimitive());
    }
    public static QbePrimitive FromEnum(QbePrimitiveEnum primitiveEnum)
    {
        return new QbePrimitive(primitiveEnum);
    }
}

public static class QbeTypeExtensions
{
    public static string ToQbeString(this QbePrimitiveEnum primitiveEnum, bool is32bit)
    {
        return primitiveEnum switch
        {
            QbePrimitiveEnum.Int32 => "w",
            QbePrimitiveEnum.Int64 => "l",
            QbePrimitiveEnum.Float => "s",
            QbePrimitiveEnum.Double => "d",
            QbePrimitiveEnum.Byte => "b",
            QbePrimitiveEnum.Int16 => "h",
            QbePrimitiveEnum.Pointer => is32bit ? "w" : "l",
            _ => throw new ArgumentOutOfRangeException(nameof(primitiveEnum), primitiveEnum, null)
        };
    }

    public static QbePrimitiveEnum ToQbePrimitive(this string str)
    {
        return str switch
        {
            "w" => QbePrimitiveEnum.Int32,
            "l" => QbePrimitiveEnum.Int64,
            "s" => QbePrimitiveEnum.Float,
            "d" => QbePrimitiveEnum.Double,
            "b" => QbePrimitiveEnum.Byte,
            "h" => QbePrimitiveEnum.Int16,
            _ => throw new ArgumentOutOfRangeException($"{str} does not correlate to a QBE type.")
        };
    }

    public static bool IsInteger(this QbePrimitiveEnum primitiveEnum)
    {
        switch(primitiveEnum)
        {
            case QbePrimitiveEnum.Int16:
            case QbePrimitiveEnum.Int32:
            case QbePrimitiveEnum.Int64:
            case QbePrimitiveEnum.Byte:
            case QbePrimitiveEnum.Pointer:
                return true;
            
            default:
            return false;
        }
    }

    public static bool IsFloat(this QbePrimitiveEnum primitiveEnum)
    {
        return primitiveEnum == QbePrimitiveEnum.Float || primitiveEnum == QbePrimitiveEnum.Double;
    }

    public static int ByteSize(this QbePrimitiveEnum primitiveEnum, bool is32Bit)
    {
        switch (primitiveEnum)
        {
            case QbePrimitiveEnum.Int64:
            case QbePrimitiveEnum.Double:
                return 8;
            
            case QbePrimitiveEnum.Float:
            case QbePrimitiveEnum.Int32:
                return 4;
            
            case QbePrimitiveEnum.Int16:
                return 2;
            
            case QbePrimitiveEnum.Byte:
                return 1;
            
            case QbePrimitiveEnum.Pointer:
                return is32Bit ? 4 : 8;
        }

        throw new ArgumentOutOfRangeException();
    }
}