using System.Collections.Generic;

namespace com.audionysos.text.render; 
/// <summary>Represents a single glyph that could be rendered in text area.</summary>
public class Glyph {
	public string name { get; set; }
	public object pixelsCache { get; set; }
	public double width { get; }
	public double height { get; }
	public IReadOnlyList<Path> paths { get; }

	public Glyph(string name, double width, double height, IReadOnlyList<Path> paths) {
		this.name = name;
		this.width = width;
		this.height = height;
		this.paths = paths;
	}

	public override string ToString() {
		return @$"""{name}"" Glyph";
	}

	/// <summary>False if null.</summary>
	/// <param name="g"></param>
	public static implicit operator bool(Glyph g) => g!=null;
}
