using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfDNet;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
	public static string mode;

	public App() {
		Startup += OnStartup;
	}


	private void OnStartup(object sender, StartupEventArgs e) {
		var ars = e.Args;
		if (ars == null || ars.Length == 0) return;
		mode = ars[0];
	}
}
