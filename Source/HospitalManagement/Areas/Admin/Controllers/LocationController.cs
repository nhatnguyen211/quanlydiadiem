using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class LocationController : BaseController
    {
        // GET: Admin/Location
        private readonly string KeyElement = "r";

        public ActionResult Index()
        {
            ViewBag.Feature = "Bảng điều khiển";
            ViewBag.Element = KeyElement;
            ViewBag.BaseURL = "/Admin/Location/All";
            return View();
        }

        [HttpPost]
        public JsonResult GetInfo(Guid id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var Location = workScope.Locations.FirstOrDefault(x => !x.IsDeleted && x.Id == id);

                return
                    Json(new
                    {
                        status = true,
                        mess = "Lấy thành công " + KeyElement,
                        data = new
                        {
                            Location.PatientCode,
                            Location.FullName,
                            DateOfBirth = Location.DateOfBirth.ToString("dd/MM/yyyy"),
                            Location.Address,
                            Age = DateTime.Now.Year - Location.DateOfBirth.Year,
                        }
                    });
            }
        }

        public ActionResult All(string patientCode, string indentificationCardId, string fullName)
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;
            ViewBag.BaseURL = "/Admin/Location";

            if (patientCode == "")
            {
                patientCode = null;
            }
            if (indentificationCardId == "")
            {
                indentificationCardId = null;
            }
            if (fullName == "")
            {
                fullName = null;
            }

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var listData = workScope.Locations.Query(x => x.Status).OrderByDescending(x => x.JoinDate).ToList();

                //var data = listData.Where(x => x.JoinDate > new DateTime(2020, 5, 1) && x.JoinDate < new DateTime(2020, 6, 1));
                //var r = new Random();
                //foreach (var p in data)
                //{
                //    int rInt = r.Next(-10, 0);
                //    if (rInt % 2 != 0)
                //        p.JoinDate = DateTime.Now.AddMonths(rInt);
                //}
                //workScope.Complete();

                var q = from mt in listData
                        where (!string.IsNullOrEmpty(patientCode) && mt.PatientCode.ToLower().Contains(patientCode.ToLower()))
                              || (!string.IsNullOrEmpty(indentificationCardId) && mt.IndentificationCardId.ToLower().Contains(indentificationCardId.ToLower()))
                              || (!string.IsNullOrEmpty(fullName) && mt.FullName.ToLower().Contains(fullName.ToLower()))
                        select mt;

                var user = GetCurrentUser();

                if (user.Role == RoleKey.Director)
                {
                    var locationOfDirectors =
                        workScope.LocationDirectors.Query(x => x.DirectorId == user.DirectorId) ?? new List<LocationDirector>();

                    var locationOfDirectorId = locationOfDirectors.Select(x => x.LocationId);
                    listData = listData.Where(x => locationOfDirectorId.Contains(x.Id)).ToList();
                    q = q.Where(x => locationOfDirectorId.Contains(x.Id)).ToList();
                }

                if (patientCode == null && indentificationCardId == null && fullName == null)
                {
                    return View(listData);
                }
                return View(q.OrderByDescending(x => x.JoinDate).ToList());
            }
        }

        public ActionResult Create()
        {
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = KeyElement;

            if (Request.Url != null)
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1));

            string code;
            // Create Location code
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var Location = workScope.Locations.GetAll().OrderByDescending(x => x.JoinDate).FirstOrDefault();
                if (Location != null)
                {
                    var codeSplit = Location.PatientCode.Split('-');
                    code = Common.Prefix + (int.Parse(codeSplit[1]) + 1);
                }
                else
                {
                    code = Common.Prefix + "1";
                }
            }

            ViewBag.Code = code;
            ViewBag.isEdit = false;
            return View();
        }

        public ActionResult Update(Guid id)
        {
            ViewBag.isEdit = true;
            ViewBag.Feature = "Cập nhật";
            ViewBag.Element = KeyElement;
            if (Request.Url != null)
            {
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1));
            }

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var Location = workScope.Locations
                    .FirstOrDefault(x => x.Id == id);

                if (Location != null)
                {
                    return View("Create", Location);
                }
                else
                {
                    return RedirectToAction("Create", "Location");
                }
            }

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(Location input, bool isEdit)
        {
            try
            {
                var user = GetCurrentUser();

                if (isEdit) //update
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.Locations.Get(input.Id);
                        if (elm != null) //update
                        {
                            input.Status = true;
                            input.PatientCode = elm.PatientCode;

                            // input.RecordId = elm.RecordId;
                            elm = input;

                            workScope.Locations.Put(elm, elm.Id);
                            workScope.Complete();
                            return Json(new
                            {
                                status = true,
                                mess = "Cập nhập thành công ",
                                data = new
                                {
                                    input.Id
                                }
                            });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                        }
                    }
                }
                else
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        string code;

                        var Location = workScope.Locations.GetAll().OrderByDescending(x => x.JoinDate).FirstOrDefault();
                        if (Location != null)
                        {
                            var codeSplit = Location.PatientCode.Split('-');
                            code = Common.Prefix + (int.Parse(codeSplit[1]) + 1);
                        }
                        else
                        {
                            code = Common.Prefix + "1";
                        }

                        input.Id = Guid.NewGuid();
                        input.PatientCode = code;
                        input.JoinDate = DateTime.Now;
                        input.Status = true;

                        workScope.Locations.Add(input);
                        workScope.Complete();

                        if (user.Role == RoleKey.Director)
                        {
                            workScope.LocationDirectors.Add(new LocationDirector
                            {
                                LocationId = input.Id,
                                DirectorId = user.DirectorId.GetValueOrDefault(),
                                Status = 1
                            });
                            workScope.Complete();
                        }
                    }

                    return Json(new
                    {
                        status = true,
                        mess = "Thêm thành công " + KeyElement,
                        data = new
                        {
                            input.Id
                        }
                    });
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
                    var user = GetCurrentUser();

                    var elm = workScope.Locations.FirstOrDefault(x => !x.IsDeleted && x.Id == id);

                    if (user.Role == RoleKey.Director)
                    {
                        var locationOfDirectors =
                            workScope.LocationDirectors.Query(x => x.DirectorId == user.DirectorId && x.LocationId == id);
                        if (locationOfDirectors == null)
                        {
                            return Json(new { status = false, mess = "Không có quyền " + KeyElement });
                        }
                    }

                    if (elm != null)
                    {
                        //del Location
                        elm.Status = false;

                        workScope.Complete();
                        return Json(new { status = true, mess = "Xóa thành công " + KeyElement });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
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