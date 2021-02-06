﻿using SQLHelperLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialTransHelperLibrary
{
    public class ArtificialTransHelper
    {
        public SQLHelper sqlite;

        public ArtificialTransHelper(string gameName) {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\ArtificialTranslation"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\ArtificialTranslation");


            if (File.Exists(Environment.CurrentDirectory + "\\ArtificialTranslation\\MisakaAT_" + gameName + ".sqlite") == false)
            {
                CreateNewNounTransDB(gameName);
            }
            else
            {
                sqlite = new SQLHelper(Environment.CurrentDirectory + "\\ArtificialTranslation\\MisakaAT_" + gameName + ".sqlite");
            }
        }

        /// <summary>
        /// 添加一条翻译（一般是在游戏过程中机器自动添加翻译）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Trans"></param>
        /// <returns></returns>
        public bool AddTrans(string source,string Trans)
        {
            if (source == null || source == "" || Trans == null) {
                //空条目不添加，且返回假
                return false;
            }
            string sql =
                $"INSERT INTO tr VALUES(NULL,'{source}','{Trans}');";
            if (sqlite.ExecuteSql(sql) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 更新翻译（一般是人为的修改）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Trans"></param>
        /// <returns></returns>
        public bool UpdateTrans(string source, string Trans) {
            string sql =
                $"UPDATE tr SET trans = '{Trans}' WHERE id = " +
                $"(SELECT id from tr WHERE source = '{source}' ORDER BY id DESC LIMIT 1);";
            if (sqlite.ExecuteSql(sql) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 新建一个人工翻译数据库（一个游戏一个库）
        /// </summary>
        /// <param name="gameName"></param>
        private void CreateNewNounTransDB(string gameName)
        {
            SQLHelper.CreateNewDatabase(Environment.CurrentDirectory + "\\ArtificialTranslation\\MisakaAT_" + gameName + ".sqlite");
            sqlite = new SQLHelper(Environment.CurrentDirectory + "\\ArtificialTranslation\\MisakaAT_" + gameName + ".sqlite");
            sqlite.ExecuteSql("CREATE TABLE tr(id INTEGER PRIMARY KEY AUTOINCREMENT,source TEXT,trans TEXT);");
        }

        /// <summary>
        /// 将数据库内容按格式导出到文件以供他人使用
        /// </summary>
        public static bool ExportDBtoFile(string FilePath,string DBPath) {
            try
            {
                SQLHelper sqliteDB = new SQLHelper(DBPath);

                List<List<string>> ret = sqliteDB.ExecuteReader("SELECT * FROM tr ORDER BY id;", 3);

                FileStream fs = new FileStream(FilePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                for (int i = 0; i < ret.Count; i++)
                {
                    sw.WriteLine("<j>");
                    sw.WriteLine(ret[i][1]);
                    sw.WriteLine("<c>");
                    sw.WriteLine(ret[i][2]);
                }

                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch (Exception) {
                return false;
            }
            
            return true;
        }
    }
}
