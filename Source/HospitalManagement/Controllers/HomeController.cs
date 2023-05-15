using BELibrary.Core.Entity;
using BELibrary.DbContext;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string query, int? page, List<bool> genders, List<Guid> CategoriesSelected, List<Guid> LocationsSelected)
        {
            ViewBag.Host = (Request.Url == null ? "" : Request.Url.Host);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            //var test = dbContext.News.FirstOrDefault();
            //var testview = Mapper.Map<NewsViewModel>(test);
            if (query == "")
            {
                query = null;
            }
            ViewBag.QueryData = query;
            var pageNumber = (page ?? 1);
            const int pageSize = 5;
            //  
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var latestPosts = workScope.Articles.Query(x => !x.IsDelete).OrderByDescending(x => x.ModifiedDate).Take(5).ToList();
                ViewBag.LatestPosts = latestPosts;
                var listData = workScope.Locations.Query(x => !x.IsDeleted).ToList();
                var listData1 = workScope.LocationRecords.Query(x => !x.IsDelete).ToList();
                var Categories = workScope.Categories.GetAll().ToList();
                ViewBag.Categories = Categories;
                var Locations = workScope.Locations.GetAll().ToList();
                ViewBag.Locations = Locations;
                var LocationRecords = workScope.LocationRecords.GetAll().ToList();
                ViewBag.LocationRecords = LocationRecords;
                ViewBag.Directors = workScope.Directors.Include(x => x.Faculty).Take(8).ToList();

                double elapsedMs = 0;
                //
                //
                var q1 = (
    from mt1 in listData1
    join f1 in Locations on mt1.LocationId equals f1.Id

    select mt1).AsQueryable();

                if (!string.IsNullOrEmpty(query))
                    q1 = q1.Where(x => x.Formation.ToLower().Contains(query.Trim().ToLower())
                                     || !string.IsNullOrEmpty(x.Title) && x.Title.ToLower().Contains(query.Trim().ToLower())
                                     || !string.IsNullOrEmpty(x.Location.FullName) && x.Location.FullName.ToLower().Contains(query.Trim().ToLower())

                    );

                if (LocationsSelected != null && LocationsSelected.Count > 0)
                    q1 = q1.Where(x => LocationsSelected.Contains(x.Location.Id));

                if (genders != null && genders.Count > 0)
                    q1 = q1.Where(x => genders != null && genders.Contains(x.Status));
                //
                ViewBag.Total = q1.Count();
                ViewBag.LocationsSelected = LocationsSelected;
                ViewBag.Genders = genders;
                watch.Stop();

                elapsedMs = (double)watch.ElapsedMilliseconds / 1000;
                ViewBag.RequestTime = elapsedMs;
                return View();
            }
        }

        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var Location = workScope.Locations.FirstOrDefault(x => x.Id == id && !x.IsDeleted);

                return Location == default ?
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
                            Location.Id,
                            Location.FullName,
                            Location.Address,
                            Location.AreaName,
                            Location.Status,
                            Location.ImageProfile,
                            Location.Description,
                            Location.Coordinate,
                            Location.Scale,
                        }
                    });
            }
        }
    }
}