using com.audionysos.text.edit;

namespace com.audionysos.text.render; 
/// <summary>Associates all objects need for text displaying/editing controls.</summary>
public class TextDisplayContext {
	public static GlyphsProvider defaulGlyphsProvider = new DefaultGlyphsProvider();
	public static ITextFormatProvider defaulTextFormat = new TextFormat();

	public TextManipulator manipulator;
	public GlyphsProvider glyphs = defaulGlyphsProvider;
	public IGlyphRenderer glyphsRenderer = new GlypRenderer();
	public TextAreaRenderer renderer;
	public TextAreaView view;
	public IGraphics2D gfx;
	/// <summary>Default text format used when no other is specified for a poriton of text.
	/// By default the static <see cref="defaulTextFormat"/> is used.</summary>
	public ITextFormat fromat = defaulTextFormat.textFormat;
}
