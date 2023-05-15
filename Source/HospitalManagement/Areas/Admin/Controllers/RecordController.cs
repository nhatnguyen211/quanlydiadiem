using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class RecordController : BaseController
    {
        private readonly string KeyElement = "Thông Tin";

        // GET: Admin/Record
        public ActionResult Index(Guid id)
        {
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = KeyElement;

            var user = GetCurrentUser();

            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var locationRecord = workScope.LocationRecords.FirstOrDefault(x => x.RecordId == id && !x.IsDelete);

                    if (user.Role == RoleKey.Director && locationRecord.DirectorId != user.DirectorId)
                    {
                        return RedirectToAction("E401", "Login");
                    }

                    if (locationRecord == null)
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }

                    var record = workScope.Records.FirstOrDefault(x => x.Id == locationRecord.RecordId);
                    var location = workScope.Locations.FirstOrDefault(x => x.Id == locationRecord.LocationId);

                    if (record == null)
                    {
                        record = new Record
                        {
                            Id = Guid.NewGuid(),
                            CreatedBy = GetCurrentUser().FullName,
                            ModifiedBy = GetCurrentUser().FullName,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            Note = "",
                            Result = "",
                            DirectorId = user.Role == RoleKey.Director ? user.DirectorId : null
                        };
                        workScope.Records.Add(record);
                        workScope.Complete();
                    }
                    ViewBag.Record = record;

                    var Directors = workScope.Directors.GetAll().ToList();
                    ViewBag.Directors = new SelectList(Directors, "Id", "Name");

                    var faculties = workScope.Faculties.GetAll().ToList();
                    ViewBag.Faculties = new SelectList(faculties, "Id", "Name");

                    var lstStatus = StatusRecord.GetDic();
                    ViewBag.ListStatus = new SelectList(lstStatus, "Value", "Text");

                    var detailRecords = workScope.DetailRecords.Query(x => x.RecordId == record.Id && !x.IsMainRecord).OrderByDescending(x => x.Process).ToList();

                    var mainDetailRecords =
                        workScope.DetailRecords.FirstOrDefault(x => x.RecordId == record.Id && x.IsMainRecord);

                    ViewBag.MainDetailRecords = mainDetailRecords;
                    ViewBag.location = location;
                    ViewBag.DetailRecords = detailRecords;

                    ViewBag.BaseURL = "/admin/locationRecord?LocationId=" + location.Id;
                    return View(locationRecord);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        public JsonResult GetJson(Guid id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var detailRecord = workScope.DetailRecords.FirstOrDefault(x => x.Id == id);

                var user = GetCurrentUser();

                if (user.Role == RoleKey.Director && detailRecord.DirectorId != user.DirectorId)
                {
                    return Json(new
                    {
                        status = false,
                        mess = "K có quyền"
                    });
                }
                return detailRecord == null ?
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
                            detailRecord.Id,
                            detailRecord.DiseaseName,
                            detailRecord.FacultyId,
                            detailRecord.DirectorId,
                            detailRecord.Note,
                            detailRecord.Status,
                            detailRecord.Result,
                            detailRecord.Process
                        }
                    });
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(DetailRecord input, Guid detailDoctorId, bool isEdit)
        {
            try
            {
                var user = GetCurrentUser();
                if (isEdit) //update
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.DetailRecords.Get(input.Id);

                        if (elm != null) //update
                        {
                            //Che bien du lieu

                            elm = input;
                            elm.DirectorId = user.Role == RoleKey.Director ? user.DirectorId : detailDoctorId;

                            workScope.DetailRecords.Put(elm, elm.Id);
                            workScope.Complete();

                            //attachments handle

                            return Json(new
                            {
                                status = true,
                                mess = "Cập nhập thành công ",
                                data = new
                                {
                                    detailRecordId = input.Id
                                }
                            });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                        }
                    }
                }
                else //Thêm mới
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        //Che bien du lieu
                        input.Id = Guid.NewGuid();
                        input.DirectorId = user.Role == RoleKey.Director ? user.DirectorId : detailDoctorId;

                        workScope.DetailRecords.Add(input);
                        workScope.Complete();
                    }
                    return Json(new
                    {
                        status = true,
                        mess = "Thêm thành công " + KeyElement,
                        data = new
                        {
                            detailRecordId = input.Id
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

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateRecord(Record input)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.Records.FirstOrDefault(x => x.Id == input.Id && !x.IsDelete);

                    if (elm != null) //update
                    {
                        //Che bien du lieu
                        input.CreatedBy = elm.CreatedBy;
                        input.CreatedDate = elm.CreatedDate;
                        input.ModifiedBy = GetCurrentUser().FullName;
                        input.ModifiedDate = DateTime.Now;

                        elm = input;

                        workScope.Records.Put(elm, elm.Id);
                        workScope.Complete();

                        return Json(new { status = true, mess = "Cập nhập thành công " });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                    }
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
                    var elm = workScope.Records.FirstOrDefault(x => x.Id == id && !x.IsDelete);

                    var user = GetCurrentUser();

                    if (user.Role == RoleKey.Director && elm.DirectorId != user.DirectorId)
                    {
                        return Json(new
                        {
                            status = false,
                            mess = "K có quyền"
                        });
                    }

                    if (elm != null)
                    {
                        elm.IsDelete = true;
                        //del
                        workScope.Records.Put(elm, elm.Id);
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