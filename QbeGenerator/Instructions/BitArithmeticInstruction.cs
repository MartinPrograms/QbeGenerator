using System.Text;

namespace QbeGenerator.Instructions;

public enum QbeBitArithmeticOperation
{
    And, // and
    Or, // or
    Xor, // xor
}

public class BitArithmeticInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "bitarith";

    public QbeValue Lhs;
    public QbeValue Rhs;
    public QbeBitArithmeticOperation Operation;
    public string OutputVariable;
    
    public BitArithmeticInstruction(string outputVariable, QbeValue lhs, QbeValue rhs, QbeBitArithmeticOperation operation)
    {
        OutputVariable = outputVariable;
        Lhs = lhs;
        Rhs = rhs;
        
        if (lhs.PrimitiveEnum != rhs.PrimitiveEnum)
        {
            throw new ArgumentException("LHS and RHS must have the same primitive type.");
        }
        
        Operation = operation;
    }
    
    public string Emit(bool is32bit)
    {
        StringBuilder sb = new();
        sb.Append($"%{OutputVariable} ={Lhs.PrimitiveEnum.ToQbeString(is32bit)} ");

        if (Operation == QbeBitArithmeticOperation.And)
        {
            sb.Append("and ");
        }
        else if (Operation == QbeBitArithmeticOperation.Or)
        {
            sb.Append("or ");
        }
        else if (Operation == QbeBitArithmeticOperation.Xor)
        {
            sb.Append("xor ");
        }
        
        sb.Append($"{Lhs.GetValue()}, {Rhs.GetValue()}");
        
        return sb.ToString();
    }
}