using System.Text;

namespace QbeGenerator.Instructions;

public enum EqualityType
{
    Equal,
    Inequal,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual
}

public enum EqualityPrimitive
{
    Int,
    UnsignedInt,
    Float
}

/*
 *  Comparison instructions return an integer value (either a word or a long), and compare values of arbitrary types. The returned value is 1 if the two operands satisfy the comparison relation, or 0 otherwise. The names of comparisons respect a standard naming scheme in three parts.
   
       All comparisons start with the letter c.
   
       Then comes a comparison type. The following types are available for integer comparisons:
           eq for equality
           ne for inequality
           sle for signed lower or equal
           slt for signed lower
           sge for signed greater or equal
           sgt for signed greater
           ule for unsigned lower or equal
           ult for unsigned lower
           uge for unsigned greater or equal
           ugt for unsigned greater 
   
       Floating point comparisons use one of these types:
           eq for equality
           ne for inequality
           le for lower or equal
           lt for lower
           ge for greater or equal
           gt for greater
           o for ordered (no operand is a NaN)
           uo for unordered (at least one operand is a NaN) 
   
       Because floating point types always have a sign bit, all the comparisons available are signed.
       Finally, the instruction name is terminated with a basic type suffix precising the type of the operands to be compared. 
   
   For example, cod (I(dd,dd)) compares two double-precision floating point numbers and returns 1 if the two floating points are not NaNs, or 0 otherwise. The csltw (I(ww,ww)) instruction compares two words representing signed numbers and returns 1 when the first argument is smaller than the second one. 
 */

public class EqualityInstruction : IQbeInstruction
{
    // We won't be using this, because equality depends very much on the types of the operands.
    public string QbeRepresentation { get; } = "eq";

    public EqualityType Type;
    public EqualityPrimitive Primitive;
    public QbeValue Lhs;
    public QbeValue Rhs;
    public string OutputVariableName;
    public EqualityInstruction(string outputVariableName, EqualityType type, EqualityPrimitive primitive, QbeValue lhs, QbeValue rhs)
    {
        Type = type;
        Primitive = primitive;
        Lhs = lhs;
        Rhs = rhs;
        OutputVariableName = outputVariableName;

        if (lhs.PrimitiveEnum != rhs.PrimitiveEnum)
            throw new Exception("Type mismatch between LHS and RHS in equality instruction");
    }
    
    public string Emit(bool is32bit)
    {
        StringBuilder sb = new();
        sb.Append($"%{OutputVariableName} =w "); // It always returns a word (1 or 0)
        
        sb.Append("c");
        if (Primitive == EqualityPrimitive.Int)
        {
            switch (Type)
            {
                case EqualityType.Equal:
                    sb.Append("eq");
                    break;
                case EqualityType.Inequal:
                    sb.Append("ne");
                    break;
                case EqualityType.LessThan:
                    sb.Append("slt");
                    break;
                case EqualityType.LessThanOrEqual:
                    sb.Append("sle");
                    break;
                case EqualityType.GreaterThan:
                    sb.Append("sgt");
                    break;
                case EqualityType.GreaterThanOrEqual:
                    sb.Append("sge");
                    break;
            }
        }
        else if (Primitive == EqualityPrimitive.UnsignedInt)
        {
            switch (Type)
            {
                case EqualityType.Equal:
                    sb.Append("eq");
                    break;
                case EqualityType.Inequal:
                    sb.Append("ne");
                    break;
                case EqualityType.LessThan:
                    sb.Append("ult");
                    break;
                case EqualityType.LessThanOrEqual:
                    sb.Append("ule");
                    break;
                case EqualityType.GreaterThan:
                    sb.Append("ugt");
                    break;
                case EqualityType.GreaterThanOrEqual:
                    sb.Append("uge");
                    break;
            }
        }
        else if (Primitive == EqualityPrimitive.Float)
        {
            switch (Type)
            {
                case EqualityType.Equal:
                    sb.Append("eq");
                    break;
                case EqualityType.Inequal:
                    sb.Append("ne");
                    break;
                case EqualityType.LessThan:
                    sb.Append("lt");
                    break;
                case EqualityType.LessThanOrEqual:
                    sb.Append("le");
                    break;
                case EqualityType.GreaterThan:
                    sb.Append("gt");
                    break;
                case EqualityType.GreaterThanOrEqual:
                    sb.Append("ge");
                    break;
            }
        }
        else
        {
            throw new Exception("Unknown equality primitive type");
        }
        
        // Now we add the type suffix
        sb.Append(Rhs.PrimitiveEnum.ToQbeString(is32bit));
        
        sb.Append($" {Lhs.GetValue()}, {Rhs.GetValue()}");
        return sb.ToString();
    }
}