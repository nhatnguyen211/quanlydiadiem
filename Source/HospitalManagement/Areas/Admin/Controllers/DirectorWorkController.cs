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
    public class DirectorWorkController : BaseController
    {
        // GET: Admin/DoctorSchedule
        private const string KeyElement = "Lịch";

        // GET: Admin/Event
        public ActionResult Index()
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;

            var user = GetCurrentUser();

            if (Request.Url != null) ViewBag.BaseURL = Request.Url.LocalPath;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                ViewBag.Status = new SelectList(new List<object>
                {
                    new
                    {
                        Id = BookingStatusKey.Active,
                        Name = BookingStatusKey.GetText(BookingStatusKey.Active),
                    },
                    new
                    {
                        Id = BookingStatusKey.Pending,
                        Name = BookingStatusKey.GetText(BookingStatusKey.Pending),
                    },
                    new
                    {
                        Id = BookingStatusKey.Reject,
                        Name = BookingStatusKey.GetText(BookingStatusKey.Reject),
                    },
                }, "Id", "Name");

                var listLocation = workScope.Locations.Query(x => !x.IsDeleted);

                var DirectorWorks = workScope.DirectorWorks.GetAll();

                var Directors = workScope.Directors.GetAll();

                if (user.Role == RoleKey.Director)
                {
                    DirectorWorks = DirectorWorks.Where(x => x.DirectorId == user.DirectorId);
                    Directors = Directors.Where(x => x.Id == user.DirectorId);
                }

                var listData = (from ds in DirectorWorks
                                join p in listLocation on ds.LocationId equals p.Id
                                join d in Directors on ds.DirectorId equals d.Id
                                select ds).ToList();

                return View(listData);
            }
        }

        [HttpPost]
        public JsonResult GetJson(int id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var Location = workScope.DirectorWorks.FirstOrDefault(x => x.Id == id);

                return Location == default ?
                    Json(new
                    {
                        status = false,
                        mess = "Có lỗi xảy ra: "
                    }) :
                    Json(new
                    {
                        status = true,
                        mess = "Lấy thành công " + KeyElement,
                        data = new
                        {
                            Location.Id,
                            Location.Status
                        }
                    });
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(DirectorWork input, bool isEdit)
        {
            try
            {
                if (isEdit)
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.DirectorWorks.Get(input.Id);

                        if (elm != null) //update
                        {
                            input.DirectorId = elm.DirectorId;
                            input.LocationId = elm.LocationId;
                            input.ScheduleBook = elm.ScheduleBook;
                            elm = input;

                            workScope.DirectorWorks.Put(elm, elm.Id);
                            workScope.Complete();

                            return Json(new { status = true, mess = "Cập nhập thành công " });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                        }
                    }
                }
                else
                {
                    return Json(new { status = true, mess = "Method not allow" + KeyElement });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Del(int id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.DirectorWorks.Get(id);
                    if (elm != null)
                    {
                        //del
                        workScope.DirectorWorks.Remove(elm);
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