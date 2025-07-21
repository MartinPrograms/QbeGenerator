namespace QbeGenerator.Instructions;

public class CallInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "call";

    public string FunctionName;
    public string OutputVariableName;
    public List<QbeValue> Arguments;
    public IQbeTypeDefinition? FunctionReturnType;

    public CallInstruction(string identifier, string outputVariableName, IQbeTypeDefinition? returnType, QbeValue[] arguments)
    {
        FunctionName = identifier;
        OutputVariableName = outputVariableName;
        FunctionReturnType = returnType;
        Arguments = arguments.ToList();
    }

    public string Emit(bool is32bit)
    {
        if (FunctionReturnType != null)
        {
            return
                $"%{OutputVariableName} ={FunctionReturnType.ToQbeString(is32bit)} {QbeRepresentation} ${FunctionName}({string.Join(", ", Arguments.Select(x => $"{x.PrimitiveEnum.ToQbeString(is32bit)} {x.GetValue()}"))})";
        }

        return
            $"{QbeRepresentation} ${FunctionName}({string.Join(", ", Arguments.Select(x => $"{x.PrimitiveEnum.ToQbeString(is32bit)} {x.GetValue()}"))})";
    }

    public override string ToString()
    {
        return OutputVariableName;
    }
}