using System.Text;

namespace QbeGenerator;

/// <summary>
/// Represents a named QBE type with a definition (aggregate).
/// </summary>
public class QbeType : IEmit, IQbeTypeDefinition
{
    public string Identifier { get; }
    
    /// <summary>
    /// If the struct should align to a certain size. By default, the alignment of an aggregate type is the maximum alignment of its members.
    /// </summary>
    public int Align = 0;

    private QbeTypeDefinition _definition;

    public QbeType(string identifier)
    {
        Identifier = identifier;
        _definition = new QbeTypeDefinition();
    }

    public void Add(IQbeTypeDefinition type)
    {
        _definition.Add(type);
    }

    public void Add(QbeTypeDefinition def)
    {
        foreach (var item in def.TypeDefinitions)
        {
            if (item.Primitive != null)
                Add(item.Primitive);
            else if (item.UnionItem != null)
                Add(item.UnionItem);
            else if (item.RefStruct != null)
                AddRef(item.RefStruct);
        }
    }

    public void AddUnion(params QbeTypeDefinition[] definitions)
    {
        _definition.AddUnion(definitions);
    }

    public void Add(QbeType type)
    {
        if (type == this)
            throw new Exception("Tried adding QbeType to itself");
        foreach (var item in type._definition.TypeDefinitions)
        {
            if (item.Primitive != null)
                Add(item.Primitive);
            else if (item.UnionItem != null)
                Add(item.UnionItem);
            else if (item.RefStruct != null)
                AddRef(item.RefStruct);
        }
    }

    /// <summary>
    /// Adds a reference to another named type, without flattening.
    /// </summary>
    public void AddRef(QbeType type)
    {
        if (type == this)
            throw new Exception("Tried adding QbeType to itself");
        _definition.AddRef(type);
    }

    public QbeTypeDefinition GetDefinition() => _definition;

    public string Emit(bool is32Bit)
    {
        var def = GetDefinition();

        StringBuilder sb = new();
        sb.Append($"type :{Identifier} = ");
        
        if (Align > 0)
        {
            sb.Append($"align {Align} ");
        }

        sb.Append(def.GetValue(is32Bit));

        return sb.ToString();
    }

    public long GetOffset(int i, bool is32Bit)
    {
        long offset = 0;
        for (int j = 0; j < i; j++)
        {
            var item = _definition.TypeDefinitions[j];
            if (item.Primitive != null)
            {
                offset += item.Primitive.ByteSize(is32Bit);
            }
            else if (item.UnionItem != null)
            {
                offset += item.UnionItem.GetSize(is32Bit);
            }
            else if (item.RefStruct != null)
            {
                offset += item.RefStruct.GetSize(is32Bit);
            }
        }
        
        // Align the offset to 4 or 8 bytes, depending on the architecture.
        if (is32Bit)
        {
            offset = (offset + 3) & ~3; // Align to 4 bytes
        }
        else
        {
            offset = (offset + 7) & ~7; // Align to 8 bytes
        }
        
        return offset;
    }

    public long GetSize(bool is32Bit)
    {
        long size = 0;
        foreach (var item in _definition.TypeDefinitions)
        {
            if (item.Primitive != null)
            {
                size += item.Primitive.ByteSize(is32Bit);
            }
            else if (item.UnionItem != null)
            {
                size += item.UnionItem.GetSize(is32Bit);
            }
            else if (item.RefStruct != null)
            {
                size += item.RefStruct.GetSize(is32Bit);
            }
        }
        return size;
    }

    public string ToQbeString(bool is32Bit)
    {
        return Qbe.TypeDef(Identifier);
    }

    public bool IsInteger()
    {
        return false;
    }

    public bool IsFloat()
    {
        return false;
    }

    public long ByteSize(bool is32Bit)
    {
        return GetSize(is32Bit);
    }
}

public record QbeTypeDefInternal(IQbeTypeDefinition? Primitive, QbeTypeDefinition? UnionItem, QbeType? RefStruct)
{
    public string GetValue(bool is32bit)
    {
        if (Primitive != null)
            return Primitive.ToQbeString(is32bit);
        if (UnionItem != null)
            return UnionItem.GetValue(is32bit);
        if (RefStruct != null)
            return Qbe.TypeDef(RefStruct.Identifier);
        throw new Exception("All values are null!");
    }
}

public class QbeTypeDefinition
{
    internal List<QbeTypeDefInternal> TypeDefinitions { get; } = new();
    
    public IReadOnlyList<QbeTypeDefInternal> Definitions => TypeDefinitions;

    public void Add(IQbeTypeDefinition prim)
    {
        TypeDefinitions.Add(new QbeTypeDefInternal(prim, null, null));
    }

    public void Add(QbeTypeDefinition nested)
    {
        TypeDefinitions.Add(new QbeTypeDefInternal(null, nested, null));
    }

    public void Add(QbeType toNest)
    {
        TypeDefinitions.Add(new QbeTypeDefInternal(null, toNest.GetDefinition(), null));
    }

    public void AddRef(QbeType refName)
    {
        TypeDefinitions.Add(new QbeTypeDefInternal(null, null, refName));
    }

    public string GetValue(bool is32bit)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{ ");

        foreach (var typedef in TypeDefinitions)
        {
            var value = typedef.GetValue(is32bit);
            sb.Append(value);
            if (typedef.UnionItem == null)
                sb.Append(", ");
            
        }

        sb.Append("} ");
        
        return sb.ToString();
    }

    public void AddUnion(QbeTypeDefinition[] definitions)
    {
        foreach(var def in definitions)
            Add(def);
    }
    
    public long GetSize(bool is32Bit)
    {
        long size = 0;
        foreach (var item in TypeDefinitions)
        {
            if (item.Primitive != null)
            {
                size += item.Primitive.ByteSize(is32Bit);
            }
            else if (item.UnionItem != null)
            {
                size += item.UnionItem.GetSize(is32Bit);
            }
            else if (item.RefStruct != null)
            {
                // For a reference, we assume the size is 4 bytes (32-bit) or 8 bytes (64-bit).
                size += is32Bit ? 4 : 8;
            }
        }
        return size;
    }
}