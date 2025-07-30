using System.Text;
using QbeGenerator.Instructions;

namespace QbeGenerator;

public class QbeBlock : IEmit
{
    public string Identifier;
    public List<IQbeInstruction> Instructions;

    private QbeFunction _function;

    public QbeBlock(string identifier, QbeFunction function)
    {
        Identifier = identifier;
        Instructions = new List<IQbeInstruction>();
        _function = function;
    }

    public QbeLocalRef? Call(string identifier, IQbeTypeDefinition? functionReturnType, params QbeValue[] qbeArgument)
    {
        var inst = new CallInstruction(identifier, functionReturnType != null ? _function.GetNextVariableName() : "", functionReturnType, qbeArgument);
        Instructions.Add(inst);
        return functionReturnType != null
            ? new QbeLocalRef(functionReturnType, inst.OutputVariableName)
            : null;
    }

    public void Return(int retValue)
    {
        var inst = new ReturnInstruction(new QbeLiteral(QbePrimitive.Int32, retValue));
        Instructions.Add(inst);
    }

    public void Return(QbeValue value)
    {
        Instructions.Add(new ReturnInstruction(value));
    }

    public void Return(CallInstruction value)
    {
        if (value.FunctionReturnType == null)
            throw new Exception("Tried to return void value from void function.");
        Instructions.Add(new ReturnInstruction(new QbeLocalRef(value.FunctionReturnType!,value.ToString())));
    }

    public void Return()
    {
        Instructions.Add(new ReturnInstruction());
    }

