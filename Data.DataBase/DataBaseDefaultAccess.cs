using Dapper;
using Domain.Models.GeneralSettings;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Mysqlx.Crud;

namespace Data.DataBase
{
    public class DataBaseDefaultAccess
    {
        IDbConnection? con = null;


        #region Relational Data Base


        public IDbConnection conectar(DataBaseConnections bd)
        {
            if (bd.Type == "SQLSERVER")
            {
                con = new SqlConnection(bd.ConnectionString);
            }
            else if (bd.Type == "ORACLE")
            {
                con = new OracleConnection(bd.ConnectionString);
            }
            else if (bd.Type == "MYSQL")
            {
                con = new MySqlConnection(bd.ConnectionString);
            }
            else if (bd.Type == "POSTGRESQL")
            {
                con = new NpgsqlConnection(bd.ConnectionString);
            }

            if (con.State == System.Data.ConnectionState.Closed)
            {
                if (String.IsNullOrEmpty(con.ConnectionString))
                {
                    con.ConnectionString = con.ConnectionString = bd.ConnectionString;
                }
                con.Open();
            }

            return con;
        }

        public async Task<T?> ExecutaProcFirstOrDefault<T>(string sQuery, object parametro, DataBaseConnections? conexao)
        {
            using (var _DBConnection = conectar(conexao))
            {
                return _DBConnection.QueryFirstOrDefault<T>(sQuery, parametro, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<T>?> ExecutaProc<T>(string sQuery, object parametro, DataBaseConnections? conexao)
        {
            using (var _DBConnection = conectar(conexao))
            {
                return _DBConnection.Query<T>(sQuery, parametro, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        public async Task<List<dynamic>?> ExecutaProc_QueryMultipleTable(string sQuery, object parametro, DataBaseConnections? conexao)
        {
            List<dynamic> objReturn = new List<dynamic>();
            using (var _DBConnection = conectar(conexao))
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


        public async Task<List<T>?> ExecutaQuery<T>(string sQuery, object parametro, DataBaseConnections? conexao)
        {
            using (var _DBConnection = conectar(conexao))
            {
                return _DBConnection.Query<T>(sQuery, parametro).ToList();
            }
        }

        public async Task<T?> ExecutaFirstOrDefault<T>(string sQuery, object parametro, DataBaseConnections? conexao)
        {
            using (var _DBConnection = conectar(conexao))
            {
                return _DBConnection.QueryFirstOrDefault<T>(sQuery, parametro);
            }
        }

        #endregion

        #region Non Relational Data Base

        #region insert
        public string? InsertOne(string sJson, DataBaseConnections MongoConfig)
        {
            MongoClient dbClient = new MongoClient(MongoConfig.ConnectionString);
            var database = dbClient.GetDatabase(MongoConfig.Name);
            var collection = database.GetCollection<BsonDocument>(MongoConfig.Collection);
            MongoDB.Bson.BsonDocument document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(sJson);
            collection.InsertOne(document);
            return document["_id"].ToString();
        }

        public List<string> InsertMany(string sJson, DataBaseConnections MongoConfig)
        {
            MongoClient dbClient = new MongoClient(MongoConfig.ConnectionString);
            var database = dbClient.GetDatabase(MongoConfig.Name);
            var collection = database.GetCollection<BsonDocument>(MongoConfig.Collection);
            List<MongoDB.Bson.BsonDocument> document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<List<BsonDocument>>(sJson);
            collection.InsertManyAsync(document).Wait();

            return (from v in document select v["_id"].ToString()).ToList();
        }
        #endregion

        #region Find

        public MongoDB.Bson.BsonDocument? FindFirstOrDefaultByMongoID(string ID, DataBaseConnections MongoConfig)
        {
            MongoClient dbClient = new MongoClient(MongoConfig.ConnectionString);
            var database = dbClient.GetDatabase(MongoConfig.Name);
            var collection = database.GetCollection<BsonDocument>(MongoConfig.Collection);
            //var firstDocument = collection.Find(new BsonDocument()).FirstOrDefault();

            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(ID));

            var Document = collection.Find(filter).FirstOrDefault();

            if (Document == null)
                return null;

            return Document;
        }
        public MongoDB.Bson.BsonDocument? FindFirstOrDefault(NonRelationalDataBaseSearchConfig SearchSettings, DataBaseConnections MongoConfig)
        {
            MongoClient dbClient = new MongoClient(MongoConfig.ConnectionString);
            var database = dbClient.GetDatabase(MongoConfig.Name);
            var collection = database.GetCollection<BsonDocument>(MongoConfig.Collection);
            //var firstDocument = collection.Find(new BsonDocument()).FirstOrDefault();


            var projectionBuilder = Builders<BsonDocument>.Projection;
            ProjectionDefinition<BsonDocument>? projection = null;
            FilterDefinition<BsonDocument> finalFilter = Builders<BsonDocument>.Filter.Empty;

            foreach (string tag in SearchSettings.ExcludeTags)
            {
                if (projection == null)
                {
                    projection = projectionBuilder.Exclude(tag);
                }
                else
                {
                    projection = projection.Exclude(tag);
                }
            }
            foreach (string tag in SearchSettings.IncludeTags)
            {
                if (projection == null)
                {
                    projection = projectionBuilder.Include(tag);
                }
                else
                {
                    projection = projection.Include(tag);
                }
            }
            foreach (var kvp in SearchSettings.filters)
            {
                finalFilter &= Builders<BsonDocument>.Filter.Eq(kvp.Key, kvp.Value);
            }
            /*if (projection == null)
            {
                projection = projectionBuilder.Exclude(""); //ISSO É PRA QUE NÃO RETIRE NADA
            }*/

            if (String.IsNullOrEmpty(SearchSettings.SortIndex))
            {
                SearchSettings.SortIndex = "_id";
            }
            if (SearchSettings.Limit == null || SearchSettings.Limit < 1)
            {
                SearchSettings.Limit = -1;
            }
            SortDefinition<BsonDocument>? sort = null;
            if (SearchSettings.SortDescending != null || SearchSettings.SortDescending == false)
            {
                sort = Builders<BsonDocument>.Sort.Descending(SearchSettings.SortIndex);
            }
            else
            {
                sort = Builders<BsonDocument>.Sort.Ascending(SearchSettings.SortIndex);
            }


            var Document = collection.Find(finalFilter)
                                            .Project(projection)
                                            .Sort(sort)
                                            .Limit(SearchSettings.Limit)
                                            .FirstOrDefault();

            if (Document == null)
                return null;

            /* var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.CanonicalExtendedJson };
             var json = JObject.Parse(Document.ToJson<MongoDB.Bson.BsonDocument>(jsonWriterSettings));

             return json.ToString().Replace("$oid", "_id").Replace("\r", "").Replace("\n", "").Replace("\\&", "");*/
            return Document;
        }

        public List<MongoDB.Bson.BsonDocument>? Find(NonRelationalDataBaseSearchConfig SearchSettings, DataBaseConnections MongoConfig)
        {
            MongoClient dbClient = new MongoClient(MongoConfig.ConnectionString);
            var database = dbClient.GetDatabase(MongoConfig.Name);
            var collection = database.GetCollection<BsonDocument>(MongoConfig.Collection);

            var projectionBuilder = Builders<BsonDocument>.Projection;
            ProjectionDefinition<BsonDocument>? projection = null;
            FilterDefinition<BsonDocument> finalFilter = Builders<BsonDocument>.Filter.Empty;

            foreach (string tag in SearchSettings.ExcludeTags)
            {
                if (projection == null)
                {
                    projection = projectionBuilder.Exclude(tag);
                }
                else
                {
                    projection = projection.Exclude(tag);
                }
            }
            foreach (string tag in SearchSettings.IncludeTags)
            {
                if (projection == null)
                {
                    projection = projectionBuilder.Include(tag);
                }
                else
                {
                    projection = projection.Include(tag);
                }
            }
            foreach (var kvp in SearchSettings.filters)
            {
                finalFilter &= Builders<BsonDocument>.Filter.Eq(kvp.Key, kvp.Value);
            }
            /*if (projection == null)
            {
                projection = projectionBuilder.Exclude(""); //ISSO É PRA QUE NÃO RETIRE NADA
            }*/

            if (String.IsNullOrEmpty(SearchSettings.SortIndex))
            {
                SearchSettings.SortIndex = "_id";
            }
            if (SearchSettings.Limit == null || SearchSettings.Limit < 1)
            {
                SearchSettings.Limit = -1;
            }
            SortDefinition<BsonDocument>? sort = null;
            if (SearchSettings.SortDescending != null || SearchSettings.SortDescending == true)
            {
                sort = Builders<BsonDocument>.Sort.Descending(SearchSettings.SortIndex);
            }
            else
            {
                sort = Builders<BsonDocument>.Sort.Ascending(SearchSettings.SortIndex);
            }


            var Documents = collection.Find(finalFilter)
                                            .Project(projection)
                                            .Sort(sort)
                                            .Limit(SearchSettings.Limit)
                                            .ToList();
            if (Documents == null)
            {
                return null;
            }

            return Documents;
        }

        public List<MongoDB.Bson.BsonDocument>? FindFullCollection(DataBaseConnections MongoConfig)
        {
            MongoClient dbClient = new MongoClient(MongoConfig.ConnectionString);
            var database = dbClient.GetDatabase(MongoConfig.Name);
            var collection = database.GetCollection<BsonDocument>(MongoConfig.Collection);

            var documents = collection.Find(new BsonDocument()).ToList();

            return documents;
        }

        #endregion

        #region update
        public Dictionary<string, string> UpdateOne(FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update, DataBaseConnections MongoConfig)
        {
            MongoClient dbClient = new MongoClient(MongoConfig.ConnectionString);
            var database = dbClient.GetDatabase(MongoConfig.Name);
            var collection = database.GetCollection<BsonDocument>(MongoConfig.Collection);
            Dictionary<string, string> dicReturn = new Dictionary<string, string>();

            var xRet = collection.UpdateOne(filter, update);
            dicReturn.Add("ModifiedCount", xRet.ModifiedCount.ToString());
            return dicReturn;
        }

        public void UpdateOne(FilterDefinition<BsonDocument> filter, string sUpdate, DataBaseConnections MongoConfig)
        {
            MongoClient dbClient = new MongoClient(MongoConfig.ConnectionString);
            var database = dbClient.GetDatabase(MongoConfig.Name);
            var collection = database.GetCollection<BsonDocument>(MongoConfig.Collection);

            collection.UpdateOne(filter, sUpdate);

        }

        public void ReplaceOne(string filter, string sUpdate, DataBaseConnections MongoConfig)
        {
            MongoClient dbClient = new MongoClient(MongoConfig.ConnectionString);
            var database = dbClient.GetDatabase(MongoConfig.Name);
            var collection = database.GetCollection<BsonDocument>(MongoConfig.Collection);
            MongoDB.Bson.BsonDocument document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(sUpdate);

            collection.ReplaceOne(filter, document);
        }

        #endregion

        #endregion
    }
}
