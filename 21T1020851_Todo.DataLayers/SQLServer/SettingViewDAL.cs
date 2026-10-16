using _21T1020851_Todo.DomainModels;
using Dapper;
using System.Data;

namespace _21T1020851_Todo.DataLayers.SQLServer
{
    public class SettingViewDAL : _BaseDAL, ICommonDAL<SettingView>
    {
        public SettingViewDAL(string connectionString) : base(connectionString)
        {
        }
        public int Add(SettingView item)
        {
            int id = 0;
            using (var conn = OpenConnection())
            {
                var param = new DynamicParameters();
                param.Add("@UserID", item.UserID);
                id = conn.Execute("sp_AddSettingView", param: param, commandType: CommandType.StoredProcedure);
                conn.Close();
            }
            return id;
        }

        public bool Delete(int id)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                var param = new DynamicParameters();
                param.Add("@UserID", id);
                result = conn.Execute("sp_DeleteSettingView", param: param, commandType: CommandType.StoredProcedure) > 0;
                conn.Close();
                return result;
            }
        }

        public SettingView? Get(int id)
        {
            var setting = new SettingView();
            using (var conn = OpenConnection())
            {
                var param = new DynamicParameters();
                param.Add("@UserID", id);
                setting = conn.QueryFirstOrDefault<SettingView>("sp_GetSettingView", param: param, commandType: CommandType.StoredProcedure);
                conn.Close();
            }
            return setting;
        }

        public bool Update(SettingView item)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                var param = new DynamicParameters();
                param.Add("@UserID", item.UserID);
                param.Add("@ViewPage", item.ViewPage);
                param.Add("@StyleView", item.StyleView);
                param.Add("@TypeView", item.TypeView);
                result = conn.Execute("sp_UpdateSettingView", param: param, commandType: CommandType.StoredProcedure) > 0;
                conn.Close();
                return result;
            }
        }

        #region Not Use
        public bool UpdateOther(string item1, string item2)
        {
            throw new NotImplementedException();
        }
        public bool InUsed(int id)
        {
            throw new NotImplementedException();
        }
        public int Count(string searchValue = "", string timeSearch = "", int UserID = 0)
        {
            throw new NotImplementedException();
        }
        public IList<SettingView> List(int page = 1, int pageSize = 0, string searchValue = "", string timeSearch = "", int UserID = 0, int typeView = 1)
        {
            throw new NotImplementedException();
        }

        public SettingView? GetOther(string item1, string item2)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
