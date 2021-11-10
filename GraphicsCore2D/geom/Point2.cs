namespace audioysos.geom {
	public class Point2 : IPoint2 {
		public double x { get; set; }
		public double y { get; set; }

		public Point2() { }

		public Point2(double x, double y) {
			this.x = x; this.y = y;
		}

		public IPoint2 copy() => new Point2(x, y);

		public IPoint2 create(double x, double y) => new Point2(x, y);

		public static implicit operator Point2((double x, double y) t)
			=> new Point2(t.x, t.y);

		/// <inheritdoc/>
		public override string ToString() {
			return $@"({x}, {y})";
		}
	}

	public class Rect : IRect<Point2> {
		public Point2 postion { get; }
		public Point2 size { get; }

		public Rect(Point2 pos, Point2 size) {
			postion = pos.copy() as Point2;
			size = size.copy() as Point2;
		}
	}

}
