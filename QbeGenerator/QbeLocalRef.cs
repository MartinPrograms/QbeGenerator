namespace QbeGenerator;

public record QbeLocalRef : QbeValue, IQbeRef
{
    public string Identifier;

    public QbeLocalRef(IQbeTypeDefinition type, string identifier)
    {
        PrimitiveEnum = type;
        Identifier = identifier;
    }
    
    public override string GetValue()
    {
        return Qbe.Var(Identifier);
    }
}