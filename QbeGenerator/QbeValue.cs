namespace QbeGenerator;

public abstract record QbeValue : IQbeRef
{
    public IQbeTypeDefinition PrimitiveEnum;
    public abstract string GetValue();
}

public interface IQbeRef
{
    // IQbeReference
    public string GetValue();
}