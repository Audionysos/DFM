using S = audionysos.gui.layout.Size;
using D = audionysos.gui.layout.LayoutDimensions;
using System.Numerics;

namespace audionysos.gui.layout;

public static class LayoutExtensions
{
    public static D relative<N>(this (N w, N h) t) where N : INumber<N>
        => new D(S.Relative(t.w), S.Relative(t.h));

    public static D toRelative(this D t)
        => new D(t.width.relative(), t.height.relative());
}
