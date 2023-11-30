using audionysos.geom;

namespace audionysos.gui.layout;

public class LayoutSettings
{
    public SurfacePlacement placement { get; set; } = new();
    public DimensionsData size { get; set; } = new();
    public Point2 position { get; internal set; } = new();

    public Rect bounds => new Rect(position, (Point2)size.actual);
}
