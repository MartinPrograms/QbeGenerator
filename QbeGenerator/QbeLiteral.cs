namespace QbeGenerator;

public class QbeLiteral : QbeValue
{
    private long _intValue;
    private double _floatValue;

    public QbeLiteral(IQbeTypeDefinition primitiveEnum, long intValue)
    {
        PrimitiveEnum = primitiveEnum;
        _intValue = intValue;
    }

    public QbeLiteral(IQbeTypeDefinition primitiveEnum, double floatValue)
    {
        PrimitiveEnum = primitiveEnum;
        _floatValue = floatValue;
    }

    public override string GetValue()
    {
        if (PrimitiveEnum.IsInteger())
        {
            return $"{_intValue}";
        }
        else if (PrimitiveEnum.IsFloat())
        {
            if (PrimitiveEnum == QbePrimitive.Float) return $"s_{_floatValue}";
            if (PrimitiveEnum == QbePrimitive.Double) return $"d_{_floatValue}";
        }

        throw new Exception("Type is not of integer, or float.");
    }
}