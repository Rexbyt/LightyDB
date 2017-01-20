using System;
using Gtk;
namespace Winux.Dialogs
{
	public class Prompt
	{
		private Window wnd = null;
		private TextView txt = null;
		public string Text;

		public Prompt()
		{
			wnd = new Window(WindowType.Toplevel);
			wnd.Modal = true;
			wnd.SetPosition(WindowPosition.Center);
			wnd.Title = "Подскажите ...";
			wnd.BorderWidth = 10;

			VBox vbox = new VBox();
			HBox hbox = new HBox();

			Frame frm = new Frame("");
			Label lbl = new Label("Укажите свой возраст");
			lbl.SetAlignment(-1, -1);
			lbl.SetPadding(10, 10);
			lbl.LineWrap = true;
			lbl.LineWrapMode = Pango.WrapMode.Word;
			lbl.WidthRequest = 450;
			frm.Add(lbl);
			frm.WidthRequest = 500;
			vbox.PackStart(frm, false, false, 0);

			txt = new TextView();
			txt.Buffer.Text = "";
			txt.Editable = true;
			txt.BorderWidth = 5;
			txt.WidthRequest = 500;
			txt.HeightRequest = 100;
			txt.LeftMargin = 10;
			txt.RightMargin = 10;
			txt.WrapMode = WrapMode.Word;
			txt.ResizeMode = ResizeMode.Parent;
			vbox.PackStart(txt);

			vbox.PackEnd(hbox, false, false, 0);

			Button btnSend = new Button();
			btnSend.Name = "btnSend";
			btnSend.Label = "Ответить";
			btnSend.HeightRequest = 30;
			btnSend.Clicked += new EventHandler(btnOkClocked);
			hbox.PackStart(btnSend, true, true, 5);

			wnd.Add(vbox);
			wnd.ShowAll();

		}

		void btnOkClocked(object sender, EventArgs e)
		{
			Text = txt.Buffer.Text;
			wnd.Destroy();
		}
}
}
