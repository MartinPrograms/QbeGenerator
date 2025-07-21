namespace QbeGenerator.Instructions;

public enum Alignment
{
    Four,
    Eight,
    Sixteen
}

public static class AlignmentExtensions
{
    public static int ToInt(this Alignment alignment)
    {
        return alignment switch
        {
            Alignment.Four => 4,
            Alignment.Eight => 8,
            Alignment.Sixteen => 16
        };
    }
}

public class AllocInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "alloc";

    public string OutputVariableName;
    public Alignment Alignment;
    public long Size;
    public IQbeTypeDefinition OutputType;

    public AllocInstruction(string outputVariableName, long size, Alignment alignment)
    {
        OutputVariableName = outputVariableName;
        Size = size;
        Alignment = alignment;
        OutputType = QbePrimitive.Pointer;
    }
    
    public string Emit(bool is32bit)
    {
        return $"%{OutputVariableName} ={OutputType.ToQbeString(is32bit)} {QbeRepresentation}{Alignment.ToInt()} {Size}";
    }
}