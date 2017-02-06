using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Winux.Dialogs;
using Winux.Data;
using System.Data;
using Gtk;
using Mono.Unix;
namespace WinuxDB
{
	public class CustomCellRendererText : CellRendererText
	{ 
		public int ColumnIndex { get; set; }
		public string TypeCell { get; set; }
	}

	public class CustomCellRendererCombo : CellRendererCombo
	{
		public int ColumnIndex { get; set; }
		public string TypeCell { get; set; }
	}

	public class CustomCellRendererToggle : CellRendererToggle
	{
		public int ColumnIndex { get; set; }
		public string TypeCell { get; set; }
	}

	public partial class wCreateTable : Gtk.Window
	{
		private ListStore rows;
		public string tbl_name = "";
		private List<string> cmbTablesID = new List<string>();
		// Origin form title
		private string ttl = "";

		public wCreateTable() :
				base(Gtk.WindowType.Toplevel)
		{
			Build();

			try
			{
				// FORTEST

				// Save origin form title
				this.ttl = this.Title;

				// Create table for edit columns table of user
				this.CreateTableColumns();

				// Get all user tables
				this.GetNewTablesList();

				// Get selected table info
				this.GetInfoSelectedTable(this.tbl_name);
			}
			catch (Exception err)
			{
				ExceptReport.Details(err);
			}
		}

		public void CreateTableColumns() { 
			CustomCellRendererText ColumnNameCell = new CustomCellRendererText();
			ColumnNameCell.Editable = true;
			ColumnNameCell.ColumnIndex = 0;
			ColumnNameCell.TypeCell = "Text";
			ColumnNameCell.Edited += new EditedHandler(OnEdt);
			tblColumns.AppendColumn(Catalog.GetString("Name"), ColumnNameCell, "text", ColumnNameCell.ColumnIndex);

			CustomCellRendererText ColumnTitleCell = new CustomCellRendererText();
			ColumnTitleCell.Editable = true;
			ColumnTitleCell.ColumnIndex = 1;
			ColumnTitleCell.TypeCell = "Text";
			ColumnTitleCell.Edited += new EditedHandler(OnEdt);
			tblColumns.AppendColumn(Catalog.GetString("Title"), ColumnTitleCell, "text", ColumnTitleCell.ColumnIndex);

			CustomCellRendererCombo TypeCell = new CustomCellRendererCombo();
			ListStore listType = new ListStore(typeof(string));
			TypeCell.Editable = true;
			TypeCell.Mode = CellRendererMode.Editable;
			TypeCell.ColumnIndex = 2;
			TypeCell.TypeCell = "Combo";
			TypeCell.TextColumn = 0;
			TypeCell.HasEntry = false;
			TypeCell.Model = listType;
			listType.AppendValues("Integer");
				listType.AppendValues("Text");
				listType.AppendValues("Date");
				listType.AppendValues("Time");
				listType.AppendValues("Money");
			TypeCell.Edited += new EditedHandler(OnEdt);
			tblColumns.AppendColumn(Catalog.GetString("Type"), TypeCell, "text", TypeCell.ColumnIndex);

			CustomCellRendererText SizeCell = new CustomCellRendererText();
			SizeCell.Editable = true;
			SizeCell.Mode = CellRendererMode.Editable;
			SizeCell.ColumnIndex = 3;
			SizeCell.TypeCell = "Double";
			SizeCell.Edited += new EditedHandler(OnEdt);
			tblColumns.AppendColumn(Catalog.GetString("Size"), SizeCell, "text", SizeCell.ColumnIndex);

			CustomCellRendererToggle UniqCell = new CustomCellRendererToggle();
			UniqCell.Activatable = true;
			UniqCell.Toggled += new ToggledHandler(OnToggled);
			UniqCell.ColumnIndex = 4;
			tblColumns.AppendColumn(Catalog.GetString("Unique"), UniqCell, "active", UniqCell.ColumnIndex);

			CustomCellRendererToggle RequiredCell = new CustomCellRendererToggle();
			RequiredCell.Activatable = true;
			RequiredCell.Toggled += new ToggledHandler(OnToggled);
			RequiredCell.ColumnIndex = 5;
			tblColumns.AppendColumn(Catalog.GetString("Required"), RequiredCell, "active", RequiredCell.ColumnIndex);

			CustomCellRendererToggle PrimaryCell = new CustomCellRendererToggle();
			PrimaryCell.Activatable = true;
			PrimaryCell.Toggled += new ToggledHandler(OnToggled);
			PrimaryCell.ColumnIndex = 6;
			tblColumns.AppendColumn(Catalog.GetString("Primary"), PrimaryCell, "active", PrimaryCell.ColumnIndex);

			CustomCellRendererToggle AutoincrementCell = new CustomCellRendererToggle();
			AutoincrementCell.Activatable = true;
			AutoincrementCell.Toggled += new ToggledHandler(OnToggled);
			AutoincrementCell.ColumnIndex = 7;
			tblColumns.AppendColumn(Catalog.GetString("Autoincrement"), AutoincrementCell, "active", AutoincrementCell.ColumnIndex);

			rows = new ListStore(typeof(string), typeof(string), typeof(string), typeof(double), typeof(bool)
			                    , typeof(bool), typeof(bool), typeof(bool));
			this.rows.AppendValues("id", "ID", "Integer", 0, true, false, true, true);
			tblColumns.Model = rows;

			tblColumns.ShowAll();
		}

