using _21T1020851_Todo.BusinessLayers;
using _21T1020851_Todo.DomainModels;
using Microsoft.AspNetCore.Mvc;

namespace _21T1020851_Todo.Web.Controllers
{
    public class SettingViewController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Cài đặt trang";
            int? nullableInt = HttpContext.Session.GetInt32("UserID");
            if (nullableInt.HasValue)
            {
                int userId = nullableInt.Value;
                var settingView = CommonDataService.GetSetting(userId);
                return View(settingView);
            }
            else 
                return RedirectToAction("Index", "Home");
        }
        public IActionResult Save(SettingView settingView)
        {
            if (settingView == null)
            {
                ModelState.AddModelError("Error", "Lỗi dữ liệu !");
                return View();
            }
            else
            {
                var result = CommonDataService.UpdateSetting(settingView);
                if (result)
                {                    
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Error", "Lỗi Cập Nhật Setting !");
                    return View();
                }
            }
        }
    }
}
