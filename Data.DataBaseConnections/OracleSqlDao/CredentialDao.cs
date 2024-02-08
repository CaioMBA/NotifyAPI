using Data.DataBaseConnections;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseConnections.OracleSqlDao
{
    public class CredentialDao
    {
        private DefaultAccess _Sql;
        public CredentialDao(DefaultAccess Sql)
        {
            _Sql = Sql;
        }

        public List<CredentialsModel> GetCredentials()
        {
            var Response = _Sql.ExecutaOracleQuery<CredentialsModel>("SELECT NOTIFYCREDENTIALID, CREDENTIAL, PASSWORD, TYPE, PROTOCOL, ACTIVE FROM NOTIFYCREDENTIALS WHERE ACTIVE = 1", null).Result;

            return Response;
        }  
    }
}
