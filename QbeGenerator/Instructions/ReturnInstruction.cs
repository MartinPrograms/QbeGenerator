namespace QbeGenerator.Instructions;

public class ReturnInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "ret";

    public QbeValue Value;

    public ReturnInstruction(QbeValue value)
    {
        Value = value;
    }

    public string Emit(bool is32bit)
    {
        return $"{QbeRepresentation} {Value.GetValue()}";
    }
}