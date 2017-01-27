using System;
using Gtk;

namespace WinuxDB
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Application.Init();
			wndMain win = new wndMain();
			win.Show();

			Application.Run();
		}
	}
}
