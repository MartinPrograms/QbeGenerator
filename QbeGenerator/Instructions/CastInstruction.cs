namespace QbeGenerator.Instructions;

public class CastInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "cast";

    public QbeValue Value;
    public IQbeTypeDefinition PrimitiveEnum;
    public string OutputVariableName;
    
    public CastInstruction(QbeValue value, IQbeTypeDefinition primitiveEnum, string outputName)
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