using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Xml;
using Winux.Data;
using System.Windows.Forms;

namespace Winux.Data
{
	class SqliteCompactAdapter
    {
		public SqliteCompactAdapter(string connectionString) {
			if (!Environment.OSVersion.ToString().Contains("Unix"))
			{
				WinSqliteCompact scp = new WinSqliteCompact(connectionString);
			}
		}
    }
}
