using System;
using System.Windows.Forms;
using System.Data;
using Gtk;
using Winux.Data;
using Winux.Dialogs;
using WinuxDB;

public partial class wndMain : Gtk.Window
{
	private TableAdapter tbl;
	private string main_table;

	public wndMain() : base(Gtk.WindowType.Toplevel)
	{
		Build();

		this.main_table = Options.Get("main_table");
		if(main_table.Length > 0)
		{
			this.tbl = new TableAdapter(tblMain, this.main_table);
			this.tbl.ShowTable();
		}
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

	protected void OnActRefreshTableActivated(object sender, EventArgs e)
	{
		try
		{
			if (this.main_table.Length > 0)
			{
				this.tbl.ShowTable();
			}
			else {
				this.main_table = Options.Get("main_table");
				MsgBox.Info(this.main_table);
				if (this.main_table.Length > 0)
				{
					this.tbl.SetTableName = this.main_table; // Error?
					this.tbl.ShowTable();
				}
				else { 
					MsgBox.Info(Config.Lang("msgNotSetMainTable", "Not set main table. Select the table manually."), 
					            Config.Lang("titleInformation", "Information"));
				}
			}
		}
		catch (Exception err){
			ExceptReport.Details(err);
		}
	}
}                       