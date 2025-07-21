namespace QbeGenerator;

public record QbeArgument(IQbeTypeDefinition Primitive, string Identifier)
{
    public string FullDefinition(bool is32bit)
    {
        return $"{Primitive.ToQbeString(is32bit)} %{Identifier}";
    }
}