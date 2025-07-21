namespace QbeGenerator;

public class QbeGlobalRef : QbeValue, IQbeRef
{
    public string Identifier;

    public QbeGlobalRef(string identifier)
    {
        PrimitiveEnum = QbePrimitive.Pointer;
        Identifier = identifier;
    }
    
    public override string GetValue()
    {
        return Qbe.GlobalPtr(Identifier);
    }
}