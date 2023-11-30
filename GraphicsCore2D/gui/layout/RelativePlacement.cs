using P = audionysos.gui.layout.RelativePlacement;
using audionysos.math;

namespace audionysos.gui.layout;

public struct RelativePlacement
{
    public static readonly P BEFORE = -2;
    public static readonly P ABOVE = -2;
    public static readonly P TOP_EDGE = -1.5;
    public static readonly P LEFT_EDGE = -1.5;
    public static readonly P TOP = -1;
    public static readonly P LEFT = -1;
    public static readonly P CENTER = 0;
    public static readonly P RIGHT = 1;
    public static readonly P BOTTOM = 1;
    public static readonly P RIGHT_EDGE = 1.5;
    public static readonly P BOTTOM_EDGE = 1.5;
    public static readonly P AFTER = 2;
    public static readonly P BELOW = 2;

    public static readonly Range<double> inside = (-1d).to(1);

    public double value = double.NaN;
    public RelativePlacement(double value)
    {
        this.value = value;
    }

    public bool isInside => inside.contains(value);

    public double localScale
    {
        get
        {
            if (isInside) return value;
            if (value < 0) return value + 1;
            return value - 1;
        }
    }

    public static implicit operator P(double v)
        => new(v);

    public override string ToString()
        => $"{value}";
}
