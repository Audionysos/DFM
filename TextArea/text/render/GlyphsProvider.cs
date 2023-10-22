using com.audionysos.text.edit;

namespace com.audionysos.text.render; 
/// <summary>Provides <see cref="Glyph"/>s for given characters.</summary>
public abstract class GlyphsProvider {
	/// <summary>Glyph used when </summary>
	public Glyph missingGlyph { get; set; }

	public abstract Glyph get(char c, ITextFormat f);

}

public class DefaultGlyphsProvider : GlyphsProvider {
	public override Glyph get(char c, ITextFormat f) {
		var r = getCached(c, f);
		if (r) return r;
		r = produceGlpyh(c, f);
		return missingGlyph;
	}

	/// <summary>Produces glpyh for given configuration.
	/// Result may be sotred in chache.
	/// Returns null if glyph cannot be produced for the configuration.</summary>
	/// <param name="c"></param>
	/// <param name="f"></param>
	private Glyph produceGlpyh(char c, ITextFormat f) {

		return null;
	}

	/// <summary>Tries to get a glyph from cache.
	/// Returns null if not glyhp for given configuration was produced yet.</summary>
	/// <param name="c"></param>
	/// <param name="f"></param>
	private Glyph getCached(char c, ITextFormat f) {
		return null;
	}
}
