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

        if (PrimitiveEnum.ByteSize(is32bit) < 4)
        {
            return
                $"%{OutputVariableName} ={QbePrimitive.Int32(PrimitiveEnum.IsSignedInteger()).ToQbeString(is32bit)} {QbeRepresentation}{(PrimitiveEnum.IsSignedInteger() ? "s" : "u")}{PrimitiveEnum.ToQbeString(is32bit)} {refstr}";
        }

        return
            $"%{OutputVariableName} ={PrimitiveEnum.ToQbeString(is32bit)} {QbeRepresentation}{PrimitiveEnum.ToQbeString(is32bit)} {refstr}";

    }
}