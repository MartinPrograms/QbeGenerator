using QbeGenerator.Instructions;

namespace QbeGenerator;

/// <summary>
/// Helper class for sigils and other static stuff.
/// </summary>
public static class Qbe
{
    public static string GlobalPtr(string identifier)
    {
        return $"${identifier}";
    }

    public static string Var(string identifier)
    {
        return $"%{identifier}";
    }

    /// <summary>
    /// Returns the variable created by this instruction
    /// </summary>
    public static string Var(CallInstruction instruction)
    {
        return instruction.OutputVariableName;
    }

    public static string TypeDef(string identifier)
    {
        return $":{identifier}";
    }

    public static QbeLocalRef LRef(IQbeTypeDefinition type, string name) => new QbeLocalRef(type, name);
    public static QbeGlobalRef GRef(string name) => new QbeGlobalRef(name);
    public static QbeLiteral Lit(IQbeTypeDefinition type, long value) => new QbeLiteral(type, value);
    public static QbeLiteral Lit(IQbeTypeDefinition type, double value) => new QbeLiteral(type, value);

    public static string Label(string conditionIdentifier)
    {
        return $"@{conditionIdentifier}";
    }
}