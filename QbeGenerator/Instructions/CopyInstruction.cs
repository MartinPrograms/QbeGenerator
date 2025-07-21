namespace QbeGenerator.Instructions;

public class CopyInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "copy";

    public IQbeRef Identifier;
    public QbeValue Value;

    public CopyInstruction(IQbeRef varname, QbeValue value)
    {
        Identifier = varname;
        Value = value;
    }
    
    public string Emit(bool is32bit)
    {
        return $"{Identifier.GetValue()} ={Value.PrimitiveEnum.ToQbeString(is32bit)} {QbeRepresentation} {Value.GetValue()}";
    }
}