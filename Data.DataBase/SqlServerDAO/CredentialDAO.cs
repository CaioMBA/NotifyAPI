using Domain.Models;
using Domain.Models.GeneralSettings;
using Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataBase.SqlServerDAO
{
    public class CredentialDAO
    {
        private DataBaseDefaultAccess _db;
        private DataBaseConnections _con;
        private Utils _utils;
        public CredentialDAO(DataBaseDefaultAccess db, Utils utils)
        {
            _db = db;
            _utils = utils;
            _con = _utils.GetDataBase("StoreVault");
        }

        public List<CredentialInfoModel>? GetCredentials()
        {
            return _db.ExecutaQuery<CredentialInfoModel>("SELECT * FROM Credentials", null, _con).Result;
        }
    }
}
