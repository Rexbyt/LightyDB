// This Adapter for automatic show table and data in in table. For correctly working a script, required 
//specify TreeView and table name from database SQLite.

using System;
using System.Data;
using Winux.Data;
using Gtk;

namespace WinuxDB
{
	public class TableAdapter
	{
		private ListStore rows;
		private bool TableShown = false;
		private Gtk.TreeView table;
		private string dbTableName_;

		public string SetTableName { 
			get {
				return this.dbTableName_;
			}

			set {
				this.dbTableName_ = value;
			}
		}

		public TableAdapter(Gtk.TreeView Table, string dbTableName)
		{
			table = Table;
			dbTableName_ = dbTableName;
		}

		public void ShowTable()
		{
			if (TableShown)
			{
				this.UpdateTable();
				return;
			}

			SqliteCompact scp = new SqliteCompact(WinuxDB.Config.connString);
			DataRowCollection drcColumns = scp.GetTableInfo(this.dbTableName_);
			CellRendererText render;
			System.Type[] types = new System.Type[drcColumns.Count];

			string[] columns = new string[drcColumns.Count];
			int TypeIndex = 0;

			// How clear all columns before adding new columns
			foreach (DataRow dr in drcColumns)
			{
				System.Windows.Forms.Application.DoEvents();
				render = new CellRendererText();
				this.table.AppendColumn(dr["name"].ToString(), render, "text", TypeIndex);
				types.SetValue(typeof(string), TypeIndex);
				TypeIndex++;
			}
			rows = new ListStore(types);

			DataRowCollection drcRows = scp.Query("SELECT * FROM " + this.dbTableName_);
			foreach (DataRow dr in drcRows)
			{
				System.Windows.Forms.Application.DoEvents();
				for (int i = 0; i < rows.NColumns; i++)
				{
					System.Windows.Forms.Application.DoEvents();
					columns.SetValue(dr[i].ToString(), i);
				}
				rows.AppendValues(columns);
			}
			this.table.Model = rows;
			this.table.ShowAll();
			this.TableShown = true;
		}

		public void UpdateTable()
		{
			this.rows.Clear();
			SqliteCompact scp = new SqliteCompact(WinuxDB.Config.connString);
			DataRowCollection drcRows = scp.Query("SELECT * FROM " + this.dbTableName_);
			string[] columns = new string[rows.NColumns];

			foreach (DataRow dr in drcRows)
			{
				System.Windows.Forms.Application.DoEvents();
				for (int i = 0; i < rows.NColumns; i++)
				{
					System.Windows.Forms.Application.DoEvents();
					columns.SetValue(dr[i].ToString(), i);
				}
				rows.AppendValues(columns);
			}
			this.table.Model = rows;
			this.table.ShowAll();
		}
	}
}
