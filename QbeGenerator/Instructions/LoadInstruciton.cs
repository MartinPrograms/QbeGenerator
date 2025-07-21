namespace QbeGenerator.Instructions;

public class LoadInstruciton : IQbeInstruction
{
    public string QbeRepresentation { get; } = "load";

    public IQbeTypeDefinition PrimitiveEnum;
    public IQbeRef Reference;
    public string OutputVariableName;
    
    public LoadInstruciton(IQbeTypeDefinition primitiveEnum, IQbeRef reference, string outputName)
    {
        PrimitiveEnum = primitiveEnum;
        Reference = reference;
        OutputVariableName = outputName;
    }
    
    public string Emit(bool is32bit)
    {
        var refstr = Reference.GetValue();

        return $"%{OutputVariableName} ={PrimitiveEnum.ToQbeString(is32bit)} {QbeRepresentation}{PrimitiveEnum.ToQbeString(is32bit)} {refstr}";
    }
}