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
    public class LocationRecordController : BaseController
    {
        // GET: Admin/Location
        private readonly string KeyElement = "Danh sách";

        public ActionResult Index(Guid? LocationId)
        {
            ViewBag.Feature = "Record";
            ViewBag.Element = KeyElement;
            ViewBag.BaseURL = "/Admin/LocationRecord";

            var user = GetCurrentUser();

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                List<LocationRecord> listData;
                var Locations = workScope.Locations.GetAll();

                if (user.Role == RoleKey.Director)
                {
                    var locationOfDirectors =
                        workScope.LocationDirectors.Query(x => x.DirectorId == user.DirectorId) ?? new List<LocationDirector>();

                    var locationOfDirectorIds = locationOfDirectors.Select(x => x.LocationId);

                    Locations = Locations.Where(x => locationOfDirectorIds.Contains(x.Id)).ToList();
                }

                if (LocationId.HasValue)
                {
                    var Location = workScope.Locations.FirstOrDefault(x => x.Id == LocationId);
                    ViewBag.Location = Location;
                    listData =
                      workScope.LocationRecords.Query(x => x.LocationId == LocationId && !x.IsDelete).OrderByDescending(x => x.TestDate)
                          .ToList();

                    ViewBag.Locations = new SelectList(Locations.Select(x => new
                    {
                        id = x.Id,
                        FullName = x.PatientCode + " - " + x.FullName
                    }), "Id", "FullName", LocationId);

                    if (user.Role == RoleKey.Director)
                    {
                        listData = listData.Where(x => x.DirectorId == user.DirectorId).ToList();
                    }
                    return View(listData);
                }
                ViewBag.Locations = new SelectList(Locations.Select(x => new
                {
                    id = x.Id,
                    FullName = x.PatientCode + " - " + x.FullName
                }), "Id", "FullName");

                listData = workScope.LocationRecords.Query(x => !x.IsDelete).OrderByDescending(x => x.TestDate).ToList();

                if (user.Role == RoleKey.Director)
                {
                    listData = listData.Where(x => x.DirectorId == user.DirectorId).ToList();
                }

                return View(listData);
            }
        }

        public ActionResult Create(Guid? LocationId)
        {
            var user = GetCurrentUser();
            ViewBag.Feature = "Thêm mới";
            ViewBag.Element = KeyElement;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                if (Request.Url != null)
                    ViewBag.BaseURL = LocationId.HasValue ? "/admin/locationRecord?LocationId=" + LocationId : "/admin/locationRecord";

                var Locations = workScope.Locations.Query(x => !x.IsDeleted).ToList();

                if (user.Role == RoleKey.Director)
                {
                    var locationOfDirectors =
                        workScope.LocationDirectors.Query(x => x.DirectorId == user.DirectorId) ?? new List<LocationDirector>();

                    var locationOfDirectorIds = locationOfDirectors.Select(x => x.LocationId);

                    Locations = Locations.Where(x => locationOfDirectorIds.Contains(x.Id)).ToList();
                }

                var patientsSelect = Locations.Select(x => new
                {
                    id = x.Id,
                    FullName = x.PatientCode + " - " + x.FullName
                });
                ViewBag.Locations = new SelectList(patientsSelect, "Id", "FullName", LocationId);

                var Directors = workScope.Directors.GetAll().Select(x => new
                {
                    id = x.Id,
                    FullName = x.Name
                });

                ViewBag.Directors = user.Role == RoleKey.Director ?
                      new SelectList(Directors, "Id", "FullName", user.DirectorId)
                    : new SelectList(Directors, "Id", "FullName");

                ViewBag.isEdit = false;

                var locationRecord = new LocationRecord
                {
                    test = 0,
                    test2 = 0,
                    test5 = 0,
                    Formation = "Không có",
                    Activity2 = "Không có",
                    Activity3 = "Không có",
                    Activity = "Không có",
                    EyePressureLeft = "0",
                    EyePressureRight = "0",
                    test3 = 0,
                    test4 = 0,
                    ForManage2 = "Không có",
                    ForManage = "Không có",
                    ForDate2 = "Không có",
                    ForDate = "Không có",
                    Event2 = "Không có",
                    Event = "Không có",
                    TestDate = DateTime.UtcNow.AddHours(7)
                };

                return View(locationRecord);
            }
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
                var Directors = workScope.Directors.GetAll().Select(x => new
                {
                    id = x.Id,
                    FullName = x.Name
                });

                ViewBag.Directors = new SelectList(Directors, "Id", "FullName");

                var locationRecord = workScope.LocationRecords
                    .FirstOrDefault(x => x.Id == id);

                if (locationRecord == null)
                    return RedirectToAction("Create", "locationRecord");

                var Locations = workScope.Locations.GetAll().Select(x => new
                {
                    id = x.Id,
                    FullName = " (*) " + x.FullName
                });
                ViewBag.Locations = new SelectList(Locations, "Id", "FullName", locationRecord.LocationId);

                return View("Create", locationRecord);
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(LocationRecord input, TimeSpan testTime, bool isEdit)
        {
            try
            {
                var user = GetCurrentUser();

                if (isEdit) //update
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.LocationRecords.FirstOrDefault(x => x.Id == input.Id && !x.IsDelete);
                        if (elm != null) //update
                        {
                            input.RecordId = elm.RecordId;
                            input.TestDate = input.TestDate.Date + testTime;
                            input.LocationId = elm.LocationId;
                            input.DirectorId = user.Role == RoleKey.Director ? user.DirectorId : input.DirectorId;
                            elm = input;

                            workScope.LocationRecords.Put(elm, elm.Id);
                            workScope.Complete();
                            return Json(new
                            {
                                status = true,
                                mess = "Cập nhập thành công ",
                                data = new
                                {
                                    input.RecordId
                                }
                            });
                        }

                        return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                    }
                }

                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var recordId = Guid.NewGuid();

                    var record = new Record
                    {
                        Id = recordId,
                        CreatedDate = DateTime.UtcNow.AddHours(7),
                        ModifiedDate = DateTime.UtcNow.AddHours(7),
                        CreatedBy = GetCurrentUser().FullName,
                        ModifiedBy = GetCurrentUser().FullName,
                        DirectorId = user.Role == RoleKey.Director ? user.DirectorId : null
                    };
                    workScope.Records.Add(record);
                    workScope.Complete();

                    var mainRecordDetail = new DetailRecord
                    {
                        Id = Guid.NewGuid(),
                        RecordId = recordId,
                        DiseaseName = input.Title,
                        IsMainRecord = true,
                        Status = true,
                        Process = 0,
                        DirectorId = user.Role == RoleKey.Director ? user.DirectorId : null
                    };
                    workScope.DetailRecords.Add(mainRecordDetail);
                    workScope.Complete();

                    input.Id = Guid.NewGuid();
                    input.RecordId = recordId;
                    input.DirectorId = user.Role == RoleKey.Director ? user.DirectorId : input.DirectorId;
                    input.TestDate = input.TestDate.Date + testTime;
                    workScope.LocationRecords.Add(input);
                    workScope.Complete();
                }

                return Json(new
                {
                    status = true,
                    mess = "Thêm thành công " + KeyElement,
                    data = new
                    {
                        input.RecordId
                    }
                });
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
                    var elm = workScope.LocationRecords.FirstOrDefault(x => x.Id == id && !x.IsDelete);
                    if (elm != null)
                    {
                        //del Location
                        var record = workScope.Records.FirstOrDefault(x => x.Id == elm.RecordId);

                        var detailRecords = record.DetailRecords.Where(x => x.RecordId == record.Id);

                        foreach (var detailRecord in detailRecords)
                        {
                            if (workScope.ItemSites.Query(x => x.DetailRecordId == detailRecord.Id).Any())
                            {
                                return Json(new { status = false, mess = "Không thể xóa" + KeyElement });
                            }
                        }

                        elm.IsDelete = true;
                        workScope.LocationRecords.Put(elm, elm.Id);

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