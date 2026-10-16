using _21T1020851_Todo.BusinessLayers;
using _21T1020851_Todo.DomainModels;
using Microsoft.AspNetCore.Mvc;

namespace _21T1020851_Todo.Web.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            var User = CommonDataService.GetAccount(HttpContext.Session.GetInt32("UserID") ?? 0);
            if (User == null) return RedirectToAction("Index", "Home");
            return View(User);
        }
        private bool _IsNullString(string value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return false;
        }
        public IActionResult Authenticate()
        {
            if (IsLogin()) 
                return RedirectToAction("Index", "Home");

            @ViewBag.Title = "Xác thực";
            return View();
        }
        private bool IsLogin()
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                return true;
            }
            return false;
        }
        private void SetSessionUser(int uniqueID, string name, string userName)
        {
            //Set info user into session
            HttpContext.Session.SetInt32("UserID", uniqueID);
            HttpContext.Session.SetString("FullName", name);
            HttpContext.Session.SetString("Username", userName);
            //end set
        }
        private void ClearSessionUser()
        {
            HttpContext.Session.Clear();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (_IsNullString(username) || _IsNullString(password))
            {
                ModelState.AddModelError("error", "Vui lòng điền đầy đủ thông tin!");
                return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
            }
            var user = CommonDataService.LoginAccount(username, password);
            if (user == null)
            {
                return Json(new { success = false, message = "Tài khoản hoặc mật khẩu không đúng." });
            }
            else if(ModelState.IsValid)
            {
                SetSessionUser(user.UserID, user.FullName, user.Username);
                return Json(new { success = true, message = "Đăng nhập thành công!" });
                //return RedirectToAction("Index", "Home");
            }
            else return Json(new { success = false, message = "Có lỗi bất thường xảy ra, vui lòng thử lại sau!!" });

            //return View();
        }
        [HttpPost]
        public IActionResult Register(Account user)
        {
            if (_IsNullString(user.Username) || _IsNullString(user.Password) || _IsNullString(user.FullName) || _IsNullString(user.Email))
            {
                ModelState.AddModelError("error", "Vui lòng điền đầy đủ thông tin!");
                return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
            }
            var checkUser = CommonDataService.GetUserWithName(user.Username);
            if (checkUser != null && (checkUser.UserID != 0 || user.Username == checkUser.Username))
            {
                return Json(new { success = false, message = "Tài khoản người dùng đã tồn tại." });
            }
            else if (ModelState.IsValid)
            {
                var setting = new SettingView()
                {
                    UserID = CommonDataService.CreateAccount(user)
                };
                if (CommonDataService.CreateSettingView(setting) > 0)
                {
                    SetSessionUser(setting.UserID, user.FullName, user.Username);
                    return Json(new { success = true, message = "Đăng ký thành công!" });
                }
                else return Json(new { success = false, message = "Có lỗi bất thường xảy ra, vui lòng thử lại sau!!" });
            }
            else return Json(new { success = false, message = "Có lỗi bất thường xảy ra, vui lòng thử lại sau!!" });

            //return View();
        }
        public IActionResult Logout()
        {
            ClearSessionUser();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Edit(Account user)
        {
            if (!IsLogin()) return RedirectToAction("Index", "Home");
            if (user.UserID == 0 || user is null)
            {
                return Json(new { success = false, message = "Có lỗi bất thường, vui lòng thử đăng xuất và đăng nhập lại sau đó thử lại sau!" });
            }
            else if (_IsNullString(user.Username) || _IsNullString(user.FullName) || _IsNullString(user.Email))
            {
                ModelState.AddModelError("error", "Vui lòng điền đầy đủ thông tin!");
                return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
            }
            else
            {
                if (CommonDataService.GetAccount(HttpContext.Session.GetInt32("UserID") ?? 0) is not null)
                {
                    CommonDataService.UpdateAccountInfo(user);
                    HttpContext.Session.SetString("FullName", user.FullName);
                    return Json(new { success = true, message = "Cập nhật thông tin người dùng thành công!!" });
                }
                return Json(new { success = true, message = "Lỗi ! Vui lòng đăng nhập lại !!" });
            }
            //return View();
        }
        public IActionResult CancelAccount(int id = 0)
        {
            if (Request.Method == "POST")
            {
                if (!IsLogin()) return RedirectToAction("Index", "Home");
                if (id == 0)
                    return Json(new { success = false, message = "Có lỗi bất thường, vui lòng thử đăng xuất và đăng nhập lại sau đó thử lại sau!" });

                if (CommonDataService.DeleteAccount(id))
                {
                    ClearSessionUser();
                    return RedirectToAction("Index", "Home");
                }
                //return Json(new { success = true, message = "Cút con mẹ m đi. Ăn hại" });
            }
            return View();
        }
        public IActionResult ChangeUserPwd(string oldPass, string newPass)
        {
            if (_IsNullString(oldPass) || _IsNullString(newPass))
            {
                ModelState.AddModelError("error", "Vui lòng không để trống dữ liệu!");
                return Json(new { success = false, message = "Vui lòng không để trống dữ liệu!" });
            }
            else
            {
                Account? User = CommonDataService.GetAccount(HttpContext.Session.GetInt32("UserID") ?? 0);
                if (User?.Password != oldPass)
                {
                    return Json(new { success = false, message = "Mật khẩu cũ không không đúng." });
                }
                else if (newPass == oldPass)
                {
                    return Json(new { success = false, message = "Vui lòng không đặt trùng mật khẩu cũ!" });
                }
                else
                {
                    CommonDataService.UpdateAccountPassword(User.UserID.ToString(), newPass);
                    return Json(new { success = true, message = "Cập nhật mật khẩu mới thành công ! Vui lòng đăng nhập lại !!" });
                }
            }
            //return View();
        }
        public IActionResult ChangePwd()
        {
            ViewBag.Title = "Đổi Mật Khẩu";
            if (!IsLogin()) return RedirectToAction("Index", "Home");
            return View();
        }
    }
}
