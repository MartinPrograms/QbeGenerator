namespace QbeGenerator.Instructions;

public class VariadicStartInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "vastart"; // Unused

    public QbeValue Value; // The value to initialize.
    
    public VariadicStartInstruction(QbeValue value)
    {
        Value = value;
    }
    
    public string Emit(bool is32bit)
    {
        return $"{QbeRepresentation} {Value.GetValue()}";
    }
}