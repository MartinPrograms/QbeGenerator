using System.Text;

namespace QbeGenerator;

public class QbeFunction
{
    /// <summary>
    /// The function name
    /// </summary>
    public string Identifier;

    /// <summary>
    /// The function flags.
    /// </summary>
    public QbeFunctionFlags Flags;

    /// <summary>
    /// The return type. Null is equal to void! If a function is null, and you pass in a value to return an error will be thrown.
    /// </summary>
    public IQbeTypeDefinition? ReturnType;

    public List<QbeArgument> Arguments;
    
    public bool VarArgs;

    private List<QbeBlock> _blocks;

    public QbeFunction(string identifier, QbeFunctionFlags flags, IQbeTypeDefinition? retType, bool varargs, QbeArgument[]? arguments)
    {
        Identifier = identifier;
        Flags = flags;
        ReturnType = retType;
        VarArgs = varargs;
        Arguments = arguments?.ToList() ?? new List<QbeArgument>();
        _blocks = new List<QbeBlock>();
    }

    public QbeBlock BuildEntryBlock()
    {
        var block = new QbeBlock("start", this);
        _blocks.Add(block);
        return block;
    }

    
    private long _variableCounter = 0;
    public string GetNextVariableName()
    {
        return $"local_var{_variableCounter++}";
    }

    public string Emit(bool is32Bit)
    {
        List<string> flags = new();
        if (Flags != QbeFunctionFlags.None)
        {
            if (Flags.HasFlag(QbeFunctionFlags.Export))
                flags.Add("export");
            if (Flags.HasFlag(QbeFunctionFlags.Thread))
                flags.Add("thread");
        }

        string flagsStr = string.Join(" ", flags);
        flagsStr = string.IsNullOrEmpty(flagsStr) ? string.Empty : $"{flagsStr} ";

        string varArgsStr = VarArgs ? "..." : string.Empty;
        if (Arguments.Count > 0)
        {
            varArgsStr = VarArgs ? ", ..." : string.Empty;
        }
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(
            $"{flagsStr}function {(ReturnType != null ? ReturnType!.ToQbeString(is32Bit) + " " : string.Empty)}${Identifier}({string.Join(", ", Arguments.Select(x => x.FullDefinition(is32Bit)))}{varArgsStr}) {{");

        foreach (var block in _blocks)
        {
            sb.AppendLine(block.Emit(is32Bit));
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    public QbeBlock BuildBlock(string label)
    {
        var block = new QbeBlock(label, this);
        _blocks.Add(block);
        return block;
    }
}