    public string Emit(bool is32bit)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"@{Identifier}");

        foreach (var instruction in Instructions)
        {
            sb.AppendLine($"\t{instruction.Emit(is32bit)}");
        }

        return sb.ToString();
    }

    public QbeLocalRef Load(IQbeTypeDefinition qbePrimitiveEnum, IQbeRef qbeGlobalRef)
    {
        var inst = new LoadInstruciton(qbePrimitiveEnum, qbeGlobalRef, _function.GetNextVariableName());
        Instructions.Add(inst);
        return new QbeLocalRef(qbePrimitiveEnum, inst.OutputVariableName);
    }

    public void Store(QbeValue qbeLocalRef, QbeValue qbeLiteral)
    {
        Instructions.Add(new StoreInstruction(qbeLocalRef, qbeLiteral));
    }

    public QbeLocalRef Allocate(long size, Alignment alignment = Alignment.Four)
    {
        var inst = new AllocInstruction(_function.GetNextVariableName(), size, alignment);
        Instructions.Add(inst);
        return new QbeLocalRef(inst.OutputType, inst.OutputVariableName);
    }

    public QbeLocalRef Allocate(IQbeTypeDefinition primitiveEnum, bool is32Bit)
    {
        var size = primitiveEnum.ByteSize(is32Bit);
        var inst = new AllocInstruction(_function.GetNextVariableName(), size, Alignment.Four);
        Instructions.Add(inst);
        return new QbeLocalRef(primitiveEnum, inst.OutputVariableName);
    }

    public QbeLocalRef Copy(QbeValue p1)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new CopyInstruction(new QbeLocalRef(p1.PrimitiveEnum,identifier), p1));
        return new QbeLocalRef(p1.PrimitiveEnum, identifier);
    }
    
    /// <summary>
    /// Copies p1 to p2.
    /// </summary>
    public void CopyTo(IQbeRef p1, QbeValue p2)
    {
        Instructions.Add(new CopyInstruction(p1, p2));
    }

    public QbeLocalRef Add(QbeValue lhs, QbeValue rhs)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new BaseArithmeticInstruction(identifier, lhs, rhs, QbeBaseArithmeticOperation.Add,
            QbeBaseArithmeticPrimitive.Signed));
        return new QbeLocalRef(lhs.PrimitiveEnum, identifier);
    }
    
    public QbeLocalRef Mul(QbeValue lhs, QbeValue rhs)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new BaseArithmeticInstruction(identifier, lhs, rhs, QbeBaseArithmeticOperation.Multiply,
            QbeBaseArithmeticPrimitive.Signed));
        return new QbeLocalRef(lhs.PrimitiveEnum, identifier);
    }
    
    public QbeLocalRef Div(QbeValue lhs, QbeValue rhs)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new BaseArithmeticInstruction(identifier, lhs, rhs, QbeBaseArithmeticOperation.Divide,
            QbeBaseArithmeticPrimitive.Signed));
        return new QbeLocalRef(lhs.PrimitiveEnum, identifier);
    }
    
    public QbeLocalRef Rem(QbeValue lhs, QbeValue rhs)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new BaseArithmeticInstruction(identifier, lhs, rhs, QbeBaseArithmeticOperation.Remainder,
            QbeBaseArithmeticPrimitive.Signed));
        return new QbeLocalRef(lhs.PrimitiveEnum, identifier);
    }

    public QbeLocalRef Sub(QbeValue lhs, QbeValue rhs)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new BaseArithmeticInstruction(identifier, lhs, rhs, QbeBaseArithmeticOperation.Subtract,
            QbeBaseArithmeticPrimitive.Signed));
        return new QbeLocalRef(lhs.PrimitiveEnum, identifier);
    }
    
    // Only 2 unsigned arithmetic operations. As QBE does not distinguish between signed and unsigned multiplication.
    public QbeLocalRef UDiv(QbeValue lhs, QbeValue rhs)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new BaseArithmeticInstruction(identifier, lhs, rhs, QbeBaseArithmeticOperation.Divide,
            QbeBaseArithmeticPrimitive.Unsigned));
        return new QbeLocalRef(lhs.PrimitiveEnum, identifier);
    }
    
    public QbeLocalRef URem(QbeValue lhs, QbeValue rhs)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new BaseArithmeticInstruction(identifier, lhs, rhs, QbeBaseArithmeticOperation.Remainder,
            QbeBaseArithmeticPrimitive.Unsigned));
        return new QbeLocalRef(lhs.PrimitiveEnum, identifier);
    }
    
    public QbeLocalRef Negate(QbeValue value)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new BaseArithmeticInstruction(identifier, value, null, QbeBaseArithmeticOperation.Negate,
            QbeBaseArithmeticPrimitive.Signed));
        return new QbeLocalRef(value.PrimitiveEnum, identifier);
    }
    
    public QbeLocalRef BitwiseAnd(QbeValue lhs, QbeValue rhs)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new BitArithmeticInstruction(identifier, lhs, rhs, QbeBitArithmeticOperation.And));
        return new QbeLocalRef(lhs.PrimitiveEnum, identifier);
    }
    
    public QbeLocalRef BitwiseOr(QbeValue lhs, QbeValue rhs)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new BitArithmeticInstruction(identifier, lhs, rhs, QbeBitArithmeticOperation.Or));
        return new QbeLocalRef(lhs.PrimitiveEnum, identifier);
    }
    
    public QbeLocalRef BitwiseXor(QbeValue lhs, QbeValue rhs)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new BitArithmeticInstruction(identifier, lhs, rhs, QbeBitArithmeticOperation.Xor));
        return new QbeLocalRef(lhs.PrimitiveEnum, identifier);
    }
    
    public QbeLocalRef ArithmeticShiftLeft(QbeValue lhs, QbeValue rhs)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new BitshiftingInstruction(identifier, lhs, rhs,
            QbeBitshiftingOperation.ArithmeticShiftRight));
        return new QbeLocalRef(lhs.PrimitiveEnum, identifier);
    }
    
    public QbeLocalRef ShiftLeft(QbeValue lhs, QbeValue rhs)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new BitshiftingInstruction(identifier, lhs, rhs, QbeBitshiftingOperation.ShiftLeft));
        return new QbeLocalRef(lhs.PrimitiveEnum, identifier);
    }

    public QbeLocalRef ShiftRight(QbeValue lhs, QbeValue rhs)
    {
        var identifier = _function.GetNextVariableName();
        Instructions.Add(new BitshiftingInstruction(identifier, lhs, rhs, QbeBitshiftingOperation.ShiftRight));
        return new QbeLocalRef(lhs.PrimitiveEnum, identifier);
    }
    
    public QbeLocalRef Convert(QbeValue value, QbeConversionOperation op)
    {
        var outputName = _function.GetNextVariableName();
        Instructions.Add(new ConversionInstruction(outputName, value, op));
        return new QbeLocalRef(value.PrimitiveEnum, outputName);
    }
    
    /// <summary>
    /// The pointer to the first variadic argument.
    /// </summary>
    public void VariadicStart(QbeValue value)
    {
        Instructions.Add(new VariadicStartInstruction(value));
    }

    /// <summary>
    /// Gets the next variadic primitive argument from the variadic arguments.
    /// </summary>
    public QbeLocalRef VariadicArg(QbeValue value, IQbeTypeDefinition primitiveEnum)
    {
        var outputName = _function.GetNextVariableName();
        Instructions.Add(new VariadicArgumentInstruction(value, primitiveEnum, outputName));
        return new QbeLocalRef(primitiveEnum, outputName);
    }
    
    public void JumpIfNotZero(QbeValue lRef, string falseLabel, string trueLabel)
    {
        Instructions.Add(new JumpInstruction(trueLabel, JumpType.Conditional, lRef, falseLabel));
    }

    public QbeValue Equality(EqualityType lessThanOrEqual, EqualityPrimitive equalityPrimitive, QbeValue lRef, QbeValue lit)
    {
        var inst = new EqualityInstruction(_function.GetNextVariableName(),lessThanOrEqual, equalityPrimitive, lRef, lit);
        Instructions.Add(inst);
        return new QbeLocalRef(QbePrimitive.Int32, inst.OutputVariableName);
    }

    /// <summary>
    /// Gets a value from a type definition at a certain index.
    /// For example:
    /// type = { i32, i32, i64 }
    /// instantiated = alloc(type.ByteSize())
    /// i64 lastField = LoadFromType(i64, instantiated, type, 2, false);
    /// </summary>
    public QbeValue LoadFromType(IQbeTypeDefinition targetType, QbeValue address, QbeType typeDefinition, int fieldIndex, bool is32Bit)
    {
        var offset = typeDefinition.GetOffset(fieldIndex, is32Bit);
        if (offset == 0)
        {
            // We can just call a load instruction on the pointer.
            if (address is IQbeRef)
            {
                return Load(targetType, (IQbeRef)address);
            }
            else if (address is QbeLocalRef localRef)
            {
                return Load(targetType, localRef);
            }
            else
            {
                throw new Exception("Tried to load from a pointer that is not a reference or local reference.");
            }
        }
        
        // Add the offset onto the pointer, we can not use the Add instruction here, as it does not support pointers.
        var identifier = _function.GetNextVariableName();
        var addinst = new BaseArithmeticInstruction(identifier, address, Qbe.Lit(QbePrimitive.Pointer, offset),
            QbeBaseArithmeticOperation.Add, QbeBaseArithmeticPrimitive.Signed);
        addinst.OutputType = QbePrimitive.Pointer; // Override the output type to be a pointer. As we are adding an offset to a pointer.
        Instructions.Add(addinst);
        
        // Now we have the local reference identifier.
        var loadInst = new LoadInstruciton(targetType, new QbeLocalRef(QbePrimitive.Pointer, identifier), _function.GetNextVariableName());
        Instructions.Add(loadInst);
        
        return new QbeLocalRef(targetType, loadInst.OutputVariableName);
    }
    
    public QbeValue StoreToType(IQbeTypeDefinition type, QbeValue prt, QbeValue value, QbeType typeDefinition, int idx, bool is32Bit)
    {
        var offset = typeDefinition.GetOffset(idx, is32Bit);
        if (offset == 0)
        {
            // We can just call a store instruction on the pointer.
            if (prt is IQbeRef)
            {
                Store(prt, value);
            }
            else if (prt is QbeLocalRef localRef)
            {
                Store(localRef, value);
            }
            else
            {
                throw new Exception("Tried to store to a pointer that is not a reference or local reference.");
            }
        }
        else
        {
            // Add the offset onto the pointer, we can not use the Add instruction here, as it does not support pointers.
            var identifier = _function.GetNextVariableName();
            var addinst = new BaseArithmeticInstruction(identifier, prt, Qbe.Lit(QbePrimitive.Pointer, offset),
                QbeBaseArithmeticOperation.Add, QbeBaseArithmeticPrimitive.Signed);
            addinst.OutputType = QbePrimitive.Pointer; // Override the output type to be a pointer. As we are adding an offset to a pointer.
            Instructions.Add(addinst);
            
            // Now we have the local reference identifier.
            Store(new QbeLocalRef(QbePrimitive.Pointer, identifier), value);
        }

        return value;
    }

    public QbeValue GetFieldPtr(IQbeTypeDefinition type, QbeValue ptr, QbeType typeDefinition, int idx, bool is32Bit)
    {
        var offset = typeDefinition.GetOffset(idx, is32Bit);
        if (offset == 0)
        {
            // We can just return the pointer.
            if (ptr is IQbeRef)
            {
                return ptr;
            }
            else if (ptr is QbeLocalRef localRef)
            {
                return localRef;
            }
            else
            {
                throw new Exception("Tried to get a field pointer from a pointer that is not a reference or local reference.");
            }
        }
        
        // Add the offset onto the pointer, we can not use the Add instruction here, as it does not support pointers.
        var identifier = _function.GetNextVariableName();
        var addinst = new BaseArithmeticInstruction(identifier, ptr, Qbe.Lit(QbePrimitive.Pointer, offset),
            QbeBaseArithmeticOperation.Add, QbeBaseArithmeticPrimitive.Signed);
        addinst.OutputType = QbePrimitive.Pointer; // Override the output type to be a pointer. As we are adding an offset to a pointer.
        Instructions.Add(addinst);
        
        return new QbeLocalRef(QbePrimitive.Pointer, identifier);
    }
}