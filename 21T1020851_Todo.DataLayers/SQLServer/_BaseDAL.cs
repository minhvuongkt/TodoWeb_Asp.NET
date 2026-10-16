using Microsoft.Data.SqlClient;

namespace _21T1020851_Todo.DataLayers.SQLServer
{
    /// <summary>
    /// Lớp đóng vai trò là lớp cha cho các lớp cài đặt các phép xử lý dữ liệu
    /// trên csdl sql server
    /// </summary>
    public abstract class _BaseDAL
    {
        protected string _connectionString = "";
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionString"></param>
        protected _BaseDAL(string connectionString)
        {
            this._connectionString = connectionString;
        }
        /// <summary>
        /// Tạo và mở kết nối đến csdl sql server
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
