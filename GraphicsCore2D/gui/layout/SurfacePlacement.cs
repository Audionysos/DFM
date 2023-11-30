using P = audionysos.gui.layout.RelativePlacement;
using audionysos.geom;

namespace audionysos.gui.layout;

public class SurfacePlacement
{
    public P horizontal { get; set; }
    public P vertical { get; set; }

    public SurfacePlacement() { }
    public SurfacePlacement(P vertical, P horizontal)
    {
        this.horizontal = horizontal;
        this.vertical = vertical;
    }

    /// <summary>Crates placement form tuple. Vertical placement goes first.</summary>
    /// <param name="t"></param>
    public static implicit operator SurfacePlacement
        ((P v, P h) t) => new(t.v, t.h);

    public static implicit operator Point2(SurfacePlacement p)
        => (p.horizontal.value, p.vertical.value);

    public override string ToString()
        => $"({vertical} x {horizontal})";
}
