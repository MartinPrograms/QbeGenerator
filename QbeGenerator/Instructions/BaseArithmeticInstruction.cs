using System.Text;

namespace QbeGenerator.Instructions;

public enum QbeBaseArithmeticOperation
{
    Add, // add
    Subtract, // sub
    Divide, // div/udiv
    Multiply, // mul
    Remainder, // rem/urem
    Negate
}

public enum QbeBaseArithmeticPrimitive
{
    Signed,
    Unsigned,
}

public class BaseArithmeticInstruction : IQbeInstruction
{
    public string QbeRepresentation { get; } = "arith"; // Unused.

    public IQbeTypeDefinition OutputType;
    public QbeValue Lhs;
    public QbeValue Rhs;
    public QbeBaseArithmeticOperation Operation;
    public QbeBaseArithmeticPrimitive Primitive;
    public string OutputVariable;

    public BaseArithmeticInstruction(string outputVariable, QbeValue lhs, QbeValue rhs, QbeBaseArithmeticOperation operation, QbeBaseArithmeticPrimitive primitive)
    {
        OutputVariable = outputVariable;
        Lhs = lhs;
        OutputType = lhs.PrimitiveEnum;
        Rhs = rhs;
        Operation = operation;
        Primitive = primitive;

        if (Operation != QbeBaseArithmeticOperation.Negate)
        {
            if (!lhs.PrimitiveEnum.IsEqual(rhs.PrimitiveEnum))
            {
                //throw new ArgumentException("LHS and RHS must have the same primitive type.");
            }
        }
    }
    
    public string Emit(bool is32bit)
    {
        StringBuilder sb = new();
        sb.Append($"%{OutputVariable} ={OutputType.ToQbeString(is32bit)} ");

        if (Primitive == QbeBaseArithmeticPrimitive.Signed)
        {
            if (Operation == QbeBaseArithmeticOperation.Add)
            {
                sb.Append("add ");
            }
            else if (Operation == QbeBaseArithmeticOperation.Subtract)
            {
                sb.Append("sub ");
            }
            else if (Operation == QbeBaseArithmeticOperation.Divide)
            {
                sb.Append("div ");
            }
            else if (Operation == QbeBaseArithmeticOperation.Multiply)
            {
                sb.Append("mul ");
            }
            else if (Operation == QbeBaseArithmeticOperation.Remainder)
            {
                sb.Append("rem ");
            }
            else if (Operation == QbeBaseArithmeticOperation.Negate)
            {
                sb.Append("neg ");
            }
            
            if (Operation != QbeBaseArithmeticOperation.Negate)
                sb.Append($"{Lhs.GetValue()}, {Rhs.GetValue()}");
            else
                sb.Append($"{Lhs.GetValue()}");
        }
        else if (Primitive == QbeBaseArithmeticPrimitive.Unsigned)
        {
            if (Operation == QbeBaseArithmeticOperation.Add)
            {
                sb.Append("add ");
            }
            else if (Operation == QbeBaseArithmeticOperation.Subtract)
            {
                sb.Append("sub ");
            }
            else if (Operation == QbeBaseArithmeticOperation.Divide)
            {
                sb.Append("udiv ");
            }
            else if (Operation == QbeBaseArithmeticOperation.Multiply)
            {
                sb.Append("mul ");
            }
            else if (Operation == QbeBaseArithmeticOperation.Remainder)
            {
                sb.Append("urem ");
            }
            else if (Operation == QbeBaseArithmeticOperation.Negate)
            {
                sb.Append("neg ");
            }
            
            if (Operation != QbeBaseArithmeticOperation.Negate)
                sb.Append($"{Lhs.GetValue()}, {Rhs.GetValue()}");
            else
                sb.Append($"{Lhs.GetValue()}");
        }
        
        return sb.ToString();
    }
}