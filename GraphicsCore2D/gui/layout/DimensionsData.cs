using S = audionysos.gui.layout.Size;
using D = audionysos.gui.layout.LayoutDimensions;

namespace audionysos.gui.layout;

public class DimensionsData
{
    /// <summary>Default size to be used as size of the parent element if the child has no parent.
    /// This should have absolute values.</summary>
    public D design { get; set; } = (200, 100);
    public D desired { get; set; }
    public D minimal { get; set; }
    public D maximal { get; set; } = D.MAX;
    /// <summary>The size that was actually set by an arranger.</summary>
    public D actual { get; internal set; } = (0, 0);

    /// <summary>Specifies a fixed size by setting both <see cref="minimal"/> and <see cref="maximal"/> properties to the same value.</summary>
    public D fix { set => minimal = maximal = value; }
    /// <summary>Tells if specified size is fixed (see <see cref="fix"/>).</summary>
    public bool isFixed => minimal == maximal;

    public static implicit operator DimensionsData
        ((S w, S h) t) => new() { desired = t };

    public override string ToString()
        => $"{actual} <= {desired}";
}
