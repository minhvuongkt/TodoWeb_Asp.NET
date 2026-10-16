using _21T1020851_Todo.DomainModels;
using Dapper;
using System.Data;

namespace _21T1020851_Todo.DataLayers.SQLServer
{
    public class AccountDAL : _BaseDAL, ICommonDAL<Account>
    {
        public AccountDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Account user)
        {
            var id = 0;
            using (var conn = OpenConnection())
            {
                var param = new DynamicParameters();
                param.Add("@Username", user.Username);
                param.Add("@Password", user.Password);
                param.Add("@FullName", user.FullName);
                param.Add("@Email", user.Email);
                id = conn.ExecuteScalar<int>("sp_AddNewAccount", param: param, commandType: CommandType.StoredProcedure);
                conn.Close();
            }
            return id;
        }

        public bool InUsed(int id)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                var param = new DynamicParameters();
                param.Add("@UserID", id);
                result = conn.ExecuteScalar<int>("sp_CheckAccountUsage", param: param, commandType: CommandType.StoredProcedure) > 0;
                conn.Close();
            }
            return result;
        }

        private void DeleteTodo(int id)
        {
            using (var conn = OpenConnection())
            {
                var sql = @"delete from TodoTasks where UserID = @UserID; ";
                var param = new DynamicParameters();
                param.Add("@UserID", id);
                conn.Execute(sql, param: param, commandType: CommandType.Text);
                conn.Close();
            }
        }
        private bool DeleteSetting(int id)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                var sql = "delete from SettingView where UserID = @UserID;";
                var param = new DynamicParameters();
                param.Add("@UserID", id);
                result = conn.Execute(sql, param: param, commandType: CommandType.Text) > 0;
                conn.Close();
            }
            return result;
        }
        public bool Delete(int id)
        {
            var result = false;
            using (var conn = OpenConnection())
            { 
                DeleteTodo(id);
                if (DeleteSetting(id))
                {
                    var param = new DynamicParameters();
                    param.Add("@UserID", id);
                    result = conn.Execute("sp_DeleteAccount", param: param, commandType: CommandType.StoredProcedure) > 0;
                    conn.Close();
                }
            }
            return result;
        }

        public Account? GetUserWithName(string username)
        {
            var acc = new Account();
            using (var conn = OpenConnection())
            {
                var sql = "select * from Accounts where Username = @Username";
                var param = new DynamicParameters();
                param.Add("@Username", username);
                acc = conn.QueryFirstOrDefault<Account>(sql: sql, param: param, commandType: CommandType.Text);
                conn.Close();
            }
            return acc;
        }
        public Account? GetOther(string user, string password)
        {
            var acc = new Account();
            using (var conn = OpenConnection())
            {
                //var sql = "select * from Accounts where Username = @Username and Password = @Password";
                var param = new DynamicParameters();
                param.Add("@Username", user);
                param.Add("@Password", password);
                acc = conn.QueryFirstOrDefault<Account>("sp_LoginAccount", param: param, commandType: CommandType.StoredProcedure);
                //acc = conn.QueryFirstOrDefault<Account>(sql: sql, param: param, commandType: CommandType.Text);
                conn.Close();
            }
            return acc;
        }
        public Account? Get(int id)
        {
            var user = new Account();
            using (var conn = OpenConnection())
            {
                var sql = "select * from Accounts where UserID = @UserID";
                var param = new DynamicParameters();
                param.Add("@UserID", id);
                user = conn.QueryFirstOrDefault<Account>(sql: sql, param, commandType: CommandType.Text);
                conn.Close();
            }
            return user;
        }
        public bool Update(Account item)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                /*var sql = @"update Accounts set FullName = @FullName, Email = @Email 
                                          where UserID = @UserID;";*/
                var param = new DynamicParameters();
                param.Add("@UserID", item.UserID);
                param.Add("@FullName", item.FullName);
                param.Add("@Email", item.Email);
                //result = conn.Execute(sql: sql, param: param, commandType: CommandType.Text) > 0;
                result = conn.Execute("sp_UpdateAccountInfo", param: param, commandType: CommandType.StoredProcedure) > 0;
                conn.Close();
            }
            return result;
        }
        public bool UpdateOther(string UserID, string Password)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                //var sql = "update Accounts set Password = @Password where UserID = @UserID;";
                var param = new DynamicParameters();
                param.Add("@UserID", UserID);
                param.Add("@Password", Password);
                //result = conn.Execute(sql: sql, param: param, commandType: CommandType.Text) > 0;
                result = conn.Execute("sp_UpdateAccountPassword", param: param, commandType: CommandType.Text) > 0;
                conn.Close();
            }
            return result;
        }
        #region Not Use
        public IList<Account> List(int page = 1, int pageSize = 0, string searchValue = "", string timeSearch = "", int UserID = 0, int typeView = 1)
        {
            throw new NotImplementedException();
        }

        public int Count(string searchValue = "", string timeSearch = "", int UserID = 0)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
