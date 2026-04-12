using System.Text;

namespace QbeGenerator.Instructions;

public class CallInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "call";

    public string FunctionName;
    public string OutputVariableName;
    public int VariadicStart;
    public List<QbeValue> Arguments;
    public IQbeTypeDefinition? FunctionReturnType;

    public CallInstruction(string identifier, string outputVariableName, IQbeTypeDefinition? returnType, int variadicStart, QbeValue[] arguments)
    {
        FunctionName = identifier;
        OutputVariableName = outputVariableName;
        FunctionReturnType = returnType;
        VariadicStart = variadicStart;
        Arguments = arguments.ToList();
    }

    public string Emit(bool is32bit)
    {
        StringBuilder sb = new StringBuilder();
        if (FunctionReturnType != null)
            sb.Append($"%{OutputVariableName} ={FunctionReturnType.ToQbeString(is32bit)} ");
        
        sb.Append($"call {FunctionName}(");
        for (int i = 0; i < Arguments.Count; i++)
        {
            var arg = Arguments[i];
            sb.Append($"{arg.PrimitiveEnum.ToQbeString(is32bit, true)} {arg.GetValue()}");
            
            if (i == VariadicStart - 1 && VariadicStart != Arguments.Count)
                sb.Append(", ...");

            if (i < Arguments.Count - 1)
                sb.Append(", ");
        }
        sb.Append(")");
        return sb.ToString();
    }

    public override string ToString()
    {
        return OutputVariableName;
    }
}