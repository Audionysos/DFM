using S = audionysos.gui.layout.Size;
using System.Numerics;
using static System.Math;
using audionysos.math;

namespace audionysos.gui.layout;

public struct Size
{
    public static readonly S ZERO = 0;
    public static readonly S AUTO = double.NaN;
    public static readonly S STRETCH = double.PositiveInfinity;

    public static S Relative<N>(N v) where N : INumber<N>
        => new S(double.CreateChecked(v)).relative();

    public S relative() => value > 0 ? new S(-value) : this;

    public double value = double.NaN;
    public bool isRelative => value < 0;

    public Size(double value)
    {
        this.value = value;
    }

    public S min(S other)
    {
        //TODO: Consider other type of size
        if (value.NaN()) return this;
        if (value > 0) return Min(value, other.value);
        return Max(value, other.value);
    }

    public S max(S other)
    {
        //TODO: Consider other type of size
        if (value.NaN()) return this;
        if (value > 0) return Max(value, other.value);
        return Min(value, other.value);
    }

    public static S operator *(S s, double v)
        => s.value * v;

    public static implicit operator S(double v)
        => new S(v);

    public static implicit operator double(S s)
        => s.value;

    public override string ToString()
        => $"{value}";
}
