using System;
using Gtk;

namespace WinuxDB
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Mono.Unix.Catalog.Init("i8n1", "./locale");
			Environment.SetEnvironmentVariable("LANGUAGE", "ru");
			Application.Init();
			wndMain win = new wndMain();
			win.Show();

			Application.Run();
		}
	}
}
