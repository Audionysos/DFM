using audionysos.display;
using com.audionysos.text.edit;

namespace com.audionysos.text.render; 
/// <summary>Associates all objects need for text displaying/editing controls.</summary>
public class TextDisplayContext {
	public static GlyphsProvider defaultGlyphsProvider = new DefaultGlyphsProvider();
	public static ITextFormatProvider defaultTextFormat = new TextFormat();

	public TextManipulator manipulator;
	public GlyphsProvider glyphs = defaultGlyphsProvider;
	public IGlyphRenderer glyphsRenderer = new GlypRenderer();
	public TextAreaRenderer renderer;
	public TextAreaView view;
	public IGraphics2D gfx;
	public DisplayObjectContainer container;
	/// <summary>Default text format used when no other is specified for a portion of text.
	/// By default the static <see cref="defaultTextFormat"/> is used.</summary>
	public ITextFormat format = defaultTextFormat.textFormat;
}
