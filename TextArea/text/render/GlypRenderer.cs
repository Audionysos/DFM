﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using V = audionysos.geom.IPoint2;
using com.audionysos;
using audionysos.geom;
using com.audionysos.text.edit;
using System.Runtime.CompilerServices;
using audionysos.display;

namespace com.audionysos.text.render; 
/// <summary>Abstract glyph rendering from SixLabors in WpfDNet project.</summary>
public class GlypRenderer : IGlyphRenderer {


	public RenderedGlyph render(IGlyphRenderingContext ctx) {
		var g = ctx.g;
		if (g == null) return null;
		var phs = g.paths; 
		//var pos = ctx.position.copy();
		var s = new Shape();
		var gfx = s.graphics;
		gfx.beginFill(ctx.format.foreground);
		for (int i = 0; i < phs.Count; i++) {
			var p = phs[i];
			var cp = p[0];
			var fp = cp;
			gfx.moveTo(cp);
			for (int j = 1; j < p.Count; j++) {
				cp = p[j];
				gfx.lineTo(cp);
			}
			gfx.lineTo(fp);
		}
		var rg = new RenderedGlyph(
			g,
			ctx.position.copy(),
			new Point2(g.width, g.height),
			s	
		);
		return rg;
	}

}

/// <summary>Abstract glyph rendering from SixLabors in WpfDNet project.</summary>
public class GlypRenderer2 : IGlyphRenderer {


	public RenderedGlyph render(IGlyphRenderingContext ctx) {
		var g = ctx.g;
		if (g == null) return null;
		var phs = g.paths; var gfx = ctx.gfx;
		var pos = ctx.position.copy();
		gfx.beginFill(ctx.format.foreground);
		for (int i = 0; i < phs.Count; i++) {
			var p = phs[i];
			var cp = p[0] + pos; var fp = cp;
			gfx.moveTo(cp);
			for (int j = 1; j < p.Count; j++) {
				cp = p[j] + pos;
				gfx.lineTo(cp);
			}
			gfx.lineTo(fp);
		}
		var rg = new RenderedGlyph(
			g,
			ctx.position.copy(),
			new Point2(g.width, g.height),
			gfx
		);
		return rg;
	}

}

public interface IGlyphRenderer {

	public RenderedGlyph render(IGlyphRenderingContext ctx);

}

public class GlyphRenderingContext : IGlyphRenderingContext {
	public Glyph g { get; set; }
	public IGraphics2D gfx { get; set; }
	public V position { get; set; } = new Point2();
	public ITextFormat format { get; set; }
}

public interface IGlyphRenderingContext {
	public Glyph g { get; }
	public IGraphics2D gfx { get; }
	public V position { get; }
	public ITextFormat format { get; }
}

public interface IGlyphPathsProvider {

	public Path[] getPath(Glyph g);
}

public class RenderedGlyph {
	public V position { get; }
	public V size { get; }
	public Glyph template { get; }
	public object data { get; }

	public RenderedGlyph(Glyph template, V position, V size, object data) {
		this.template = template;
		this.position = position;
		this.size = size;
		this.data = data;
	}


	public override string ToString() {
		return $@"""{template.name}"", RenderedGlyph";
	}

	/// <summary>False if null.</summary>
	public static implicit operator bool(RenderedGlyph g)
		=> g != null;
}
