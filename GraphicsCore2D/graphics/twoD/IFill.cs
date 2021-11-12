namespace com.audionysos {
	
	public interface IFill : IFillPiece {
		public int id { get; }
		public int width { get; }
		public int height { get; }
		public uint getSample(double u, double v);

	}

	public interface IFillPorovider {
		IFill getFill(int id);
	}

	/// <summary>Represents a fill as a texture.</summary>
	public interface IFillPiece {
		int fillID { get; }
		int u { get; }
		int v { get; }
	}

	public class Color : IFill {
		public int id { get; }
		public int width { get; }
		public int height { get; }
		public int fillID { get; }
		public int u { get; }
		public int v { get; }

		public uint rgba { get; }

		public uint rgb => rgba >> 8;
		public uint r => rgba >> 24;
		public uint g => (rgba >> 16) & 0x000000FF;
		public uint b => (rgba >> 8) & 0x000000FF;
		public uint a => rgba & 0x000000FF;

		public Color(uint rgba) {
			this.rgba = rgba;
		}

		public uint getSample(double u, double v) {
			return rgba;
		}

		public static implicit operator Color(uint rgba)
			=> new Color(rgba);

		//NOTE: not tested
		public static bool operator ==(Color a, Color b)
			=> (!(a is null)) && a.Equals(b)
				|| (a is null && b is null);

		public static bool operator !=(Color a, Color b)
			=> (!(a is null)) && !a.Equals(b)
				|| (a is null && !(b is null));

		/// <inheritdoc/>
		public override bool Equals(object obj) {
			if (!(obj is Color oc)) return false;
			return rgba == oc.rgba;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
			=> rgba.GetHashCode();

		public override string ToString() {
			return rgba.ToString("X8");
		}
	}

}
