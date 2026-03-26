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

    public QbeLocalRef? Call(string identifier, IQbeTypeDefinition? functionReturnType, int variadicStart, params QbeValue[] qbeArgument)
    {
        var inst = new CallInstruction($"${identifier}", functionReturnType != null ? _function.GetNextVariableName() : "", functionReturnType, variadicStart, qbeArgument);
        Instructions.Add(inst);
        return functionReturnType != null
            ? new QbeLocalRef(functionReturnType, inst.OutputVariableName)
            : null;
    }

    public void Return(IQbeRef value)
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
    /// Copies p2 to p1
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
    
    public void JumpIfNotZero(QbeValue lRef, QbeBlock falseBlock, QbeBlock trueBlock)
    {
        Instructions.Add(new JumpInstruction(trueBlock.Identifier, JumpType.Conditional, lRef, falseBlock.Identifier));
    }

    public QbeValue Equality(EqualityType equality, EqualityPrimitive equalityPrimitive, QbeValue left, QbeValue right)
    {
        var inst = new EqualityInstruction(_function.GetNextVariableName(),equality, equalityPrimitive, left, right);
        Instructions.Add(inst);
        return new QbeLocalRef(QbePrimitive.Int32(false), inst.OutputVariableName);
    }

    public QbeValue Equality(EqualityType equality, QbeValue left, QbeValue right)
    {
        EqualityPrimitive primitive;
        if (left.PrimitiveEnum.IsFloat() && right.PrimitiveEnum.IsFloat())
            primitive = EqualityPrimitive.Float;
        else if (left.PrimitiveEnum.IsSignedInteger() && right.PrimitiveEnum.IsSignedInteger())
            primitive = EqualityPrimitive.Int;
        else if (left.PrimitiveEnum.IsInteger() && right.PrimitiveEnum.IsInteger())
            primitive = EqualityPrimitive.UnsignedInt;
        else 
            throw new Exception("Tried to compare two values with an unsupported primitive type for equality. Left: " + left.PrimitiveEnum + " Right: " + right.PrimitiveEnum);
        
        var inst = new EqualityInstruction(_function.GetNextVariableName(), equality, primitive, left, right);
        Instructions.Add(inst);
        return new QbeLocalRef(QbePrimitive.Int32(false), inst.OutputVariableName);
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
        var addinst = new BaseArithmeticInstruction(identifier, address, Qbe.Lit(QbePrimitive.Pointer(), offset),
            QbeBaseArithmeticOperation.Add, QbeBaseArithmeticPrimitive.Signed);
        addinst.OutputType = QbePrimitive.Pointer(); // Override the output type to be a pointer. As we are adding an offset to a pointer.
        Instructions.Add(addinst);
        
        // Now we have the local reference identifier.
        var loadInst = new LoadInstruciton(targetType, new QbeLocalRef(QbePrimitive.Pointer(), identifier), _function.GetNextVariableName());
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
            var addinst = new BaseArithmeticInstruction(identifier, prt, Qbe.Lit(QbePrimitive.Pointer(), offset),
                QbeBaseArithmeticOperation.Add, QbeBaseArithmeticPrimitive.Unsigned);
            addinst.OutputType = QbePrimitive.Pointer(); // Override the output type to be a pointer. As we are adding an offset to a pointer.
            Instructions.Add(addinst);
            
            // Now we have the local reference identifier.
            Store(new QbeLocalRef(QbePrimitive.Pointer(), identifier), value);
        }

        return value;
    }

    public QbeValue GetFieldPtr(QbeValue ptr, QbeType typeDefinition, int idx, bool is32Bit)
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
        var addinst = new BaseArithmeticInstruction(identifier, ptr, Qbe.Lit(QbePrimitive.Pointer(), offset),
            QbeBaseArithmeticOperation.Add, QbeBaseArithmeticPrimitive.Signed);
        addinst.OutputType = QbePrimitive.Pointer(); // Override the output type to be a pointer. As we are adding an offset to a pointer.
        Instructions.Add(addinst);
        
        return new QbeLocalRef(QbePrimitive.Pointer(), identifier);
    }

    public bool HasTerminator()
    {
        // Check if there is an unconditional jump, or a return instruction as the last instruction in the block.
        if (Instructions.Count == 0)             return false;
        var lastInst = Instructions[Instructions.Count - 1];
        return lastInst is JumpInstruction ||
               lastInst is ReturnInstruction;
    }

    public void Jump(QbeBlock peek)
    {
        Instructions.Add(new JumpInstruction(peek.Identifier, JumpType.Unconditional));
    }

    // Convert from one primitive type to another. For example from i32 to f32.
    // This function automatically determines the correct conversion operation to use based on the source, and target! When it fails it bitcasts as a fallback!
    
    /*public enum QbeConversionOperation
       {
           ExtendSignedWord, // extsw
           ExtendUnsignedWord, // extuw
           ExtendSignedHalf, // extsh
           ExtendUnsignedHalf, // extuh
           ExtendSignedByte, // extsb
           ExtendUnsignedByte, // extub
           ExtendSingle, // exts
           TruncateDouble, // truncd
           SingleToSignedInt, // stosi
           SingleToUnsignedInt, // stoui
           DoubleToSignedInt, // dtosi
           DoubleToUnsignedInt, // dtoui
           SignedWordToFloat, // swtof
           UnsignedWordToFloat, // uwtof
           SignedLongToFloat, // sltof
           UnsignedLongToFloat // ultof
       } 
    */
    public QbeValue Convert(QbeValue toConvert, IQbeTypeDefinition targetType)
    {
        if (toConvert.PrimitiveEnum.IsEqual(targetType))
            return toConvert; // No conversion needed.
        
        QbeConversionOperation op = QbeConversionOperation.ExtendSingle; // Default value, will be overridden before use.
        
        bool isExtension = toConvert.PrimitiveEnum.ByteSize(false) < targetType.ByteSize(false);
        bool isFloatingPoint = toConvert.PrimitiveEnum.IsFloat() && targetType.IsFloat();
        bool isIntegerToFloat = toConvert.PrimitiveEnum.IsInteger() && targetType.IsFloat();
        bool isFloatToInteger = toConvert.PrimitiveEnum.IsFloat() && targetType.IsInteger();
        
        if (isExtension)
        {
            if (toConvert.PrimitiveEnum.IsSignedInteger() && targetType.IsSignedInteger())
                op = toConvert.PrimitiveEnum.ByteSize(false) switch
                {
                    1 => QbeConversionOperation.ExtendSignedByte,
                    2 => QbeConversionOperation.ExtendSignedHalf,
                    4 => QbeConversionOperation.ExtendSignedWord,
                    _ => throw new Exception("Unsupported byte size for extension: " + toConvert.PrimitiveEnum.ByteSize(false))
                };
            else if (toConvert.PrimitiveEnum.IsInteger() && targetType.IsInteger())
                op = toConvert.PrimitiveEnum.ByteSize(false) switch
                {
                    1 => QbeConversionOperation.ExtendUnsignedByte,
                    2 => QbeConversionOperation.ExtendUnsignedHalf,
                    4 => QbeConversionOperation.ExtendUnsignedWord,
                    _ => throw new Exception("Unsupported byte size for extension: " + toConvert.PrimitiveEnum.ByteSize(false))
                };
            else if (isFloatingPoint)
                op = QbeConversionOperation.ExtendSingle;
            else
                throw new Exception($"Unsupported extension from {toConvert.PrimitiveEnum} to {targetType}");
        }
        else if (isFloatToInteger)
        {
            if (targetType.IsSignedInteger())
                op = toConvert.PrimitiveEnum.ByteSize(false) switch
                {
                    4 when targetType.ByteSize(false) == 4 => QbeConversionOperation.SingleToSignedInt,
                    8 when targetType.ByteSize(false) == 4 => QbeConversionOperation.DoubleToSignedInt,
                    4 when targetType.ByteSize(false) == 8 => QbeConversionOperation.SingleToSignedInt,
                    8 when targetType.ByteSize(false) == 8 => QbeConversionOperation.DoubleToSignedInt,
                    _ => throw new Exception("Unsupported float to int conversion from " + toConvert.PrimitiveEnum +
                                             " to " + targetType)
                };
            else if (targetType.IsInteger())
                op = toConvert.PrimitiveEnum.ByteSize(false) switch
                {
                    4 when targetType.ByteSize(false) == 4 => QbeConversionOperation.SingleToUnsignedInt,
                    8 when targetType.ByteSize(false) == 4 => QbeConversionOperation.DoubleToUnsignedInt,
                    4 when targetType.ByteSize(false) == 8 => QbeConversionOperation.SingleToUnsignedInt,
                    8 when targetType.ByteSize(false) == 8 => QbeConversionOperation.DoubleToUnsignedInt,
                    _ => throw new Exception("Unsupported float to int conversion from " + toConvert.PrimitiveEnum +
                                             " to " + targetType)
                };
        }
        else if (isIntegerToFloat)
        {
            if (toConvert.PrimitiveEnum.IsSignedInteger())
                op = toConvert.PrimitiveEnum.ByteSize(false) switch
                {
                    4 when targetType.ByteSize(false) == 4 => QbeConversionOperation.SignedWordToFloat,
                    8 when targetType.ByteSize(false) == 4 => QbeConversionOperation.SignedLongToFloat,
                    4 when targetType.ByteSize(false) == 8 => QbeConversionOperation.SignedWordToFloat,
                    8 when targetType.ByteSize(false) == 8 => QbeConversionOperation.SignedLongToFloat,
                    _ => throw new Exception("Unsupported int to float conversion from " + toConvert.PrimitiveEnum +
                                             " to " + targetType)
                };
            else if (toConvert.PrimitiveEnum.IsInteger())
                op = toConvert.PrimitiveEnum.ByteSize(false) switch
                {
                    4 when targetType.ByteSize(false) == 4 => QbeConversionOperation.UnsignedWordToFloat,
                    8 when targetType.ByteSize(false) == 4 => QbeConversionOperation.UnsignedLongToFloat,
                    4 when targetType.ByteSize(false) == 8 => QbeConversionOperation.UnsignedWordToFloat,
                    8 when targetType.ByteSize(false) == 8 => QbeConversionOperation.UnsignedLongToFloat,
                    _ => throw new Exception("Unsupported int to float conversion from " + toConvert.PrimitiveEnum +
                                             " to " + targetType)
                };
        }
        else if (isFloatingPoint)
        {
            op = QbeConversionOperation.ExtendSingle;
        }
        else if (toConvert.PrimitiveEnum.ByteSize(false) > targetType.ByteSize(false))
        {
            op = QbeConversionOperation.TruncateDouble;
        }
        else
        {
            // We can not convert between these two types, try bitcasting as a fallback.
            var bitcasted = Bitcast(toConvert, targetType);
            bitcasted.PrimitiveEnum = targetType;
            return bitcasted;
        }

        var converted = Convert(toConvert, op);
        converted.PrimitiveEnum = targetType;
        return converted;
    }
    
    public QbeValue Bitcast(QbeValue toConvert, IQbeTypeDefinition targetType)
    {
        var output = new QbeLocalRef(targetType, _function.GetNextVariableName());
        Instructions.Add(new CastInstruction(toConvert, targetType, output.Identifier));
        return output;
    }
}