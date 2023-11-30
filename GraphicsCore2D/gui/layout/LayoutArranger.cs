using audionysos.display;
using audionysos.geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS = audionysos.gui.layout.LayoutSettings;
using static audionysos.utils.Fantastatics;
using audionysos.math;

namespace audionysos.gui.layout;
public class LayoutArranger
{

    public LayoutArranger()
    {

    }

    public void arrange(UIElementContainer? parent, UIElement child)
    {
        //LS pL = parent.layout;
        LS chL = child.layout;
        var avs = parent?.layout.size.actual
            ?? child.layout.size.design;

        var ds = chL.size.desired
            .max(chL.size.minimal)
            .min(chL.size.maximal);

        var (w, h) = (ds.width.value, ds.height.value);
        if (ds.width.isRelative)
            w = avs.width * ds.width;
        if (ds.height.isRelative)
            h = avs.height * ds.height;
        var s = chL.size.actual = (w, h);

        var sl = (avs - s) * .5; //size left

        chL.position = each(t =>
        {
            var (p, sl, s, av) = t; //placement, size left(parent), size
            if (p.isInside) return sl * p.value + sl;
            else
            {
                var e = sl * p.value.clip(-1, 1) + sl;
                return e + s * p.localScale;
            }
        }
            , (chL.placement.horizontal, sl.width, s.width, avs.width)
            , (chL.placement.vertical, sl.height, s.height, avs.height)
        );

        //Point2 p = chL.placement;
        //var (x, y) = (0d, 0d);
        //if (chL.placement.horizontal.isInside) {
        //	x = sl.width * p.x + sl.width;
        //}else {
        //	x = sl.width * 2 + (s.width * .5)
        //		* chL.placement.horizontal.localScale();
        //}
        //if (chL.placement.vertical.isInside) {
        //	y = sl.height * p.y + sl.height;
        //} else {
        //	y = sl.height * 2 + (s.height * .5)
        //		* chL.placement.vertical.localScale();
        //}
        //chL.position = (x, y);


    }

}



