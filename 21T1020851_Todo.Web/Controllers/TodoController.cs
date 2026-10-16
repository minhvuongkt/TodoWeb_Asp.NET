using _21T1020851_Todo.BusinessLayers;
using _21T1020851_Todo.DomainModels;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace _21T1020851_Todo.Web.Controllers
{
    public class TodoController : Controller
    {
        private bool IsLogin()
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                return true;
            }
            return false;
        }
        private bool _IsNullString(string value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return false;
        }
        public IActionResult Create(TodoTask todo)
        {
            ViewBag.Title = "Tạo công việc mới";
            if (!IsLogin()) return RedirectToAction("Index", "Home");
            if (Request.Method == "POST")
            {
                if (_IsNullString(todo.TaskTitle)) ModelState.AddModelError(nameof(todo.TaskTitle), "Vui lòng không để trống tên công việc!");
                if (_IsNullString(todo.TaskDescription)) ModelState.AddModelError(nameof(todo.TaskDescription), "Vui lòng không để trống mô tả công việc!");
                if (todo.TimeStart.ToShortDateString().Equals(todo.TimeEnd.ToShortDateString()) || todo.TimeStart > todo.TimeEnd)
                    ModelState.AddModelError("timeerror", "Lỗi khoảng thời gian không hợp lệ!");
                if (!ModelState.IsValid)
                    return View();
                //return Json(new { success = true, message = "Dữ liệu chưa hợp lệ!!" });
                else
                {
                    CommonDataService.CreateTodo(todo);
                    return RedirectToAction("Index", "Home");
                    //return Json(new { success = true, message = "Tạo công việc mới thành công!" });
                }
            }
            return View();

        }
        public IActionResult Edit(int id = 0)
        {
            if (!IsLogin()) return RedirectToAction("Index", "Home");
            var todoEdit = CommonDataService.GetTodo(id);
            if (todoEdit is null || id == 0)
            {
                ModelState.AddModelError("item", "Lỗi lấy dữ liệu !");
            }
            if (ModelState.IsValid)
                return View(todoEdit);
            return Json(new { success = false, message = "Lỗi lấy dữ liệu !" });
        }
        public IActionResult Delete(int id = 0)
        {
            if (!IsLogin()) return RedirectToAction("Index", "Home");
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteTodo(id);
                return RedirectToAction("Index", "Home");
            }
            var todoDel = CommonDataService.GetTodo(id);
            if (todoDel is null || id == 0)
            {
                ModelState.AddModelError("item", "Lỗi lấy dữ liệu !");
            }
            if (ModelState.IsValid)
            {
                return View(todoDel);
            }
            else return Json(new { success = false, message = "Lỗi lấy dữ liệu !" });
        }
        [HttpPost]
        public IActionResult UpdateStatus(string taskId, string isCompleted)
        {
            if (!IsLogin())
                return RedirectToAction("Index", "Home");
            if (_IsNullString(taskId) || _IsNullString(isCompleted))
            {
                ModelState.AddModelError("error", "Lỗi lấy dữ liệu !");
                return Json(new { success = false, message = "Lỗi lấy dữ liệu !" });
            }
            if (ModelState.IsValid)
            {
                CommonDataService.UpdateFinishTodo(taskId, isCompleted);
                return Json(new { success = true, newStatus = isCompleted });
            }
            return Json(new { success = false, message = "Lỗi lấy dữ liệu !" });
        }
        [HttpPost]
        public IActionResult Update(TodoTask todo)
        {
            if (!IsLogin())
                return RedirectToAction("Index", "Home");
            if (todo is null || todo.TaskID == 0)
            {
                ModelState.AddModelError("error", "Dữ liệu rỗng!");
                return Json(new { success = false, message = "Lỗi dữ liệu bị rỗng! Vui lòng thử đăng xuất và đăng nhập lại!" });
            }
            else if (todo.TimeStart.ToShortDateString().Equals(todo.TimeEnd.ToShortDateString()) || todo.TimeStart > todo.TimeEnd)
            {
                ModelState.AddModelError("timeerror", "");
                return Json(new { success = false, message = "Lỗi khoảng thời gian không hợp lệ!" });
            }
            if (ModelState.IsValid)
            {
                if (CommonDataService.UpdateTodo(todo))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return Json(new { success = false, message = "Lỗi cập nhật công việc !" });
        }
    }
}
