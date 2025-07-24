namespace QbeGenerator.Instructions;

public class ReturnInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "ret";

    public QbeValue? Value;

    public ReturnInstruction(QbeValue? value)
    {
        Value = value;
    }    
    
    public ReturnInstruction()
    {
        Value = null;
    }

    public string Emit(bool is32bit)
    {
        if (Value == null)
        {
            return $"{QbeRepresentation}";
        }
        return $"{QbeRepresentation} {Value.GetValue()}";
    }
}