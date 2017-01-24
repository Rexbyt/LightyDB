using System;
using Winux.Dialogs;
using Gtk;
namespace WinuxDB
{
	public class CustomCellRenderText : CellRendererText
	{ 
		public int ColumnIndex { get; set; }
	}

	public class CustomCellRendererCombo : CellRendererCombo
	{
		public int ColumnIndex { get; set; }
	}

	public partial class wCreateTable : Gtk.Window
	{
		private ListStore rows;

		public wCreateTable() :
				base(Gtk.WindowType.Toplevel)
		{
			Build();

			try
			{
				CustomCellRenderText EditableCell = new CustomCellRenderText();
				EditableCell.Editable = true;
				EditableCell.ColumnIndex = 0;
				EditableCell.Edited += new EditedHandler(OnEdt);
				tblColumns.AppendColumn("Parameter", EditableCell, "text", EditableCell.ColumnIndex);

				CustomCellRenderText EditableCell1 = new CustomCellRenderText();
				EditableCell1.Editable = true;
				EditableCell1.ColumnIndex = 1;
				EditableCell1.Edited += new EditedHandler(OnEdt);
				tblColumns.AppendColumn("Value", EditableCell1, "text", EditableCell1.ColumnIndex);

				CustomCellRendererCombo EditableCell2 = new CustomCellRendererCombo();
				ListStore listType = new ListStore(typeof(string));
				EditableCell2.Editable = true;
				EditableCell2.Mode = CellRendererMode.Editable;
				EditableCell2.ColumnIndex = 2;
				EditableCell2.TextColumn = 0;
				//EditableCell2.Foreground = "blue";
				EditableCell2.Model = listType;

				listType.AppendValues("Numeric");
				listType.AppendValues("Text");
				listType.AppendValues("Date");
				listType.AppendValues("Time");
				listType.AppendValues("Money");

				EditableCell2.Edited += new EditedHandler(OnComboEdt);
				tblColumns.AppendColumn("Type", EditableCell2, "combo", EditableCell2.ColumnIndex);

				rows = new ListStore(typeof(string), typeof(string), typeof(ComboBox));
				tblColumns.Model = rows;

				tblColumns.ShowAll();
			}
			catch (Exception err)
			{
				ExceptReport.Details(err);
			}
		}

		void OnEdt(object o, EditedArgs args)
		{
			try
			{
				TreeIter iter;

				tblColumns.Selection.GetSelected(out iter);
				tblColumns.Model.SetValue(iter, ((CustomCellRenderText)o).ColumnIndex, args.NewText);
			}
			catch (Exception err){
				ExceptReport.Details(err);
			}
		}

		void OnComboEdt(object o, EditedArgs args)
		{
			try
			{
				//TreeIter tblIter;

				//tblColumns.Selection.GetSelected(out tblIter);
				//tblColumns.Model.SetValue(tblIter, 2, args.NewText);
				//MsgBox.Apply(combo.ToString(), "");
				((CustomCellRendererCombo)o).Text = args.NewText;

			}
			catch (Exception err){
				ExceptReport.Details(err);
			}
		}

		protected void OnActAddColumnActivated(object sender, EventArgs e)
		{
			try
			{
				this.rows.AppendValues("...", "...", "Text");
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
	}
}
