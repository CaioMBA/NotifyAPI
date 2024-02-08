using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Domain.Models;
using Domain.Models.Settings;
using Domain.Utils;
using MySqlConnector;
using Oracle.ManagedDataAccess.Client;
using static Dapper.SqlMapper;

namespace Data.DataBaseConnections
{
    public class DefaultAccess
    {
        private string _ConnectionString;
        public DefaultAccess(AppSettingsModel config, Utils utils)
        {
            var repositoryConfig = utils.GetDataBase("OracleSQL");
            _ConnectionString = repositoryConfig.ConnectionString;
        }

        public DataSet ExecuteMySqlDataSet(string sQuery)
        {
            using (var _DBConnection = new MySqlConnection(_ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(sQuery, _DBConnection))
                {
                    cmd.CommandTimeout = 30;
                    using (MySqlDataAdapter DBAdapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        DBAdapter.Fill(ds);
                        return ds;
                    }
                }
            }
        }

        public DataSet ExecuteOracleDataSet(string sQuery)
        {
            using (var _DBConnection = new OracleConnection(_ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(sQuery, _DBConnection))
                {
                    cmd.CommandTimeout = 30;
                    using (OracleDataAdapter DBAdapter = new OracleDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        DBAdapter.Fill(ds);
                        return ds;
                    }
                }
            }
        }

        public DataSet ExecuteSqlServerDataSet(string sQuery)
        {
            using (var _DBConnection = new SqlConnection(_ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sQuery, _DBConnection))
                {
                    cmd.CommandTimeout = 30;
                    using (SqlDataAdapter DBAdapter = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        DBAdapter.Fill(ds);
                        return ds;
                    }
                }
            }
        }

        public async Task<T> ExecutaMySqlProcFirstOrDefault<T>(string sQuery, object parametro)
        {
            using (var _DBConnection = new MySqlConnection(_ConnectionString))
            {
                return _DBConnection.QueryFirstOrDefault<T>(sQuery, parametro, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<T> ExecutaOracleProcFirstOrDefault<T>(string sQuery, object parametro)
        {
            using (var _DBConnection = new OracleConnection(_ConnectionString))
            {
                return _DBConnection.QueryFirstOrDefault<T>(sQuery, parametro, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<T> ExecutaSqlServerProcFirstOrDefault<T>(string sQuery, object parametro)
        {
            using (var _DBConnection = new SqlConnection(_ConnectionString))
            {
                return _DBConnection.QueryFirstOrDefault<T>(sQuery, parametro, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<T>> ExecutaMySqlProc<T>(string sQuery, object parametro)
        {
            using (var _DBConnection = new MySqlConnection(_ConnectionString))
            {
                return _DBConnection.Query<T>(sQuery, parametro, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public async Task<List<T>> ExecutaOracleProc<T>(string sQuery, object parametro)
        {
            using (var _DBConnection = new OracleConnection(_ConnectionString))
            {
                return _DBConnection.Query<T>(sQuery, parametro, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public async Task<List<T>> ExecutaSqlServerProc<T>(string sQuery, object parametro)
        {
            using (var _DBConnection = new SqlConnection(_ConnectionString))
            {
                return _DBConnection.Query<T>(sQuery, parametro, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public async Task<List<object>> ExecutaMySqlProc_QueryMultipleTables(string sQuery, object parametro)
        {
            List<dynamic> objReturn = new List<dynamic>();
            using (var _DBConnection = new MySqlConnection(_ConnectionString))
            {
                var result = _DBConnection.QueryMultiple(sQuery, parametro, commandType: CommandType.StoredProcedure);


                while (!result.IsConsumed)
                {
                    try
                    {
                        objReturn.Add(result.Read<dynamic>());
                    }
                    catch
                    {
                        return null;
                    }

                }
            }

            return objReturn;
        }

        public async Task<List<object>> ExecutaOracleProc_QueryMultipleTables(string sQuery, object parametro)
        {
            List<dynamic> objReturn = new List<dynamic>();
            using (var _DBConnection = new OracleConnection(_ConnectionString))
            {
                var result = _DBConnection.QueryMultiple(sQuery, parametro, commandType: CommandType.StoredProcedure);


                while (!result.IsConsumed)
                {
                    try
                    {
                        objReturn.Add(result.Read<dynamic>());
                    }
                    catch
                    {
                        return null;
                    }

                }
            }

            return objReturn;
        }

        public async Task<List<object>> ExecutaSqlServerProc_QueryMultipleTables(string sQuery, object parametro)
        {
            List<dynamic> objReturn = new List<dynamic>();
            using (var _DBConnection = new SqlConnection(_ConnectionString))
            {
                var result = _DBConnection.QueryMultiple(sQuery, parametro, commandType: CommandType.StoredProcedure);


                while (!result.IsConsumed)
                {
                    try
                    {
                        objReturn.Add(result.Read<dynamic>());
                    }
                    catch
                    {
                        return null;
                    }

                }
            }

            return objReturn;
        }

        public async Task<List<T>> ExecutaMySqlQuery<T>(string sQuery, object parametro)
        {
            using (var _DBConnection = new MySqlConnection(_ConnectionString))
            {
                return _DBConnection.Query<T>(sQuery, parametro).ToList();
            }
        }

        public async Task<List<T>> ExecutaOracleQuery<T>(string sQuery, object parametro)
        {
            using (var _DBConnection = new OracleConnection(_ConnectionString))
            {
                return _DBConnection.Query<T>(sQuery, parametro).ToList();
            }
        }

        public async Task<List<T>> ExecutaSqlServerQuery<T>(string sQuery, object parametro)
        {
            using (var _DBConnection = new SqlConnection(_ConnectionString))
            {
                return _DBConnection.Query<T>(sQuery, parametro).ToList();
            }
        }

        public async Task<T> ExecutaOracleFirstOrDefault<T>(string sQuery, object parametro)
        {
            using (var _DBConnection = new OracleConnection(_ConnectionString))
            {
                return _DBConnection.QueryFirstOrDefault<T>(sQuery, parametro);
            }
        }

        public async Task<T> ExecutaSqlServerFirstOrDefault<T>(string sQuery, object parametro)
        {
            using (var _DBConnection = new SqlConnection(_ConnectionString))
            {
                return _DBConnection.QueryFirstOrDefault<T>(sQuery, parametro);
            }
        }
    }
}
