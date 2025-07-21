namespace QbeGenerator;

public abstract class QbeValue
{
    public IQbeTypeDefinition PrimitiveEnum;
    public abstract string GetValue();
}

public interface IQbeRef
{
    // IQbeReference
    public string GetValue();
}