		void OnToggled(object o, ToggledArgs args)
		{
			TreeIter iter;
			TreeModel store = tblColumns.Model;

			CustomCellRendererToggle chb = o as CustomCellRendererToggle;

			if (store.GetIterFromString(out iter, args.Path))
			{
				bool val = (bool)store.GetValue(iter, chb.ColumnIndex);
				store.SetValue(iter, chb.ColumnIndex, !val);
			}
		}

		void OnEdt(object o, EditedArgs args)
		{
			try
			{
				TreeIter iter;
				TreeModel model;

				tblColumns.Selection.GetSelected(out model, out iter);
				if (o.ToString().Contains("RendererText"))
				{
					if (((CustomCellRendererText)o).TypeCell == "Text")
					{
						model.SetValue(iter, ((CustomCellRendererText)o).ColumnIndex, args.NewText);
					}
					else if (((CustomCellRendererText)o).TypeCell == "Double")
					{
						double newvalue = double.Parse(args.NewText.Replace(".", ","));
						model.SetValue(iter, ((CustomCellRendererText)o).ColumnIndex, newvalue);
					}
				}
				else if (o.ToString().Contains("RendererCombo")){
					model.SetValue(iter, ((CustomCellRendererCombo)o).ColumnIndex, args.NewText);
				}
			}
			catch (Exception err){
				ExceptReport.Details(err);
			}
		}

		protected void OnActAddColumnActivated(object sender, EventArgs e)
		{
			try
			{
				//Create new row for column table
				this.rows.AppendValues("...", "...", "...", 0, false, false, false, false);
				tblColumns.Model = this.rows;
				tblColumns.ShowAll();
			}
			catch (Exception err){
				ExceptReport.Details(err);
			}
		}

		protected void OnActDeleteActivated(object sender, EventArgs e)
		{
			this.tblColumnRowDelete();
		}

		private void tblColumnRowDelete() { 
			// Delete selected row
			TreeSelection ts = tblColumns.Selection;
			TreeIter iter;
			if (ts.GetSelected(out iter))
			{
				if (ts.CountSelectedRows() == 1)
				{
					this.rows.Remove(ref iter);
				}
				else {
					this.rows.Clear();
				}
				tblColumns.Model = this.rows;
				tblColumns.ShowAll();
			}
		}

		protected void OnTblColumnsKeyPressEvent(object o, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Delete)
			{
				this.tblColumnRowDelete();
			}
		}

