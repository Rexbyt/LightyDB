using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using Winux.Dialogs;
namespace WinuxDB
{
	public static class Config
	{
		public static string connString = "Data Source = template.sqlite3";

		public static string Lang(string id, string text)
		{
			string lang = "Default";
			if (lang == "Default")
			{
				return text;
			}
			else {
				XDocument xdoc = XDocument.Load("lang\\"+lang+".xml");
				IEnumerable<XElement> xel = xdoc.Element("lang").Elements("item").Where(e => e.Attribute("id").Value == id);
				XElement el = xel.First();
				return el.Attribute("title").Value.Trim();
			}
		}
	}
}
