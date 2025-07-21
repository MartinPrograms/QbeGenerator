namespace QbeGenerator;

public class QbeGlobal : IEmit
{
    public string Identifier;
    private QbeValue _literal;
    private bool _isString = false;
    private string _stringValue;
    
    public QbeGlobal(string identifier, IQbeTypeDefinition primitiveEnum, long intValue)
    {
        Identifier = identifier;
        _literal = new QbeLiteral(primitiveEnum, intValue);
    }
    
    public QbeGlobal(string identifier, IQbeTypeDefinition primitiveEnum, double floatValue)
    {
        Identifier = identifier;
        _literal = new QbeLiteral(primitiveEnum, floatValue);
    }
    
    public QbeGlobal(string identifier, string stringValue)
    {
        Identifier = identifier;
        _isString = true;
        _stringValue = stringValue;
    }

    public string Emit(bool is32bit)
    {
        if (_isString)
        {
            return $"data ${Identifier} = {{ b \"{_stringValue}\", b 0 }}";
        }

        return $"data ${Identifier} = {{ {_literal.PrimitiveEnum.ToQbeString(is32bit)} {_literal.GetValue()} }}";
    }
}