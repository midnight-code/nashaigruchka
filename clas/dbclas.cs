using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Text;

namespace igruchka7.clas
{
    class dbclas
    {
        public void inserdb(string commandString, string ConnectionString)
        {
            OleDbCommand insertCommand = new OleDbCommand(commandString);
            insertCommand.Connection = new OleDbConnection(ConnectionString);
            insertCommand.Connection.Open();
            try
            {
                insertCommand.ExecuteNonQuery();
            }
            finally
            {
                insertCommand.Connection.Close();
            }
        }
        public DataSet selectdb(string commandString, string ConnectionString)
        {
            DataSet nds = new DataSet();
            OleDbDataAdapter sqlDa = new OleDbDataAdapter();
            sqlDa.SelectCommand = new OleDbCommand();
            sqlDa.SelectCommand.Connection = new OleDbConnection(ConnectionString);
            sqlDa.SelectCommand.Connection.Open();
            sqlDa.SelectCommand.CommandText = commandString;
            sqlDa.Fill(nds);
            sqlDa.SelectCommand.Connection.Close();
            return nds;
        }
        public void deletedb(string commandString, string ConnectionString)
        {
            OleDbCommand deleteCommand = new OleDbCommand(commandString);
            deleteCommand.Connection = new OleDbConnection(ConnectionString);
            deleteCommand.Connection.Open();
            try
            {
                deleteCommand.ExecuteNonQuery();
            }
            finally
            {
                deleteCommand.Connection.Close();
            }
        }
    }
}
