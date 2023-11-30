using audionysos.display;
using audionysos.graphics.extensions.shapes;
using com.audionysos;

namespace audionysos.gui.style;

public class DefaultPainter : GUIPainter
{
    public override void paint(UIElement e, DisplayObject view)
    {
        var s = view as Sprite;
        if (s == null) return;
        var g = s.graphics;
        var vs = e.style;
        var c = vs.background as Color;
        if (c == null)
            c = new Color(0x000000FF);
        //c = vs.colorPalette[];
        g.clear();
        g.beginFill(c);
        var acs = e.layout.size.actual;
        g.drawRect(0, 0, acs.width, acs.height);
        g.close();
        s.transform.pos = e.layout.position;
    }
}



