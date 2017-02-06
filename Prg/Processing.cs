using Gtk;
namespace WinuxDB
{
	public class Processing
	{
		private Window wnd = null;
		public int Width = 400;
		public int Height = 50;
		public string ImagePath = "img/loadingAnimation.gif";

		public Processing()
		{
			Image img = new Image(ImagePath);
			VBox vbox = new VBox();
			wnd = new Window(WindowType.Toplevel);
			vbox.PackStart(img);
			wnd.Add(vbox);
			OptionsUpdate();
		}

		public void SetSize(int width, int height) {
			Width = width;
			Height = height;
		}

		private void OptionsUpdate() {
			wnd.WindowPosition = WindowPosition.Center;
			wnd.SetSizeRequest(Width, Height);
			wnd.Resizable = false;
		}

		public void Start(string Title) {
			OptionsUpdate();
			wnd.Title = Title;
			wnd.ShowAll();
		}

		public void Start(string Title, string ImgPath)
		{
			ImagePath = ImgPath;
			wnd.Title = Title;
			OptionsUpdate();
			wnd.ShowAll();
		}

		public void Stop() {
			wnd.Destroy();
		}
	}
}
