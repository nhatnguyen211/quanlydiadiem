using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HospitalManagement.Handler;

namespace HospitalManagement.Controllers
{
    public class LocationRecordController : Controller
    {
        // GET: Location
        public ActionResult Index()
        {
                return RedirectToAction("Search"); 
        }

        public ActionResult Search(string query, int? page, List<bool> genders, List<Guid> LocationsSelected)
        {
            ViewBag.Host = (Request.Url == null ? "" : Request.Url.Host);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // the code that you want to measure comes here

            if (query == "")
            {
                query = null;
            }

            ViewBag.QueryData = query;
            var pageNumber = (page ?? 1);
            const int pageSize = 5;

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var listData = workScope.LocationRecords.Query(x => !x.IsDelete).ToList();
                var Locations = workScope.Locations.GetAll().ToList();
                var Directors = workScope.Directors.GetAll().ToList();
                var LocationRecords = workScope.LocationRecords.GetAll().ToList();
                ViewBag.Locations = Locations; 
                ViewBag.Directors = Directors;
                ViewBag.LocationRecords = LocationRecords;

                double elapsedMs = 0;

                var q = (
                    from mt in listData
                    join f in Locations on mt.LocationId equals f.Id

                    select mt).AsQueryable();

                if (!string.IsNullOrEmpty(query))
                    q = q.Where(x => x.Title.ToLower().Contains(query.Trim().ToLower())
                                     || !string.IsNullOrEmpty(x.Formation) && x.Formation.ToLower().Contains(query.Trim().ToLower())
                                     || !string.IsNullOrEmpty(x.Location.FullName) && x.Location.FullName.ToLower().Contains(query.Trim().ToLower())

                    );
                if (genders != null && genders.Count > 0)
                    q = q.Where(x => genders != null && genders.Contains(x.Status));
                if (LocationsSelected != null && LocationsSelected.Count > 0)
                    q = q.Where(x => LocationsSelected.Contains(x.Location.Id));
                //
                //
                ViewBag.Total = q.Count();
                ViewBag.LocationsSelected = LocationsSelected;
                ViewBag.Genders = genders;
                watch.Stop();

                elapsedMs = (double)watch.ElapsedMilliseconds / 1000;
                ViewBag.RequestTime = elapsedMs;
                return View(q.ToPagedList(pageNumber, pageSize));
            }
        }
        //
        //
        public ActionResult Detail(Guid id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var LocationRecord = workScope.LocationRecords.Include(x => x.Location).FirstOrDefault(x => x.Id == id);
                if (LocationRecord != null )
                {
                    return View(LocationRecord);
                }
                else
                {
                    return RedirectToAction("Search");
                }
            }
        }
        //
        //
        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var LocationRecord = workScope.LocationRecords.FirstOrDefault(x => x.Id == id && !x.IsDelete);

                return LocationRecord == default ?
                    Json(new
                    {
                        status = false,
                        mess = "Có lỗi xảy ra: "
                    }) :
                    Json(new
                    {
                        status = true,
                        mess = "Lấy thành công ",
                        data = new
                        {
                            LocationRecord.Id,
                            LocationRecord.Title,
                            LocationRecord.TestDate,
                            LocationRecord.test,
                            LocationRecord.test2,
                            LocationRecord.test3,
                            LocationRecord.test4,
                            LocationRecord.test5,
                            LocationRecord.Event,
                            LocationRecord.Event2,
                            LocationRecord.ForManage,
                            LocationRecord.ForManage2,
                            LocationRecord.ForDate,
                            LocationRecord.ForDate2,
                            LocationRecord.EyePressureRight,
                            LocationRecord.EyePressureLeft,
                            LocationRecord.Formation,
                            LocationRecord.Activity,
                            LocationRecord.Activity2,
                            LocationRecord.Activity3,
                            LocationRecord.DirectorId,
                            LocationRecord.RecordId,
                            LocationRecord.LocationId,
                            LocationRecord.ImageProfile,
                            LocationRecord.ImageProfile2,
                            LocationRecord.ImageProfile3,
                        }
                    });
            }
        }
        
    }
}