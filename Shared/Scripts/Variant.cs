namespace CraterSprite;

// https://vladris.com/blog/2018/07/16/implementing-a-variant-type-in-csharp.html

internal interface IVariantHolder
{
    bool Is<T>();

    object Get();
}

internal sealed class VariantHolder<T>(T item) : IVariantHolder
{
    public T item { get; } = item;

    public bool Is<TU>() => typeof(TU) == typeof(T);

    public object Get() => item;

    public override string ToString()
    {
        return item.ToString();
    }
}

public sealed class Variant<T1, T2, T3>
{
    private readonly IVariantHolder _variant;
    
    public readonly int index;

    public bool Is<T>() => _variant.Is<T>();

    public T Get<T>() => ((VariantHolder<T>)_variant).item;

    public object Get() => _variant.Get();

    // T1 constructor, casts
    public Variant(T1 item)
    {
        _variant = new VariantHolder<T1>(item);
        index = 0;
    }

    public static implicit operator Variant<T1, T2, T3>(T1 item) => new(item);

    public static explicit operator T1(Variant<T1, T2, T3> item) => item.Get<T1>();


    public Variant(T2 item)
    {
        _variant = new VariantHolder<T2>(item);
        index = 1;
    }

    public static implicit operator Variant<T1, T2, T3>(T2 item) => new(item);

    public static explicit operator T2(Variant<T1, T2, T3> item) => item.Get<T2>();


    public Variant(T3 item)
    {
        _variant = new VariantHolder<T3>(item);
        index = 2;
    }

    public static implicit operator Variant<T1, T2, T3>(T3 item) => new(item);

    public static explicit operator T3(Variant<T1, T2, T3> item) => item.Get<T3>();
    
    public override bool Equals(object obj)
    {
        if (obj is not Variant<T1, T2, T3> other)
        { 
            return false;
        }
        return index == other.index && Get().Equals(other.Get());
    }
    
    public override int GetHashCode() => Get().GetHashCode();

    public override string ToString()
    {
        return _variant.ToString();
    }
}