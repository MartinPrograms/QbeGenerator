namespace QbeGenerator.Instructions;

public class VariadicArgumentInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "vaarg";

    public QbeValue Value; // The value to get the argument from.
    public IQbeTypeDefinition PrimitiveEnum; // The primitive type of the argument.
    public string OutputVariableName; // The variable to store the argument in.
    
    public VariadicArgumentInstruction(QbeValue value, IQbeTypeDefinition primitiveEnum, string outputName)
    {
        Value = value;
        PrimitiveEnum = primitiveEnum;
        OutputVariableName = outputName;
    }
    
    public string Emit(bool is32bit)
    {
        return $"%{OutputVariableName} ={PrimitiveEnum.ToQbeString(is32bit)} {QbeRepresentation}{PrimitiveEnum.ToQbeString(is32bit)} {Value.GetValue()}";
    }
}