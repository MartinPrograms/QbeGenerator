namespace QbeGenerator.Instructions;

public class BlitInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "blit";

    public QbeValue Source;
    public QbeValue Destination;
    public long Size;
    
    public BlitInstruction(QbeValue source, QbeValue destination, long size)
    {
        Source = source;
        Destination = destination;
        Size = size;
    }
    
    public string Emit(bool is32bit)
    {
        return $"{QbeRepresentation}{Source.PrimitiveEnum.ToQbeString(is32bit)} {Source.GetValue()}, {Destination.GetValue()}, {Size}";
    }
}