using _21T1020851_Todo.BusinessLayers;
using _21T1020851_Todo.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _21T1020851_Todo.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        int PAGE_SIZE { get; set; } = 0;
        public IActionResult Index(int page = 1, string searchValue = "", string timeSearch = "")
        {
            ViewBag.Title = "Trang chủ";
            int? nullableInt = HttpContext.Session.GetInt32("UserID");
            if (nullableInt.HasValue && nullableInt > 0)
            {
                int userId = nullableInt.Value;
                int rowCount = 0;
                var settingView = CommonDataService.GetSetting(userId);
                if (settingView == null)
                {
                    return RedirectToAction("Logout", "User");
                }
                if (HttpContext.Session.GetInt32("PageSize").HasValue)
                {
                    PAGE_SIZE = HttpContext.Session.GetInt32("PageSize").Value;
                }
                var todos = CommonDataService.ListOfTodos(out rowCount, page, PAGE_SIZE, searchValue ?? "", timeSearch ?? "", userId, settingView.ViewPage);

                var listNotFinish = todos.Where(x => x.IsFinished == false).ToList();
                var listFinish = todos.Where(x => x.IsFinished == true).ToList();

                var list = settingView.TypeView switch
                {
                    1 => listFinish,
                    2 => listNotFinish,
                    _ => todos
                };

                TodoTaskSearchResult Datatodos = new TodoTaskSearchResult()
                {
                    Page = page,
                    PageSize = PAGE_SIZE,
                    SearchValue = searchValue ?? "",
                    TimeSearch = timeSearch ?? "",
                    RowCount = rowCount,
                    UserID = userId,
                    PageSetting = settingView,
                    DataTodos = list
                };
                if ((!string.IsNullOrEmpty(timeSearch) || !string.IsNullOrEmpty(searchValue)) && rowCount < 1)
                {
                    ViewBag.Title = "404 Không tìm thấy";
                    ViewBag.NotHaveSearch = 0;
                }
                return View(Datatodos);
            }
            return View(null);
        }
        public IActionResult UpdatePageSize(int pageSize)
        {
            HttpContext.Session.SetInt32("PageSize", pageSize);
            return Json(new { success = true });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
