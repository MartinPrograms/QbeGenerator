namespace QbeGenerator;

public interface IQbeTypeDefinition
{
    public string ToQbeString(bool is32bit);
    public bool IsInteger();
    public bool IsFloat();
    public long ByteSize(bool is32Bit);
    public bool IsEqual(IQbeTypeDefinition other)
    {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (this.GetType() != other.GetType()) return false;

        return ToQbeString(true) == other.ToQbeString(true);
    }
}