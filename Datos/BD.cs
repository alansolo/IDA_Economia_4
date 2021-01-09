using Herramientas;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class BD
    {
        private SqlConnection sqlConnection;
        private SqlCommand command;
        protected string connectionString;

        public BD()
        {
            connectionString = ConfigurationManager.ConnectionStrings["IDA_Economia"].ToString();
        }
        public BD(string dataBaseConnection)
        {
            connectionString = ConfigurationManager.ConnectionStrings[dataBaseConnection].ToString();
        }
        protected object ExecuteScalar(string spName, List<SqlParameter> parameters)
        {
            object result = new object();
            //object uno = new object();
            StringBuilder str = new StringBuilder();

            //SqlDatabase db = new SqlDatabase(connectionString);
            //DbCommand command1; 

            DataSet dataSet = null;
            DataTable dataTable = new DataTable();

            using (sqlConnection = new SqlConnection(connectionString))
            //using (DbConnection con = db.CreateConnection())
            {
                try
                {
                    //command1 = con.CreateCommand();


                    command = new SqlCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    foreach (SqlParameter item in parameters)
                        command.Parameters.Add(item);

                    command.Connection = sqlConnection;
                    sqlConnection.Open();
                    //result = (string)db.ExecuteScalar(command1);
                    //uno = command.ExecuteReader();

                    //str.Append(db.ExecuteScalar(command1));

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataSet = new DataSet("dsResult");
                    dataAdapter.Fill(dataSet);

                    if (dataSet.Tables.Count > 0)
                    {
                        dataTable = dataSet.Tables[0];
                    }

                    result = dataTable;
                }
                catch (Exception ex)
                {
                    //Write log error
                    ArchivoLog.EscribirLog(null, DateTime.Now.ToString("dd/MM/yyyy mm:ss") + ": Method: " + ex.Source + " Error: " + ex.Message);
                    throw ex;
                }
            }

            return result;
        }
        protected object ExecuteScalarMultiple(string spName, List<SqlParameter> parameters)
        {
            object result = new object();
            //object uno = new object();
            StringBuilder str = new StringBuilder();

            //SqlDatabase db = new SqlDatabase(connectionString);
            //DbCommand command1; 

            DataSet dataSet = null;

            using (sqlConnection = new SqlConnection(connectionString))
            //using (DbConnection con = db.CreateConnection())
            {
                try
                {
                    //command1 = con.CreateCommand();


                    command = new SqlCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    foreach (SqlParameter item in parameters)
                        command.Parameters.Add(item);

                    command.Connection = sqlConnection;
                    sqlConnection.Open();
                    //result = (string)db.ExecuteScalar(command1);
                    //uno = command.ExecuteReader();

                    //str.Append(db.ExecuteScalar(command1));

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataSet = new DataSet("dsResult");
                    dataAdapter.Fill(dataSet);

                    result = dataSet;

                }
                catch (Exception ex)
                {
                    //Write log error
                    ArchivoLog.EscribirLog(null, DateTime.Now.ToString("dd/MM/yyyy mm:ss") + ": Method: " + ex.Source + " Error: " + ex.Message);
                    throw ex;
                }
            }

            return result;
        }
        protected string[] ExecuteScalar(string spName, List<SqlParameter> parameters, string spNameDetalle, List<SqlParameterGroup> parametersGroup)
        {
            string[] result = new string[2];
            SqlTransaction transaction;
            SqlParameter sqlParameterOut = new SqlParameter();
            SqlParameter sqlParameter = new SqlParameter();
            long idLog = 0;
            const string mensajeOK = "{\"MENSAJE\":\"OK\"}";

            using (sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                transaction = sqlConnection.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    command = new SqlCommand();
                    command.Connection = sqlConnection;
                    command.Transaction = transaction;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;

                    sqlParameter = new SqlParameter();
                    sqlParameter.ParameterName = "@IdLog";
                    sqlParameter.SqlDbType = SqlDbType.BigInt;
                    sqlParameter.Direction = ParameterDirection.Output;
                    command.Parameters.Add(sqlParameter);

                    foreach (SqlParameter item in parameters)
                        command.Parameters.Add(item);

                    result[0] = (string)command.ExecuteScalar();

                    if(result[0] != mensajeOK)
                    {
                        throw new Exception(result[0]);
                    }

                    //OBTENER ID LOG
                    sqlParameterOut = command.Parameters.Cast<SqlParameter>().Where(n => n.ParameterName == "@IdLog").FirstOrDefault();

                    if (sqlParameterOut != null && sqlParameterOut.Value != null)
                    {
                        idLog = Convert.ToInt32(sqlParameterOut.Value);
                    }

                    foreach (SqlParameterGroup paramGroup in parametersGroup)
                    {
                        try
                        {
                            command = new SqlCommand();
                            command.Connection = sqlConnection;
                            command.Transaction = transaction;
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = spNameDetalle;

                            //Agregar Id Log
                            sqlParameter = new SqlParameter();
                            sqlParameter.ParameterName = "IdLog";
                            sqlParameter.Value = idLog;

                            command.Parameters.Add(sqlParameter);

                            foreach (SqlParameter item in paramGroup.ListSqlParameter)
                                command.Parameters.Add(item);

                            result[1] = (string)command.ExecuteScalar();

                            if (result[1] != mensajeOK)
                            {
                                throw new Exception(result[0]);
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            //Write log error
                            ArchivoLog.EscribirLog(null, DateTime.Now.ToString("dd/MM/yyyy mm:ss") + ": Method: " + ex.Source + " Error: " + ex.Message);
                            throw ex;
                        }
                    }

                }
                catch (Exception ex)
                {
                    //Write log error
                    transaction.Rollback();
                    ArchivoLog.EscribirLog(null, DateTime.Now.ToString("dd/MM/yyyy mm:ss") + ": Method: " + ex.Source + " Error: " + ex.Message);
                    throw ex;
                }

                transaction.Commit();
            }

            return result;
        }
        protected int ExecuteNonQuery(string spName, List<SqlParameter> parameters)
        {
            int result = 0;

            using (sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    command = new SqlCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    foreach (SqlParameter item in parameters)
                        command.Parameters.Add(item);

                    command.Connection = sqlConnection;
                    sqlConnection.Open();
                    result = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    //Write log error
                    ArchivoLog.EscribirLog(null, DateTime.Now.ToString("dd/MM/yyyy mm:ss") + ": Method: " + ex.Source + " Error: " + ex.Message);
                    throw ex;
                }
            }

            return result;
        }
        protected DataTable ExecuteNonQueryDataTable(string spName, List<SqlParameter> parameters)
        {
            DataSet dataSet = null;
            DataTable dataTable = new DataTable();

            using (sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    command = new SqlCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;

                    foreach (SqlParameter item in parameters)
                        command.Parameters.Add(item);

                    command.Connection = sqlConnection;
                    sqlConnection.Open();

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataSet = new DataSet("dsResult");
                    dataAdapter.Fill(dataSet);

                    if (dataSet.Tables.Count > 0)
                    {
                        dataTable = dataSet.Tables[0];
                    }
                }
                catch (Exception ex)
                {
                    //Write log error
                    ArchivoLog.EscribirLog(null, DateTime.Now.ToString("dd/MM/yyyy mm:ss") + ": Method: " + ex.Source + " Error: " + ex.Message);
                    throw ex;
                }
            }

            return dataTable;
        }
        protected DataTable ExecuteDataTable(string spName, List<SqlParameterGroup> parametersGroup)
        {
            DataSet dataSet = null;
            DataTable dataTable = new DataTable();
            DataTable dtTotal = new DataTable();
            SqlTransaction transaction;

            using (sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                transaction = sqlConnection.BeginTransaction(IsolationLevel.ReadCommitted);

                foreach (SqlParameterGroup paramGroup in parametersGroup)
                {
                    try
                    {
                        command = new SqlCommand();
                        command.Connection = sqlConnection;
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = spName;

                        foreach (SqlParameter item in paramGroup.ListSqlParameter)
                            command.Parameters.Add(item);

                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        dataSet = new DataSet("dsResult");
                        dataAdapter.Fill(dataSet);

                        if (dataSet.Tables.Count > 0)
                        {
                            dataTable = dataSet.Tables[0];

                            dtTotal.Merge(dataTable);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        //Write log error
                        ArchivoLog.EscribirLog(null, DateTime.Now.ToString("dd/MM/yyyy mm:ss") + ": Method: " + ex.Source + " Error: " + ex.Message);
                        throw ex;
                    }
                }

                transaction.Commit();
            }

            return dtTotal;
        }
        protected DataTable ExecuteDataTable(string spName, List<SqlParameter> parameters)
        {
            DataSet dataSet = null;
            DataTable dataTable = new DataTable();

            using (sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    command = new SqlCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    foreach (SqlParameter item in parameters)
                        command.Parameters.Add(item);

                    command.Connection = sqlConnection;
                    sqlConnection.Open();

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataSet = new DataSet("dsResult");
                    dataAdapter.Fill(dataSet);

                    if (dataSet.Tables.Count > 0)
                    {
                        dataTable = dataSet.Tables[0];
                    }
                }
                catch (Exception ex)
                {
                    //Write log error
                    ArchivoLog.EscribirLog(null, DateTime.Now.ToString("dd/MM/yyyy mm:ss") + ": Method: " + ex.Source + " Error: " + ex.Message);
                    throw ex;
                }
            }

            return dataTable;
        }
        protected DataSet ExecuteDataSet(string spName, List<SqlParameter> parameters)
        {
            DataSet dataSet = null;

            using (sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    command = new SqlCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = spName;
                    foreach (SqlParameter item in parameters)
                        command.Parameters.Add(item);

                    command.Connection = sqlConnection;
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataSet = new DataSet("dsResult");
                    dataAdapter.Fill(dataSet);
                }
                catch (Exception ex)
                {
                    //Write log error
                    ArchivoLog.EscribirLog(null, DateTime.Now.ToString("dd/MM/yyyy mm:ss") + ": Method: " + ex.Source + " Error: " + ex.Message);
                    throw ex;
                }
            }

            return dataSet;
        }
        protected DataTable ExecuteDataTable(string query)
        {
            DataSet dataSet = null;
            DataTable dataTable = new DataTable();

            using (sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    command = new SqlCommand();
                    command.CommandType = CommandType.Text;
                    command.CommandText = query;

                    command.Connection = sqlConnection;
                    sqlConnection.Open();

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataSet = new DataSet("dsResult");
                    dataAdapter.Fill(dataSet);

                    if (dataSet.Tables.Count > 0)
                    {
                        dataTable = dataSet.Tables[0];
                    }
                }
                catch (Exception ex)
                {
                    //Write log error
                    ArchivoLog.EscribirLog(null, DateTime.Now.ToString("dd/MM/yyyy mm:ss") + ": Method: " + ex.Source + " Error: " + ex.Message);
                    throw ex;
                }
            }

            return dataTable;
        }
    }
}
