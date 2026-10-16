using _21T1020851_Todo.DomainModels;

namespace _21T1020851_Todo.Web.Models
{
    public class PaginationSearchResult
    {

        /*#region Setting Page
        public int ViewPage { get; set; } = 1;
        public int TypeView { get; set; } = 1;
        public int StyleView { get; set; } = 1;
        #endregion*/

        #region Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
        public string SearchValue { get; set; } = "";
        public int RowCount { get; set; }
        public string TimeSearch { get; set; } = "";
        public int UserID { get; set; }
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 1;

                int n = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                    n += 1;
                return n;
            }
        }
        #endregion
    }
    public class TodoTaskSearchResult : PaginationSearchResult
    {
        public required List<TodoTask> DataTodos { get; set; }
        public required SettingView PageSetting { get; set; }
    }
}
