using Gtk;
namespace Winux.Dialogs
{
	public static class MsgBox
	{
		public static ResponseType Apply(string message, string title) {
			MessageDialog msg = new MessageDialog(null, DialogFlags.Modal, MessageType.Other, ButtonsType.Ok, message);
			msg.SetPosition(WindowPosition.Center);
			msg.Title = title;
			if ((ResponseType)msg.Run() == ResponseType.Ok)
			{
				msg.Destroy();
				return ResponseType.Ok;
			}
			else {
				return ResponseType.None;
			}
		}
		public static ResponseType Apply(string message) {
			return MsgBox.Apply(message, "Apply");
		}

		public static ResponseType Info(string message, string title)
		{
			MessageDialog msg = new MessageDialog(null, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, message);
			msg.SetPosition(WindowPosition.Center);
			msg.Title = title;
			if ((ResponseType)msg.Run() == ResponseType.Ok)
			{
				msg.Destroy();
				return ResponseType.Ok;
			}
			else {
				return ResponseType.None;
			}
		}
		public static ResponseType Info(string message)
		{
			return MsgBox.Info(message, "Information");
		}

		public static ResponseType Error(string message, string title)
		{
			MessageDialog msg = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, message);
			msg.SetPosition(WindowPosition.Center);
			msg.Title = title;
			if ((ResponseType)msg.Run() == ResponseType.Ok)
			{
				msg.Destroy();
				return ResponseType.Ok;
			}
			else {
				return ResponseType.None;
			}
		}
		public static ResponseType Error(string message)
		{
			return MsgBox.Error(message, "Error");
		}

		public static ResponseType Question(string message, string title)
		{
			MessageDialog msg = new MessageDialog(null, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, message);
			msg.SetPosition(WindowPosition.Center);
			msg.Title = title;
			int res = msg.Run();
			if (res < 0)
			{
				msg.Destroy();
				return (ResponseType)res;
			}
			else 
			{
				msg.Destroy();
				return ResponseType.None;
			}
		}
		public static ResponseType Question(string message)
		{
			return MsgBox.Question(message, "Question");
		}

		public static ResponseType Warning(string message, string title)
		{
			MessageDialog msg = new MessageDialog(null, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, message);
			msg.SetPosition(WindowPosition.Center);
			msg.Title = title;
			int res = msg.Run();
			if (res < 0)
			{
				msg.Destroy();
				return (ResponseType)res;
			}
			else
			{
				msg.Destroy();
				return ResponseType.None;
			}
		}
		public static ResponseType Warning(string message)
		{
			return MsgBox.Warning(message, "Warning");
		}
	}
}
