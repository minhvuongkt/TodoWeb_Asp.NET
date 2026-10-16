using _21T1020851_Todo.DataLayers;
using _21T1020851_Todo.DataLayers.SQLServer;
using _21T1020851_Todo.DomainModels;

namespace _21T1020851_Todo.BusinessLayers
{
    public static class CommonDataService
    {
        static readonly ICommonDAL<Account> accountDB;
        static readonly ICommonDAL<TodoTask> todoDB;
        static readonly ICommonDAL<SettingView> settingDB;
        static CommonDataService()
        {
            accountDB = new AccountDAL(Config.ConnectionString);
            todoDB = new TodoTaskDAL(Config.ConnectionString);
            settingDB = new SettingViewDAL(Config.ConnectionString);
        }
        #region Account
        public static int CreateAccount(Account account) => accountDB.Add(account); //đăng ký
        public static bool UpdateAccountInfo(Account account) => accountDB.Update(account); //cập nhật thông tin người dùng (tên, email, ...)
        public static bool UpdateAccountPassword(string UserID, string password) => accountDB.UpdateOther(UserID, password); //cập nhật mật khẩu
        public static bool DeleteAccount(int UserID) => accountDB.Delete(UserID); //xóa tài khoản
        public static Account? GetAccount(int UserID) => accountDB.Get(UserID); //lấy dữ liệu với id của tài khoản
        public static Account? GetUserWithName(string Username)//lấy dữ liệu với username của tài khoản
        {
            var db = new AccountDAL(Config.ConnectionString);
            if (db == null) return null;
            return db.GetUserWithName(Username); 
        }
        public static Account? LoginAccount(string UserName, string Password) => accountDB.GetOther(UserName, Password); //đăng nhập
        public static bool IsHaveDataOtherTable(int id) => accountDB.InUsed(id); //kiểm tra xem có được sử dụng ở bảng khác không
        #endregion
        #region TodoTask
        public static int CreateTodo(TodoTask todo) => todoDB.Add(todo);
        public static bool UpdateTodo(TodoTask todo) => todoDB.Update(todo);     
        public static bool UpdateFinishTodo(string todoID, string isFinish) => todoDB.UpdateOther(todoID, isFinish);
        public static bool DeleteTodo(int todoID) => todoDB.Delete(todoID);
        public static List<TodoTask> ListOfTodos(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "", string timeSearch = "", int UserID = 0, int typeView = 1)
        {
            rowCount = todoDB.Count(searchValue, timeSearch, UserID);
            return todoDB.List(page, pageSize, searchValue, timeSearch, UserID, typeView).ToList();
        }
        public static TodoTask? GetTodo(int todoID) => todoDB.Get(todoID);
        public static int TodoCounts(string searchValue = "", string timeSearch = "", int UserID = 0) => todoDB.Count(searchValue, timeSearch, UserID); 
        #endregion
        #region Setting
        public static int CreateSettingView(SettingView setting) => settingDB.Add(setting);
        public static SettingView? GetSetting(int UserID) => settingDB.Get(UserID);
        public static bool DeleteSetting(int UserID) => settingDB.Delete(UserID);
        public static bool UpdateSetting(SettingView setting) => settingDB.Update(setting);
        #endregion

    }
}
