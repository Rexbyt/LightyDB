using System;
using Gtk;
namespace Winux.Dialogs
{
	public static class ExceptReport
	{
		public static void Details(Exception exception)
		{
			Window wnd = new Window(WindowType.Toplevel);
			wnd.Modal = true;
			wnd.SetPosition(WindowPosition.Center);
			wnd.Title = "Техническая ошибка";
			wnd.BorderWidth = 10;

			VBox vbox = new VBox();
			HBox hbox = new HBox();

			Frame frm = new Frame("Описание ошибки:");
			Label lbl = new Label(exception.Message);
			lbl.SetAlignment(-1, -1);
			lbl.SetPadding(10, 10);
			lbl.LineWrap = true;
			lbl.LineWrapMode = Pango.WrapMode.Word;
			lbl.WidthRequest = 450;
			frm.Add(lbl);
			frm.WidthRequest = 500;
			vbox.PackStart(frm, false, false, 0);

			TextView txt = new TextView();
			txt.Buffer.Text = Environment.StackTrace + Environment.NewLine + Environment.NewLine +"Main error: "+ 
				exception.StackTrace;
			txt.Editable = false;
			txt.BorderWidth = 5;
			txt.WidthRequest = 500;
			txt.HeightRequest = 200;
			txt.LeftMargin = 10;
			txt.RightMargin = 10;
			txt.ResizeMode = ResizeMode.Parent;
			vbox.PackStart(txt);

			vbox.PackEnd(hbox, false, false, 0);

			Button btnOk = new Button();
			btnOk.Name = "btnOk";
			btnOk.Label = "Ок";
			btnOk.HeightRequest = 30;
			btnOk.Clicked += new EventHandler(btnOkClocked);
			hbox.PackStart(btnOk, true, true, 5);

			Button btnSendReport = new Button();
			btnSendReport.Name = "btnSendReport";
			btnSendReport.Label = "Сообщить об ошибке";
			btnSendReport.HeightRequest = 30;
			btnSendReport.Clicked += new EventHandler(btnSendReportClicked);
			hbox.PackEnd(btnSendReport, true, true, 5);

			wnd.Add(vbox);
			wnd.ShowAll();
		}


		static void btnOkClocked(object sender, EventArgs e)
		{
			((Button)sender).Parent.Parent.Parent.Destroy();
		}

		static void btnSendReportClicked(object sender, EventArgs e)
		{
			// Отправляем на центральный сервер репорт об ошибке
			// с детальным описанием (Версия программы, ОС, СтакТрейс)
		}
	}
}
