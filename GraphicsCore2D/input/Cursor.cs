using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace audionysos.input;
public class Cursor {
	public static event Action? CHANGED;

	public static readonly Cursor TEXT = new Cursor();

	public static Cursor? current { get; private set; }

	public static void set(Cursor c) {
		current = c;
		CHANGED?.Invoke();
	}

}
