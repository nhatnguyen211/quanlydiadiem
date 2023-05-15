using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using BELibrary.Utils;
using HospitalManagement.Areas.Admin.Authorization;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    [Permission(Role = RoleKey.Director)]
    public class ProfileController : BaseController
    {
        // GET: Admin/Profile

        private string _keyElement = "Tài khoản";

        public ActionResult Index()
        {
            ViewBag.Feature = "Hồ sơ";
            ViewBag.Element = _keyElement;
            var user = GetCurrentUser();

            if (user == null)
                return Redirect("/admin");

            if (user.Role != RoleKey.Director)
                return View(user);

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                ViewBag.Director = workScope.Directors.Include(x => x.Faculty)
                    .FirstOrDefault(x => x.Id == user.DirectorId);
            }
            return View(user);
        }

        public ActionResult Edit()
        {
            ViewBag.Feature = "Cập nhật";
            ViewBag.Element = _keyElement;
            var user = GetCurrentUser();

            if (user == null)
                return Redirect("/admin");

            ViewBag.Genders = new SelectList(GenderKey.GetDic(), "Value", "Text");

            if (user.Role != RoleKey.Director)
                return View(user);

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var Director = workScope.Directors.Include(x => x.Faculty)
                    .FirstOrDefault(x => x.Id == user.DirectorId) ?? new Director();
                ViewBag.Director = Director;

                var faculties = workScope.Faculties.GetAll().ToList();
                ViewBag.Faculties = new SelectList(faculties, "Id", "Name", selectedValue: Director.FacultyId);
            }
            return View(user);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateInfo(Account input, Guid? facultyId, string rePassword)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var account = GetCurrentUser();

                    if (account != null) //update
                    {
                        //xu ly password
                        if (!string.IsNullOrEmpty(input.Password) || rePassword != "")
                        {
                            if (!CookiesManage.Logined())
                            {
                                return Json(new { status = false, mess = "Chưa đăng nhập" });
                            }
                            if (input.Password != rePassword)
                            {
                                return Json(new { status = false, mess = "Mật khẩu không khớp" });
                            }

                            var passwordFactory = input.Password + VariableExtensions.KeyCrypto;
                            var passwordCryptor = CryptorEngine.Encrypt(passwordFactory, true);
                            input.Password = passwordCryptor;
                        }
                        else
                        {
                            input.Password = account.Password;
                        }

                        input.Id = account.Id;
                        input.UserName = account.UserName;
                        input.Role = account.Role;
                        input.LocationId = account.LocationId;
                        input.DirectorId = account.DirectorId;

                        account = input;
                        workScope.Accounts.Put(account, account.Id);

                        if (account.Role == RoleKey.Director && facultyId.HasValue)
                        {
                            var Director = workScope.Directors.FirstOrDefault(x => x.Id == account.DirectorId);
                            if (Director != null && workScope.Faculties.GetAll().Any(x => x.Id == facultyId))
                            {
                                Director.FacultyId = facultyId.GetValueOrDefault();
                                workScope.Directors.Put(Director, Director.Id);
                            }
                        }

                        workScope.Complete();

                        return Json(new { status = true, mess = "Cập nhập thành công " });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " + _keyElement });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}