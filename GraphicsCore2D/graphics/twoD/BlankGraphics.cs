using audionysos.display;

namespace com.audionysos; 
/// <summary>Blank <see cref="IMicroGraphics2D"/> implementation to act as a placeholder when no proper instance could be provided.</summary>
public class BlankGraphics : IMicroGraphics2D {
	public static readonly BlankGraphics instance = new BlankGraphics();
	/// <summary><see cref="BlankGraphics"/> doesn't do anything.</summary>
	public double x { get; }
	/// <summary><see cref="BlankGraphics"/> doesn't do anything.</summary>
	public double y { get; }
	/// <summary><see cref="BlankGraphics"/> doesn't do anything.</summary>
	public IMicroGraphics2D beginFill(uint rgb, double a = 1) => this;
	/// <summary><see cref="BlankGraphics"/> doesn't do anything.</summary>
	public IMicroGraphics2D beginFill(IFillPiece fill) => this;
	/// <summary><see cref="BlankGraphics"/> doesn't do anything.</summary>
	public IMicroGraphics2D clear() => this;
	/// <summary><see cref="BlankGraphics"/> doesn't do anything.</summary>
	public IMicroGraphics2D close() => this;
	/// <summary><see cref="BlankGraphics"/> doesn't do anything.</summary>
	public IMicroGraphics2D endFill() => this;
	/// <summary><see cref="BlankGraphics"/> doesn't do anything.</summary>
	public IMicroGraphics2D lineStyle(double w = 0, uint rgb = 0, double a = 1) => this;
	/// <summary><see cref="BlankGraphics"/> doesn't do anything.</summary>
	public IMicroGraphics2D lineTo(double x, double y) => this;
	/// <summary><see cref="BlankGraphics"/> doesn't do anything.</summary>
	public IMicroGraphics2D moveTo(double x, double y) => this;
	/// <summary><see cref="BlankGraphics"/> doesn't do anything.</summary>
	public IMicroGraphics2D transform(Transform t) => this;
	/// <summary><see cref="BlankGraphics"/> doesn't do anything.</summary>
	public IMicroGraphics2D wait() => this;
}
