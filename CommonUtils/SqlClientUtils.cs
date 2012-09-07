using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace CommonUtils
{
    public static class SqlClientUtils
    {
        public static SqlCommand GetCommandForStoredProcedure(SqlConnection connection, string spName
            , ICollection<KeyValuePair<string, object>> inParams, ICollection<string> outParamNames)
        {
            var command = new SqlCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;

            if (inParams != null)
            {
                foreach (var inputParam in inParams)
                    command.Parameters.AddWithValue(inputParam.Key, inputParam.Value);
            }

            if (outParamNames != null)
            {
                foreach (var outputParam in outParamNames)
                {
                    var param = command.CreateParameter();
                    param.ParameterName = outputParam;
                    param.Direction = ParameterDirection.Output;
                    command.Parameters.Add(param);
                }
            }

            return command;
        }

        public static SqlConnection GetConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }


        public static ICollection<KeyValuePair<string, object>> ExecuteStoredProcedure(string connectionString, string spName
            , ICollection<KeyValuePair<string, object>> inParams = null, ICollection<string> outParamNames = null)
        {
            DataSet resultSets;
            ICollection<KeyValuePair<string, object>> outparams;
            ExecuteStoredProcedure(connectionString, spName, inParams, outParamNames, false, out outparams, out resultSets);

            return outparams;
        }

        public static DataSet ExecuteStoredProcedure(string connectionString, string spName
            , ICollection<KeyValuePair<string, object>> inParams = null)
        {
            DataSet resultSets;
            ICollection<KeyValuePair<string, object>> outparams;
            ExecuteStoredProcedure(connectionString, spName, inParams, null, true, out outparams, out resultSets);

            return resultSets;
        }

        public static void ExecuteStoredProcedure(string connectionString,  string spName
            , ICollection<KeyValuePair<string, object>> inParams, ICollection<string> outParamNames, bool expectResultSets
            , out ICollection<KeyValuePair<string, object>> outParams, out DataSet resultSets)
        {
            using (var connection = GetConnection(connectionString))
            {
                using (var command = GetCommandForStoredProcedure(connection, spName, inParams, outParamNames))
                {
                    if (!expectResultSets)
                    {
                        command.ExecuteNonQuery();
                        resultSets = null;
                    }
                    else
                    {
                        var adapter = new SqlDataAdapter(command);
                        resultSets = new DataSet();
                        adapter.Fill(resultSets);
                    }

                    outParams = outParamNames == null
                        ? null
                        : outParamNames.Select(paramName => new KeyValuePair<string, object>(paramName, command.Parameters[paramName].Value)).ToList();
                }
            }
        }

        public static IEnumerable<TEntity> ToEntities<TEntity>(this DataTable table, ICollection<string> propertiesToSet) where TEntity : new()
        {
            var settableProperties = EntitySettableProperties<TEntity>.SettableProperties;

            foreach (DataRow row in table.Rows)
            {
                var entity = new TEntity();
                foreach(var propertyName in propertiesToSet)
                {
                     settableProperties[propertyName].SetValue(entity, row[propertyName]);
                }

                yield return entity;
            }

        }

        private static class EntitySettableProperties<TEntity>
        {
            public static readonly IReadOnlyDictionary<string, PropertyInfo> SettableProperties = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.CanWrite).ToDictionary(p => p.Name, p => p);
        }
    }
}