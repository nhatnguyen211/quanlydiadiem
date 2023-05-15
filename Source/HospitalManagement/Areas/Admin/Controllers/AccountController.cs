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
    [Permission(Role = RoleKey.Admin)]
    public class AccountController : BaseController
    {
        // GET: Admin/Course
        private string _keyElement = "Tài khoản";

        public ActionResult Index(int? role)
        {
            role = role ?? RoleKey.Admin;

            ViewBag.Feature = "Danh sách";
            ViewBag.Element = _keyElement;

            if (Request.Url != null) ViewBag.BaseURL = Request.Url.LocalPath;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var lstRole = RoleKey.GetDic();
                ViewBag.Roles = new SelectList(lstRole, "Value", "Text");
                ViewBag.Role = role;

                switch (role)
                {
                    case RoleKey.Admin:
                        {
                            _keyElement += " - Quản trị";

                            var listData = workScope.Accounts.Query(x => x.Role == RoleKey.Admin && !x.IsDeleted).ToList();
                            return View(listData);
                        }
                    case RoleKey.Director:
                        {
                            _keyElement += " - Bác Sĩ";
                            var accountDirector = workScope.Accounts.Query(x => x.Role == RoleKey.Director && !x.IsDeleted).ToList();
                            return View(accountDirector);
                        }
                    case RoleKey.Location:
                        {
                            var accountLocation = workScope.Accounts.Query(x => x.Role == RoleKey.Location && !x.IsDeleted).ToList();
                            return View(accountLocation);
                        }
                    default:
                        {
                            var listData = workScope.Accounts.Query(x => !x.IsDeleted).ToList();
                            return View(listData);
                        }
                }
            }
        }

        public ActionResult Create(int role)
        {
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = RoleKey.GetRole(role);

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1)) + "?role=" + role;

            ViewBag.isEdit = false;
            ViewBag.Role = role;

            ViewBag.Genders = new SelectList(GenderKey.GetDic(), "Value", "Text");
            ViewBag.Roles = new SelectList(RoleKey.GetDic(), "Value", "Text");

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var listDoctor = workScope.Directors.GetAll().ToList();
                var accountDirector = workScope.Accounts.Query(x => x.Role == RoleKey.Director && !x.IsDeleted).ToList();

                foreach (var Director in listDoctor.Where(Director => accountDirector.Any(x => x.DirectorId == Director.Id)))
                {
                    listDoctor = listDoctor.Where(d => d.Id != Director.Id).ToList();
                }

                var Directors = listDoctor.Select(x => new
                {
                    id = x.Id,
                    FullName = x.Name
                });

                ViewBag.Directors = new SelectList(Directors, "Id", "FullName");

                //

                var listPatient = workScope.Locations.GetAll().ToList();
                var accountLocation = workScope.Accounts.Query(x => x.Role == RoleKey.Location && !x.IsDeleted).ToList();

                foreach (var Location in listPatient.Where(Location => accountLocation.Any(x => x.LocationId == Location.Id)))
                {
                    listPatient = listPatient.Where(d => d.Id != Location.Id).ToList();
                }

                var Locations = listPatient.Select(x => new
                {
                    id = x.Id,
                    FullName = x.PatientCode + " - " + x.FullName
                });

                ViewBag.Locations = new SelectList(Locations, "Id", "FullName");

                return View();
            }
        }

        public ActionResult Update(Guid id, int role)
        {
            ViewBag.isEdit = true;
            ViewBag.Role = role;
            ViewBag.Feature = "Cập nhật";

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1)) + "?role=" + role;

            ViewBag.Element = RoleKey.GetRole(role);
            ViewBag.Genders = new SelectList(GenderKey.GetDic(), "Value", "Text");
            ViewBag.Roles = new SelectList(RoleKey.GetDic(), "Value", "Text");

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                if (role == RoleKey.Director)
                {
                    var acc = workScope.Accounts.Include(x => x.Director).FirstOrDefault(x => x.Id == id);
                    if (acc != null)
                    {
                        acc.Password = "";
                        return View("Create", acc);
                    }
                }
                else if (role == RoleKey.Location)
                {
                    var acc = workScope.Accounts.Include(x => x.Location).FirstOrDefault(x => x.Id == id);
                    if (acc != null)
                    {
                        acc.Password = "";
                        return View("Create", acc);
                    }
                }
                else
                {
                    var acc = workScope.Accounts.FirstOrDefault(x => x.Id == id);
                    acc.Password = "";
                    return View("Create", acc);
                }
            }
            return View("Create");
        }

        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var account = workScope.Accounts.FirstOrDefault(x => x.Id == id && !x.IsDeleted);

                return account == default ?
                    Json(new
                    {
                        status = false,
                        mess = "Có lỗi xảy ra: "
                    }) :
                    Json(new
                    {
                        status = true,
                        mess = "Lấy thành công " + _keyElement,
                        data = new
                        {
                            account.Id,
                            account.FullName,
                            account.LocationId,
                            account.Phone,
                            account.UserName,
                            account.Gender,
                            account.Role,
                            account.DirectorId
                        }
                    });
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(Account input, bool isEdit, string rePassword)
        {
            try
            {
                if (isEdit) //update
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.Accounts.FirstOrDefault(x => !x.IsDeleted && x.Id == input.Id);

                        if (elm != null) //update
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

                                var passwordFactory = input.Password + (input.Role == RoleKey.Location ? VariableExtensions.KeyCryptorClient : VariableExtensions.KeyCrypto);
                                var passwordCryptor = CryptorEngine.Encrypt(passwordFactory, true);
                                input.Password = passwordCryptor;
                            }
                            else
                            {
                                input.Password = elm.Password;
                            }

                            input.UserName = elm.UserName;
                            input.Role = elm.Role;
                            input.LocationId = elm.LocationId;
                            input.DirectorId = elm.DirectorId;

                            if (input.Role != RoleKey.Admin)
                            {
                                input.Phone = elm.Phone;
                                input.Gender = elm.Gender;
                            }

                            elm = input;

                            workScope.Accounts.Put(elm, elm.Id);
                            workScope.Complete();

                            return Json(new { status = true, mess = "Cập nhập thành công " });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + _keyElement });
                        }
                    }
                }
                else //Thêm mới
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        if (string.IsNullOrEmpty(input.Password) || string.IsNullOrEmpty(rePassword))
                        {
                            return Json(new { status = false, mess = "Không được để trống mật khẩu" });
                        }

                        if (input.Password != rePassword)
                        {
                            return Json(new { status = false, mess = "Mật khẩu không khớp" });
                        }

                        var elm = workScope.Accounts.Query(x => x.UserName.ToLower() == input.UserName.ToLower() && !x.IsDeleted).Any();
                        if (elm)
                        {
                            return Json(new { status = false, mess = "Tên đăng nhập đã tồn tại" });
                        }

                        var passwordFactory = input.Password + (input.Role == RoleKey.Location ? VariableExtensions.KeyCryptorClient : VariableExtensions.KeyCrypto);
                        var passwordCrypto = CryptorEngine.Encrypt(passwordFactory, true);

                        input.Password = passwordCrypto;
                        input.Id = Guid.NewGuid();

                        if (input.Role == RoleKey.Location)
                        {
                            var Location = workScope.Locations.FirstOrDefault(x => x.Id == input.LocationId);

                            if (Location == null)
                            {
                                return Json(new { status = false, mess = "Bệnh nhân k tồn tại" });
                            }

                            input.LocationId = Location.Id;
                            input.DirectorId = null;
                            input.Phone = Location.Phone;
                            input.Gender = Location.Gender;

                            workScope.Accounts.Add(input);
                            workScope.Complete();
                        }
                        else if (input.Role == RoleKey.Director)
                        {
                            var Director = workScope.Directors.FirstOrDefault(x => x.Id == input.DirectorId);

                            if (Director == null)
                            {
                                return Json(new { status = false, mess = "Bác sĩ k tồn tại" });
                            }

                            input.LocationId = null;
                            input.DirectorId = Director.Id;
                            input.Phone = Director.Phone;
                            input.Gender = Director.Gender;

                            workScope.Accounts.Add(input);
                            workScope.Complete();
                        }
                        else if (input.Role == RoleKey.Admin)
                        {
                            input.LocationId = null;
                            input.DirectorId = null;

                            workScope.Accounts.Add(input);
                            workScope.Complete();
                        }
                    }
                    return Json(new { status = true, mess = "Thêm thành công " + _keyElement });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    mess = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.Accounts.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (elm != null)
                    {
                        elm.IsDeleted = true;
                        //del
                        workScope.Accounts.Put(elm, elm.Id);
                        workScope.Complete();
                        return Json(new { status = true, mess = "Xóa thành công " + _keyElement });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " + _keyElement });
                    }
                }
            }
            catch
            {
                return Json(new { status = false, mess = "Thất bại" });
            }
        }
    }
}