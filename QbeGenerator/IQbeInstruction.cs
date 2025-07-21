namespace QbeGenerator;

public interface IQbeInstruction : IEmit
{
    public string QbeRepresentation { get; }
}