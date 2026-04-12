using System.Collections;
using System.Text;

namespace QbeGenerator;

/// <summary>
/// Represents a named QBE type with a definition (aggregate).
/// </summary>
public class QbeAggregateType : IQbeTypeDefinition
{
    public record Member(IQbeTypeDefinition Type);
    
    public string Identifier { get; }

    /// <summary>
    /// If the struct should align to a certain size. By default, the alignment of an aggregate type is the maximum alignment of its members.
    /// </summary>
    public int Align = 0;

    public QbeAggregateType(string identifier, int align = 0)
    {
        Identifier = identifier;
        Align = align;
    }
    
    private List<Member> _members = new();
    public IReadOnlyList<Member> Members => _members;

    public string ToQbeString(bool is32bit, bool isAggregate = false)
    {
        if (isAggregate)
            return Qbe.TypeDef(Identifier);
        return QbePrimitive.Pointer().ToQbeString(is32bit);
    }

    public bool IsInteger()
    {
        return false;
    }

    public bool IsSignedInteger()
    {
        return false;
    }

    public bool IsFloat()
    {
        return false;
    }

    public long ByteSize(bool is32Bit)
    {
        long size = 0;
        foreach (var member in _members)
        {
            long memberAlign = member.Type.GetAlignment(is32Bit);
            size = (size + memberAlign - 1) / memberAlign * memberAlign;
            size += member.Type.ByteSize(is32Bit);
        }

        // Final padding to overall alignment
        long overallAlign = GetAlignment(is32Bit);
        size = (size + overallAlign - 1) / overallAlign * overallAlign;
        return size;
    }

    public long GetAlignment(bool is32Bit)
    {
        long natural = 1;
        foreach (var member in _members)
        {
            long align = member.Type.GetAlignment(is32Bit);
            if (align > natural) natural = align;
        }
        return Math.Max(natural, Align);
    }

    public void Add(IQbeTypeDefinition fieldType)
    {
        _members.Add(new Member(fieldType));
    }

    public long GetOffset(int fieldIndex, bool is32Bit)
    {
        if (fieldIndex < 0 || fieldIndex >= _members.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(fieldIndex), "Field index is out of range.");
        }

        long offset = 0;
        for (int i = 0; i < fieldIndex; i++)
        {
            var member = _members[i];
            long memberAlign = member.Type.GetAlignment(is32Bit);
            offset = (offset + memberAlign - 1) / memberAlign * memberAlign;
            offset += member.Type.ByteSize(is32Bit);
        }
        return offset;
    }
    
    public string GetVisualRepresentation(bool is32Bit)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Aggregate Type: {Identifier}");
        sb.AppendLine($"Alignment: {GetAlignment(is32Bit)} bytes");
        sb.AppendLine($"Size: {ByteSize(is32Bit)} bytes");
        sb.AppendLine("Members:");
        for (int i = 0; i < _members.Count; i++)
        {
            var member = _members[i];
            sb.AppendLine($"  [{i}] Offset: {GetOffset(i, is32Bit)} bytes, Type: {member.Type.ToQbeString(is32Bit)}");
        }
        return sb.ToString();
    }

    /*
     * Aggregate Types
       
       TYPEDEF :=
           # Regular type
           'type' :IDENT '=' ['align' NUMBER]
           '{'
               ( SUBTY [NUMBER] ),
           '}'
         | # Union type
           'type' :IDENT '=' ['align' NUMBER]
           '{'
               (
                   '{'
                       ( SUBTY [NUMBER] ),
                   '}'
               )+
           '}'
         | # Opaque type
           'type' :IDENT '=' 'align' NUMBER '{' NUMBER '}'
       
       SUBTY := EXTTY | :IDENT
       
     */
    public string Emit(bool is32Bit)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"type {Qbe.TypeDef(Identifier)} = ");
        
        if (_members.Count == 0)
        {
            sb.Append($"align {Align} {{ 0 }}");
        }
        else
        {
            sb.Append($"align {GetAlignment(is32Bit)} {{\n");
            foreach (var member in _members)
            {
                sb.AppendLine($"    {member.Type.ToQbeString(is32Bit)},");
            }
            sb.Append("}");
        }
        
        return sb.ToString();
    }
}