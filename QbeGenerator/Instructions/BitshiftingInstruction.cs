using System.Text;

namespace QbeGenerator.Instructions;

public enum QbeBitshiftingOperation
{
    ArithmeticShiftRight, // sar
    ShiftRight, // shr
    ShiftLeft, // shl
}

public class BitshiftingInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "bitshift";

    public QbeValue Lhs;
    public QbeValue Rhs;
    public QbeBitshiftingOperation Operation;
    public string OutputVariable;
    
    public BitshiftingInstruction(string outputVariable, QbeValue lhs, QbeValue rhs, QbeBitshiftingOperation operation)
    {
        OutputVariable = outputVariable;
        Lhs = lhs;
        Rhs = rhs;

        if (!lhs.PrimitiveEnum.IsInteger() || !rhs.PrimitiveEnum.IsInteger())
        {
            throw new ArgumentException("LHS and RHS must be integer types.");
        }

        Operation = operation;
    }
    
    public string Emit(bool is32bit)
    {
        StringBuilder sb = new();
        sb.Append($"%{OutputVariable} ={Lhs.PrimitiveEnum.ToQbeString(is32bit)} ");

        if (Operation == QbeBitshiftingOperation.ArithmeticShiftRight)
        {
            sb.Append("sar ");
        }
        else if (Operation == QbeBitshiftingOperation.ShiftRight)
        {
            sb.Append("shr ");
        }
        else if (Operation == QbeBitshiftingOperation.ShiftLeft)
        {
            sb.Append("shl ");
        }

        sb.Append($"{Lhs.GetValue()}, {Rhs.GetValue()}");

        return sb.ToString();
    }
}