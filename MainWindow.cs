using System;
using System.Windows.Forms;
using Gtk;
using Winux.Tables;
using Winux.Data;
using Winux.Dialogs;

public partial class wndMain : Gtk.Window
{
	public TableAdapter _atblMain;

	public wndMain() : base(Gtk.WindowType.Toplevel)
	{
		Build();
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Gtk.Application.Quit();
		a.RetVal = true;
	}
	protected void OnActCreateTableActivated(object sender, EventArgs e)
	{
		// Only one comment
		WinuxDB.wCreateTable wct = new WinuxDB.wCreateTable();
		wct.ShowAll();
	}
}                       