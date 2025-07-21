namespace QbeGenerator.Instructions;

public class StoreInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "store";

    public QbeValue Address;
    public QbeValue Value;

    public StoreInstruction(QbeValue address, QbeValue value)
    {
        Address = address;
        Value = value;
    }

    public string Emit(bool is32bit)
    {
        return $"{QbeRepresentation}{Value.PrimitiveEnum.ToQbeString(is32bit)} {Value.GetValue()}, {Address.GetValue()}";
    }
}