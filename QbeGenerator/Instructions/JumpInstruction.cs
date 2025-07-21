namespace QbeGenerator.Instructions;

public enum JumpType
{
    Unconditional,
    Conditional,
    Halt
}
/*
 * JUMP :=
     'jmp' @IDENT               # Unconditional
   | 'jnz' VAL, @IDENT, @IDENT  # Conditional
   | 'ret' [VAL]                # Return
   | 'hlt'                      # Termination
 */

public class JumpInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "jmp";

    public JumpType Type;
    public string Identifier;
    public string ConditionIdentifier;
    public QbeValue ConditionValue;
    
    public JumpInstruction(string identifier, JumpType type = JumpType.Unconditional, QbeValue conditionValue = null, string conditionIdentifier = null)
    {
        Identifier = identifier;
        Type = type;
        ConditionValue = conditionValue;
        ConditionIdentifier = conditionIdentifier;

        if (type == JumpType.Conditional && (conditionValue == null || conditionIdentifier == null))
            throw new ArgumentException("Conditional jump must have a value and an identifier");
    }
    
    public string Emit(bool is32bit)
    {
        switch (Type)
        {
            case JumpType.Unconditional:
                return $"{QbeRepresentation} {Qbe.Label(Identifier)}";
            case JumpType.Conditional:
                return $"jnz {ConditionValue.GetValue()}, {Qbe.Label(ConditionIdentifier)}, {Qbe.Label(Identifier)}";
            case JumpType.Halt:
                return "hlt";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}