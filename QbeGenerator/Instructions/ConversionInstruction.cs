namespace QbeGenerator.Instructions;

/*
 * Conversions
   
   Conversion operations change the representation of a value, possibly modifying it if the target type cannot hold the value of the source type. Conversions can extend the precision of a temporary (e.g., from signed 8-bit to 32-bit), or convert a floating point into an integer and vice versa.
   
       extsw, extuw -- l(w)
       extsh, extuh -- I(ww)
       extsb, extub -- I(ww)
       exts -- d(s)
       truncd -- s(d)
       stosi -- I(ss)
       stoui -- I(ss)
       dtosi -- I(dd)
       dtoui -- I(dd)
       swtof -- F(ww)
       uwtof -- F(ww)
       sltof -- F(ll)
       ultof -- F(ll) 
   
   Extending the precision of a temporary is done using the ext family of instructions. Because QBE types do not specify the signedness (like in LLVM), extension instructions exist to sign-extend and zero-extend a value. For example, extsb takes a word argument and sign-extends the 8 least-significant bits to a full word or long, depending on the return type.
   
   The instructions exts (extend single) and truncd (truncate double) are provided to change the precision of a floating point value. When the double argument of truncd cannot be represented as a single-precision floating point, it is truncated towards zero.
   
   Converting between signed integers and floating points is done using stosi (single to signed integer), stoui (single to unsigned integer, dtosi (double to signed integer), dtoui (double to unsigned integer), swtof (signed word to float), uwtof (unsigned word to float), sltof (signed long to float) and ultof (unsigned long to float).
   
   Because of Subtyping, there is no need to have an instruction to lower the precision of an integer temporary. 
 */

public enum QbeConversionOperation
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

public class ConversionInstruction : IQbeInstruction
{
    private static Dictionary<QbeConversionOperation, string> _operationToQbeString = new()
    {
        { QbeConversionOperation.ExtendSignedWord, "extsw" },
        { QbeConversionOperation.ExtendUnsignedWord, "extuw" },
        { QbeConversionOperation.ExtendSignedHalf, "extsh" },
        { QbeConversionOperation.ExtendUnsignedHalf, "extuh" },
        { QbeConversionOperation.ExtendSignedByte, "extsb" },
        { QbeConversionOperation.ExtendUnsignedByte, "extub" },
        { QbeConversionOperation.ExtendSingle, "exts" },
        { QbeConversionOperation.TruncateDouble, "truncd" },
        { QbeConversionOperation.SingleToSignedInt, "stosi" },
        { QbeConversionOperation.SingleToUnsignedInt, "stoui" },
        { QbeConversionOperation.DoubleToSignedInt, "dtosi" },
        { QbeConversionOperation.DoubleToUnsignedInt, "dtoui" },
        { QbeConversionOperation.SignedWordToFloat, "swtof" },
        { QbeConversionOperation.UnsignedWordToFloat, "uwtof" },
        { QbeConversionOperation.SignedLongToFloat, "sltof" },
        { QbeConversionOperation.UnsignedLongToFloat, "ultof" }
    };

    private static Dictionary<QbeConversionOperation, IQbeTypeDefinition> _resultTypeMap = new()
    {
        { QbeConversionOperation.ExtendSignedWord, QbePrimitive.Int64(true) },
        { QbeConversionOperation.ExtendUnsignedWord, QbePrimitive.Int64(false) },
        { QbeConversionOperation.ExtendSignedHalf, QbePrimitive.Int32(true) },
        { QbeConversionOperation.ExtendUnsignedHalf, QbePrimitive.Int32(false) },
        { QbeConversionOperation.ExtendSignedByte, QbePrimitive.Int32(true) },
        { QbeConversionOperation.ExtendUnsignedByte, QbePrimitive.Int32(false) },
        { QbeConversionOperation.ExtendSingle, QbePrimitive.Double() },
        { QbeConversionOperation.TruncateDouble, QbePrimitive.Float() },
        { QbeConversionOperation.SingleToSignedInt, QbePrimitive.Int32(true) },
        { QbeConversionOperation.SingleToUnsignedInt, QbePrimitive.Int32(false) },
        { QbeConversionOperation.DoubleToSignedInt, QbePrimitive.Int64(true) },
        { QbeConversionOperation.DoubleToUnsignedInt, QbePrimitive.Int64(false) },
        { QbeConversionOperation.SignedWordToFloat, QbePrimitive.Float() },
        { QbeConversionOperation.UnsignedWordToFloat, QbePrimitive.Float() },
        { QbeConversionOperation.SignedLongToFloat, QbePrimitive.Float() },
        { QbeConversionOperation.UnsignedLongToFloat, QbePrimitive.Float() }
    };
    
    public string QbeRepresentation { get; } = "conversion";

    public QbeValue InputValue;
    public QbeConversionOperation Operation;
    public string OutputVariable;

    public ConversionInstruction(string outputVariable, QbeValue inputValue, QbeConversionOperation operation)
    {
        OutputVariable = outputVariable;
        InputValue = inputValue;
        Operation = operation;

        if (!inputValue.PrimitiveEnum.IsInteger() && !inputValue.PrimitiveEnum.IsFloat())
        {
            throw new ArgumentException("Input value must be an integer or float type.");
        }
    }

    public string Emit(bool is32bit)
    {
        return $"%{OutputVariable} ={_resultTypeMap[Operation].ToQbeString(is32bit)} {_operationToQbeString[Operation]} {InputValue.GetValue()}";
    }
}