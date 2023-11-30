using S = audionysos.gui.layout.Size;
using D = audionysos.gui.layout.LayoutDimensions;
using audionysos.geom;

namespace audionysos.gui.layout;

public record struct LayoutDimensions
{
    public static readonly D MAX = (double.MaxValue, double.MaxValue);

    public S width;
    public S height;

    public LayoutDimensions(S width, S height)
    {
        this.width = width;
        this.height = height;
    }

    public D min(D other)
        => (width.min(other.width), height.min(other.height));

    public D max(D other)
        => (width.max(other.width), height.max(other.height));

    //TODO:
    public static D operator -(D a, D b)
        => new D(a.width - b.width, a.height - b.height);

    public static D operator *(D a, double v)
        => new D(a.width * v, a.height * v);

    public static implicit operator D
        ((S w, S h) t) => new(t.w, t.h);

    public static implicit operator Point2
        (D d) => new(d.width, d.height);

    public override readonly string ToString()
        => $"({width} x {height})";
}
