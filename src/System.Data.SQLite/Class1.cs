// DUMMY LIBRARY FOR COMPILE TIME, USE T7 SQL LIB
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.SQLite
{
    public class SQLiteConnection
    {
        public SQLiteConnection(string dummy)
        {

        }

        public void Open()
        {

        }
    }

    public class SQLiteCommand
    {
        public SQLiteCommand(string dummy, SQLiteConnection dummy2)
        {

        }

        public SQLiteDataReader ExecuteReader()
        {
            return new SQLiteDataReader();
        }
    }

    public class SQLiteDataReader
    {
        public SQLiteDataReader()
        {

        }

        public bool Read()
        {
            return false;
        }
    }
}