		private string ColumnsSQL = "";
		private string tblHeaders = "";
		protected void OnActCreateActivated(object sender, EventArgs e)
		{
			try
			{
				SqliteCompact scp = new SqliteCompact(Config.connString);
				// Collect main request for create table
				string tblName = txtTableName.Text.Trim().ToLower();
				string tblTitle = txtTableTitle.Text.Trim();

				scp.Open();
				int rows = Convert.ToInt32(scp.QueryScalar("SELECT COUNT([name]) FROM sqlite_master WHERE type = 'table' AND LOWER([name]) = 'itbl_"+tblName+"'"));
				if (rows > 0)
				{
					MsgBox.Warning(
						string.Format(Catalog.GetString("Table ({0}) already exists in database!"), tblName), 
						Catalog.GetString("Warning"));
					return;
				}

				// Prefix itbl_ helps to distinguish table info of user
				// For auto select from sqlite_master
				this.ColumnsSQL = "CREATE TABLE itbl_" + tblName + "(";

				tblColumns.Model.Foreach(ForeachModelOftblColumns);

				this.ColumnsSQL = this.ColumnsSQL.Trim().Substring(0, (this.ColumnsSQL.Trim().Length - 1))+");";

				scp.Execute(this.ColumnsSQL);
				// Add headers
				if (this.tblHeaders.Trim().Length > 0)
				{
					if (!Options.Set("headers", "itbl_" + tblName, this.tblHeaders, true))
					{
						MsgBox.Error(
							string.Format(Catalog.GetString("For unknown reasons, failed to install the headers for the table ({0})!"),
										  tblName), Catalog.GetString("Error"));
					}
				}
				scp.Close();

				if (chbMainTable.Active)
				{
					if (!Options.Set("main_table", "itbl_" + tblName, true))
					{
						MsgBox.Error(
							string.Format(Catalog.GetString("For unknown reasons, failed to make ({0}) the main table!"),
										  tblName), Catalog.GetString("Error"));
					}
				}

				// Insert new table in ComboBox and selecting him
				int newTblIndex = this.cmbTablesID.Count;
				cmbTableList.InsertText(newTblIndex, tblTitle);
				cmbTablesID.Add(tblName);
				cmbTableList.Active = newTblIndex;
				this.tbl_name = tblName;

				MsgBox.Info(Catalog.GetString("Table created successfully"), Catalog.GetString("Information"));
			}
			catch (Exception err){
				ExceptReport.Details(err);
			}
		}

		//Collect request for columns of new table
		bool ForeachModelOftblColumns(TreeModel model, TreePath path, TreeIter iter)
		{
			TreeIter newIter;
			TreePath newPath = path;
			string tblTitle = txtTableTitle.Text.Trim();
			string columnsHeaders = "";


			while (model.GetIter(out newIter, newPath))
			{
				string colTitle = model.GetValue(newIter, 1).ToString().Trim();
				string colType = model.GetValue(newIter, 2).ToString().Trim();
				double colSize = Convert.ToDouble(model.GetValue(newIter, 3));
				string colUniq = Convert.ToBoolean(model.GetValue(newIter, 4)) ? "UNIQUE" : "";
				string colRequired = Convert.ToBoolean(model.GetValue(newIter, 5)) ? "_" : "";
				string colName = model.GetValue(newIter, 0).ToString().Trim() + colRequired;
				string colPrimary = Convert.ToBoolean(model.GetValue(newIter, 6)) ? "PRIMARY KEY" : "";
				string colAutoincrement = Convert.ToBoolean(model.GetValue(newIter, 7)) ? "AUTOINCREMENT" : "";

				string size = "";
				string toSqlite3Type = "";
				if (colType.Trim().ToLower() == "money")
					toSqlite3Type = "double";
				else if (colType.Trim().ToLower() == "text" && colSize > 0.0)
					toSqlite3Type = "varchar";
				else
					toSqlite3Type = colType.Trim().ToLower();

				if (colSize > 0.0)
					size = "(" + colSize.ToString() + ")";

				// Collect the list of headers
				// If title not exists, when leave the name of the id column
				if (colTitle.Trim().Length > 0) 
				{
					columnsHeaders += "\""+colName+"\":\""+colTitle+"\",";
				}

				// Collect the command on column create
				this.ColumnsSQL += colName + " " + toSqlite3Type + size + " NOT NULL " + colPrimary + " " + 
					colAutoincrement + " " + colUniq + ",";

				newPath.Next();
			}

			if (tblTitle.Length > 0)
				this.tblHeaders += "\"table\":\""+tblTitle+"\"";

			if (tblTitle.Length > 0 && columnsHeaders.Length > 0)
				this.tblHeaders += ",";

			if (columnsHeaders.Length > 0)
				this.tblHeaders += "\"column\":{" + columnsHeaders.Substring(0, (columnsHeaders.Length - 1)) + "}";

			this.tblHeaders = "{"+this.tblHeaders+"}";

			return true;
		}

