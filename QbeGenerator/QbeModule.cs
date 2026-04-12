using System.Data;
using System.Text;

namespace QbeGenerator;

public class QbeModule
{
    private List<QbeAggregateType> _types;
    public IReadOnlyList<QbeAggregateType> Types => _types;
    
    private List<QbeGlobal> _globals;
    private List<QbeFunction> _functions;
    public bool Is32Bit = false;

    public QbeModule(bool is32bit = false)
    {
        Is32Bit = is32bit;

        _types = new List<QbeAggregateType>();
        _globals = new List<QbeGlobal>();
        _functions = new List<QbeFunction>();
    }

    public QbeFunction AddFunction(string identifier, QbeFunctionFlags flags, IQbeTypeDefinition? retType, bool isVarArg = false,
        params QbeArgument[]? args)
    {
        var func = (new QbeFunction(identifier, flags, retType, isVarArg, args));
        _functions.Add(func);
        return func;
    }

    public QbeGlobalRef AddGlobal(string content)
    {
        string identifier = GetNextIdentifier();
        _globals.Add(new QbeGlobal(identifier, content));
        
        return new QbeGlobalRef(identifier);
    }

    public QbeGlobalRef AddGlobal(IQbeTypeDefinition primitiveEnum, long intValue)
    {
        string identifier = GetNextIdentifier();
        _globals.Add(new QbeGlobal(identifier, primitiveEnum, intValue));
        return new QbeGlobalRef(identifier);
    }

    public QbeGlobalRef AddGlobal(IQbeTypeDefinition primitiveEnum, double floatValue)
    {
        string identifier = GetNextIdentifier();
        _globals.Add(new QbeGlobal(identifier, primitiveEnum, floatValue));
        return new QbeGlobalRef(identifier);
    }

    public string Emit()
    {
        StringBuilder sb = new StringBuilder();
        
        QbeTypeSorter typeSorter = new QbeTypeSorter();
        foreach (var type in _types)
        {
            typeSorter.AddType(type);
        }
        
        var sorted = typeSorter.SortTypes();
        foreach (var type in sorted)
        {
            sb.AppendLine(type.Emit(Is32Bit));
        }

        foreach (var global in _globals)
        {
            sb.AppendLine(global.Emit(Is32Bit));
        }

        foreach (var function in _functions)
        {
            sb.AppendLine(function.Emit(Is32Bit));
        }

        sb.AppendLine("# QbeGenerator - k");
        sb.AppendLine($"# {DateTime.Now.ToLongTimeString()}");

        return sb.ToString();
    }

    public bool HasMainFunction()
    {
        return _functions.Exists(x => x.Identifier == "main" && x.Flags.HasFlag(QbeFunctionFlags.Export));
    }

    public QbeAggregateType AddType(string identifier)
    {
        QbeAggregateType aggregateType = new(identifier);
        _types.Add(aggregateType);
        return aggregateType;
    }
    
    public QbeAggregateType AddType(QbeAggregateType aggregateType)
    {
        _types.Add(aggregateType);
        return aggregateType;
    }
    
    private long _identifierCounter = 0;
    public string GetNextIdentifier()
    {
        return $"global_var{_identifierCounter++}";
    }

    public bool HasFunction(string qbeIdentifier) => _functions.Any(f => f.Identifier == qbeIdentifier);
}