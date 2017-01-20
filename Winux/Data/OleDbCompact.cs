using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace Winux.Data
{
    class OleDbCompact
    {
        public OleDbConnection Connection = null;
        public string sqlString = "";
        private string connstr = "";

        /// <summary>
        /// Ошибка выполенния работы с БД
        /// </summary>
        public string errors = "";

        /// <summary>
        /// Конструктор для подключения к базе данных
        /// </summary>
        /// <param name="connectionString">Строка соединения</param>
        public OleDbCompact(string connectionString) {
            try
            {
                this.Connection = new OleDbConnection(connectionString);
                this.connstr = connectionString;
            }
            catch (Exception err) {
                this.errors = err.Message.ToString() + " || " + err.StackTrace;
            }
        }

        /// <summary>
        /// Получаем сообщение об ошибке выполнения запроса, если таковая имеется. В лучшем случае ничего не произойдет
        /// </summary>
        public int GetError()
        {
            if (this.errors.Trim().Length > 0)
            {
				Gtk.MessageDialog msg = new Gtk.MessageDialog(null, Gtk.DialogFlags.NoSeparator, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, null);
				msg.Text = "Произошла ошибка в процессе выполнения запроса к базе данных:\n" + this.errors;
				msg.Title = "Ошибка";
				if (msg.Run() < 0)
					msg.Destroy();
                this.errors = "";
                return 1;
            }
            else {
                return 0;
            }
        }

        /// <summary>
        /// Полчает результаты от множества запросов в DataSet
        /// </summary>
        /// <param name="queries">Список запросов {"Название таблице в DataSet для результат","Запрос"}</param>
        /// <returns></returns>
        public List<int> QueryList(List<string> queries) {
            List<int> results = new List<int>();
            foreach (string kvp in queries) {
                 results.Add(this.Query(kvp).Count);
            }
            return results;
        }

        /// <summary>
        /// Получаем autoincrement id внутри сессии
        /// </summary>
        /// <returns>Автоматически присвоены ID</returns>
        public int getAutoincrementID()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT SCOPE_IDENTITY()", this.Connection);
            object val = cmd.ExecuteScalar();
            return Convert.ToInt32(val);
        }

        public object QueryScalar(string sql)
        {
            try
            {
                this.Open();
                OleDbCommand cmd = new OleDbCommand(sql, this.Connection);
                object val = cmd.ExecuteScalar();
                this.Close();
                return val;
            }
            catch (Exception err) {
                this.Close();
                this.errors = err.Message.ToString() + " ==> " + sql + " || " + err.StackTrace;
                return -1;
            }
        }

        public object Execute(string baseName, string procedure)
        {
            try
            {
                //Match mtch = Regex.Match(procedure, "([a-zA-Z0-9-_\\.]+){1}\\s*\\((.*)\\)\\s+VALUES\\s*\\((.*)\\)");
                if (/*mtch.Success*/ true)
                {
                    /*string pName = mtch.Groups[1].Value.Trim();
                    List<string> param = mtch.Groups[2].Value.Trim().Split(",".ToCharArray()).ToList<string>();
                    List<object> values = mtch.Groups[3].Value.Trim().Split(",".ToCharArray()).ToList<object>();*/

                    this.Open();
                    OleDbCommand cmd = new OleDbCommand("USE "+baseName+"; EXECUTE "+procedure, this.Connection);
                    cmd.CommandType = CommandType.Text;

                    /*if (param.Count > 0)
                    {
                        if (param.Count == values.Count)
                        {
                            for (int i = 0; i < param.Count; i++)
                            {
                                Application.DoEvents();
                                //cmd.Parameters.AddWithValue(param[i].Trim(), );
                                cmd.Parameters.Add(values[i]);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Количество параметров и значений не совпадает!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return "";
                        }
                    }*/

                    object val = cmd.ExecuteNonQuery();

                    this.Close();
                    return val;
                }
                else {
                    return 0;
                }
            }
            catch (Exception err)
            {
                this.Close();
                this.errors = err.Message.ToString() + " ==> " + procedure + " || " + err.StackTrace;
                return -1;
            }
        }

        public int getLastIncrementID(string tblName) {
            OleDbCommand cmd = new OleDbCommand("SELECT MAX(id) FROM " + tblName,this.Connection);
            object val = cmd.ExecuteScalar();
            return Convert.ToInt32(val);
        }

        /// <summary>
        /// Открытие соединения
        /// </summary>
        public void Open() {
            try
            {
                this.Connection.Open();
            }
            catch (Exception err) {
                this.errors = err.Message.ToString() + " || " + err.StackTrace;
            }
        }

        /// <summary>
        /// Закрытие соединения
        /// </summary>
        public void Close() {
            try
            {
                this.Connection.Close();
            }
            catch (Exception err) {
                this.errors = err.Message.ToString() + " || " + err.StackTrace;
            }
        }

        /// <summary>
        /// Компактное добавление данных для избежания путаницы
        /// </summary>
        /// <param name="tblName">Имя таблици добавдения</param>
        /// <param name="parameters">Перечисляем все поля и их значения в Dictionary</param>
        /// <returns>Вернет количество добавленных строк</returns>
        public int Insert(string tblName, Dictionary<string,object> param) {
            string sql = "INSERT INTO " + tblName + " (";
            string values = "";
            int intVar = 0;
            // Формируем список заполняемых полей
            foreach (KeyValuePair<string,object> kp in param) {
                sql += kp.Key + ",";
                try
                {
                    intVar = Convert.ToInt32(kp.Value);
                    values += kp.Value + ",";
                }
                catch {
                    values += "'" + kp.Value + "',";
                }
            }
            sql = sql.Trim().Substring(0, (sql.Length - 1));
            values = values.Trim().Substring(0,(values.Trim().Length - 1));
            sql += ") VALUES (" + values + ")";
            try
            {
                this.sqlString = sql;
                OleDbCommand cmd = new OleDbCommand(sql, this.Connection);
                int res = cmd.ExecuteNonQuery(); 
                return res;
            }
            catch(Exception err) {
                this.errors = err.Message.ToString() + " || " + err.StackTrace;
                return 0;
            }
        }

        /// <summary>
        /// Компактное альтернативное добавление данных для избежания путаницы.
        /// </summary>
        /// <param name="tblName">Имя таблици добавдения</param>
        /// <param name="param">Перечисляем все поля и их значения в Dictionary. 
        /// Его ключи должны соответствовать названиям полей целевой таблицы в БД!</param>
        /// <param name="fields">Перечисляем нужные поля из Dictionary</param>
        /// <returns>Вернет количество добавленных строк</returns>
        public int Insert(string tblName, Dictionary<string, object> param, string fields)
        {
            string sql = "INSERT INTO " + tblName + " (";
            string values = "";
            int intVar = 0;
            // Формируем список заполняемых полей
            if (fields.Trim().Length > 0) { 
                string[] field = fields.Trim().Split(",".ToCharArray());
                foreach (string str in field) {
                    Application.DoEvents();
                    if (param.Keys.Contains(str.Trim()))
                    {
                        sql += str.Trim() + ",";
                        try
                        {
                            intVar = Convert.ToInt32(param[str.Trim()]);
                            values += param[str.Trim()] + ",";
                        }
                        catch
                        {
                            values += "'" + param[str.Trim()].ToString().Trim() + "',";
                        }
                    }
                    else {
                        this.errors = "Список параметров не содержит поле ["+str+"]!";
                        return 0;
                    }
                }
            }
            else
            { 
                this.errors = "Список полей не должен быть пустым при альтернативном инсерте!";
                return 0;
            }

            sql = sql.Trim().Substring(0, (sql.Length - 1));
            values = values.Trim().Substring(0, (values.Trim().Length - 1));
            sql += ") VALUES (" + values + ")";
            try
            {
                this.sqlString = sql;
                OleDbCommand cmd = new OleDbCommand(sql, this.Connection);
                int res = cmd.ExecuteNonQuery();
                return res;
            }
            catch (Exception err)
            {
                this.errors = err.Message.ToString() + " || " + err.StackTrace;
                return 0;
            }
        }

        /// <summary>
        /// Обновление базы данных
        /// </summary>
        /// <param name="tblName">Название таблицы</param>
        /// <param name="param">Название столбцов и их значения в виде массива Dictionary</param>
        /// <param name="filter">Вильтр WHERE типа string. Пример указания: "id=1,name=\"Alex\"", без WHERE</param>
        /// <returns>Вернет количество измененных строк</returns>
        public int Update(string tblName,Dictionary<string,object> param, string filter) {
            string sql = "UPDATE " + tblName + " SET ";
            string paramString = "";
            int intVar = 0;
            foreach(KeyValuePair<string,object> kp in param){
                try
                {
                    intVar = Convert.ToInt32(kp.Value);
                    paramString += kp.Key + "=" + kp.Value + ",";
                }
                catch {
                    paramString += kp.Key + "='" + kp.Value + "',";
                }
            }
            paramString = paramString.Trim().Substring(0, (paramString.Trim().Length - 1));
            sql += paramString;
            if (filter.Trim().Length > 0) {
                sql += " " + filter.Trim();
            }
            try
            {
                this.sqlString = sql;
                OleDbCommand cmd = new OleDbCommand(sql,this.Connection);
                return cmd.ExecuteNonQuery();
            }
            catch(Exception err) {
                this.errors = err.Message.ToString() + " || " + err.StackTrace;
                return 0;
            }
        }

        /// <summary>
        /// Альтенативная функция обновления
        /// </summary>
        /// <param name="tblName">Название таблицы</param>
        /// <param name="param">Название столбцов и их значения в виде массива Dictionary</param>
        /// <param name="filter">Вильтр WHERE типа string. Пример указания: "id=1,name=\"Alex\"", без WHERE</param>
        /// <param name="fields">Список полее, которые нужно обновлять</param>
        /// <returns>Вернет количество измененных строк</returns>
        public int Update(string tblName, Dictionary<string, object> param, string filter, string fields)
        {
            string sql = "UPDATE " + tblName + " SET ";
            string paramString = "";
            int intVar = 0;

            // Формируем список заполняемых полей
            if (fields.Trim().Length > 0)
            {
                string[] field = fields.Trim().Split(",".ToCharArray());
                foreach (string str in field)
                {
                    Application.DoEvents();
                    if (param.Keys.Contains(str.Trim()))
                    {
                        try
                        {
                            intVar = Convert.ToInt32(param[str.Trim()]);
                            paramString += str.Trim() + "=" + param[str.Trim()] + ",";
                        }
                        catch
                        {
                            paramString += str.Trim() + "='" + param[str.Trim()] + "',";
                        }   
                    }
                    else
                    {
                        this.errors = "Список параметров не содержит поле [" + str + "]!";
                        return 0;
                    }
                }
            }
            else
            {
                this.errors = "Список полей не должен быть пустым при альтернативном обновление!";
                return 0;
            }

            paramString = paramString.Trim().Substring(0, (paramString.Trim().Length - 1));
            sql += paramString;
            if (filter.Trim().Length > 0)
            {
                sql += " " + filter.Trim();
            }
            try
            {
                this.sqlString = sql;
                OleDbCommand cmd = new OleDbCommand(sql, this.Connection);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                this.errors = err.Message.ToString() + " || " + err.StackTrace;
                return 0;
            }
        }

        /// <summary>
        /// Удаление нужной строки
        /// </summary>
        /// <param name="tblName">Имя таблицы в которой производим удаление</param>
        /// <param name="filter">Фильтрация используя WHERE</param>
        /// <returns></returns>
        public int Delete(string tblName,string filter) {
            try
            {
                string sql = "DELETE FROM " + tblName;
                if (filter.Trim().Length > 0)
                {
                    sql += " " + filter;
                }

                this.sqlString = sql;
                OleDbCommand cmd = new OleDbCommand(sql, this.Connection);
                int res = cmd.ExecuteNonQuery();
                if (res <= 0)
                {
                    this.errors = "Удаление не произведено!";
                }
                return res;
            }
            catch (Exception err) {
                this.errors = "Техническая ошибка: " + err.Message.ToString() + " || " + err.StackTrace;
                return 0;
            }
        }

        /// <summary>
        /// Селекторный запрос в стандартном формате SQL
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <returns>Вернет колекцию строк соответствующих запросу</returns>
        public DataRowCollection Query(string sql) {
            DataSet ds = new DataSet();
            try
            {
                OleDbDataAdapter da = new OleDbDataAdapter(sql, this.Connection);
                da.Fill(ds,"result");
                return ds.Tables["result"].Rows;
            }
            catch(Exception err) {
                this.errors = err.Message.ToString() + " || " + err.StackTrace;
                ds.Tables.Add("result");
                return ds.Tables["result"].Rows;
            }
        }

        /// <summary>
        /// Формирования выборки в формате HTML
        /// </summary>
        /// <param name="sql">Запрос на выборку из БД</param>
        /// <param name="columnsName">Список имен колонок в порядке выборки</param>
        /// <returns>Вернет все по строчно в формате HTML</returns>
        public string getInHTML(string sql, List<string> columnsName)
        {
            DataRowCollection rows = Query(sql);
            string html = "";
            if (rows[0].Table.Columns.Count == columnsName.Count && columnsName.Count > 0)
            {
                // Собираем шапку таблицы
                string columnsHead = "";
                foreach (string colName in columnsName) {
                    columnsHead += "<td style='border: 1px solid black; font-weight: bold;'>" + colName + "</td>";
                }

                html = "<table style='border: 1px solid black; border-collapse: collapse;'><tr style='background-color: grey;'>" + columnsHead + "</tr>";
                // Выводим строки в таблицу
                foreach (DataRow dr in rows) {
                    html += "<tr>";
                    for (int i = 0; i < columnsName.Count; i++) {
                        html += "<td style='border: 1px solid black;'>" + dr[i] + "</td>";
                    }
                    html += "</tr>";
                }
                html += "</table>";
                return html;
            }
            else {
                errors = "Количество имен колонок не соответсвует количеству выбраных столбцов из БД";
                return null;
            }
        }
    }
}