		protected void OnCmbTableListChanged(object sender, EventArgs e)
		{
			// Get data of selected table for edit
			this.tbl_name = this.cmbTablesID[cmbTableList.Active];
			this.GetInfoSelectedTable(this.tbl_name);
		}

		private void GetInfoSelectedTable(string tblname)
		{
			// Get data of selected table for edit method
			if (tblname.Trim().Length > 0)
			{
				SqliteCompact scp = new SqliteCompact(Config.connString);
				this.Title = this.ttl + " [" + tblname + "]";
				// Table Name
				Match mtch = Regex.Match(tblname.Trim(), "^itbl_([\\w\\d]+)$", RegexOptions.IgnoreCase);
				txtTableName.Text = mtch.Groups[1].Value;
				// Table Title
				Headers header = Table.Headers(tblname);
				if (header != null && header.table.Trim().Length > 0)
					txtTableTitle.Text = header.table.Trim();
				// This is main table
				if (Options.Get("main_table").Trim() == tblname)
					chbMainTable.Active = true;
				else
					chbMainTable.Active = false;
				// Get column list in table
				//DataRowCollection drcRows = Table.GetTableColumn(tblname);
			}
			else
			{
				this.Title = this.ttl;
				// Clear all info
				txtTableName.Text = "";
				txtTableTitle.Text = "";
				txtvDescription.Buffer.Clear();
				cmbTblDependence.Clear();
				cmbClmDependence.Clear();
				chbMainTable.Active = false;
				this.rows.Clear();
				this.rows.AppendValues("id", "ID", "Integer", 0, true, false, true, true);
			}
		}

		// Get all user tables
		private void GetNewTablesList()
		{
			// Processing start
			Processing prcs = new Processing();
			prcs.Start(Catalog.GetString("Getting a list of tables..."));
			// Clear old list
			ListStore ls = new ListStore(typeof(string));
			cmbTableList.Model = ls;
			// Collect new list tables
			DataRowCollection drcTables = Table.GetTablesName("itbl_%");
			if (drcTables.Count > 0)
			{
				int sortIndex = 0;
				cmbTableList.InsertText(sortIndex, "");
				this.cmbTablesID.Add("");
				foreach (DataRow dr in drcTables)
				{
					sortIndex++;
					string tblID = dr[0].ToString().Trim();
					this.cmbTablesID.Add(tblID);
					// Get header for current table id
					Headers header = Table.Headers(tblID);
					if (header != null && header.table.Trim().Length > 0)
						cmbTableList.InsertText(sortIndex, header.table);
					else
						cmbTableList.InsertText(sortIndex, tblID);
				}
			}
			prcs.Stop();
		}

		protected void OnBtnRefreshTablesClicked(object sender, EventArgs e)
		{
			// Get all user tables
			this.GetNewTablesList();
		}
	}
}
