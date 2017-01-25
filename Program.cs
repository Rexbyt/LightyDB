using System;
using Gtk;

namespace WinuxDB
{
	static class XData 
	{
		public static string connString = "Data Source = template.sqlite3"; 
	}

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
