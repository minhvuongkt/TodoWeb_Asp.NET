using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21T1020851_Todo.DataLayers
{
    /// <summary>
    /// Định nghĩa các phép xử lý dữ liệu chung
    /// </summary>
    /// <typeparam name="T"> Kiểu dữ liệu của đối tượng cần xử lý </typeparam>
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        /// tìm kiếm và lấy danh sách hiển thị dưới dạng phân trang
        /// </summary>
        /// <param name="page">trang cần hiển thị</param>
        /// <param name="pageSize">số dòng hiển thị trên mỗi trang</param>
        /// <param name="searchValue">giá trị tìm kiếm trong trang (rỗng = không tìm kiếm)</param>
        /// <returns></returns>
        IList<T> List(int page = 1, int pageSize = 0, string searchValue = "", string timeSearch = "", int UserID = 0, int typeView = 1);
        /// <summary>
        /// Đếm số dòng tìm kiếm được
        /// </summary>
        /// <param name="searchValue">giá trị tìm kiếm trong trang (rỗng = không tìm kiếm)</param>
        /// <returns></returns>
        int Count(string searchValue = "", string timeSearch = "", int UserID = 0);
        /// <summary>
        /// Lấy một bảng ghi/dòng dữ liệu dựa trên ID của dữ liệu đó
        /// </summary>
        /// <param name="id">Mã dữ liệu cần lấy</param>
        /// <returns></returns>
        T? Get(int id);
        T? GetOther(string item1, string item2);
        /// <summary>
        /// Bổ sung dữ liệu vào cơ sở dữ liệu. Hàm trả về id (mã) của dữ liệu được bổ sung.
        /// </summary>
        /// <param name="item">Dữ liệu cần bổ sung</param>
        /// <returns></returns>
        int Add(T item);
        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Update(T item);
        bool UpdateOther(string item1, string item2);
        /// <summary>
        /// Xoá 1 dòng dữ liệu/bảng ghi với TaskID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);
        /// <summary>
        /// Kiểm tra xem một dòng dữ liệu có TaskID hiện đang có dữ liệu liên quan ở các bảng khác hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);

    }
}
