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
    public class DashboardController : BaseController
    {
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            ViewBag.Element = "Hệ thống";
            ViewBag.Feature = "Bảng điều khiển";
            if (Request.Url != null) ViewBag.BaseURL = Request.Url.LocalPath;

            var user = GetCurrentUser();

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var documents = workScope.Attachments.GetAll().ToList();

                var Locations = workScope.Locations.GetAll().ToList();

                if (user.Role == RoleKey.Director)
                {
                    var locationOfDirectors =
                        workScope.LocationDirectors.Query(x => x.DirectorId == user.DirectorId) ?? new List<LocationDirector>();

                    var locationOfDirectorIds = locationOfDirectors.Select(x => x.LocationId);

                    Locations = Locations.Where(x => locationOfDirectorIds.Contains(x.Id)).ToList();
                }

                var ItemSites = workScope.ItemSites.GetAll().ToList();

                var items = workScope.Items.GetAll().ToList();

                var schedules = workScope.DirectorWorks.GetAll().ToList();

                if (user.Role == RoleKey.Director)
                {
                    schedules = schedules.Where(x => x.DirectorId == user.DirectorId).ToList();
                }
                //
                ViewBag.DocumentCount = documents.Count;
                ViewBag.PatientCount = Locations.Count;
                ViewBag.PrescriptionCount = ItemSites.Count;
                ViewBag.ItemCount = items.Count;
                ViewBag.ScheduleCount = schedules.Count;

                var now = DateTime.Now;
                //
                ViewBag.DocumentTodayCount = documents.Count(x => x.ModifiedDate.Day == now.Day && x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);
                ViewBag.DocumentMonthCount = documents.Count(x => x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);

                ViewBag.ScheduleTodayCount = schedules.Count(x => x.ScheduleBook.Day == now.Day && x.ScheduleBook.Month == now.Month && x.ScheduleBook.Year == now.Year);
                ViewBag.ScheduleMonthCount = schedules.Count(x => x.ScheduleBook.Month == now.Month && x.ScheduleBook.Year == now.Year);

                ViewBag.LocationTodayCount = Locations.Count(x => x.JoinDate.Day == now.Day && x.JoinDate.Month == now.Month && x.JoinDate.Year == now.Year);
                ViewBag.LocationMonthCount = Locations.Count(x => x.JoinDate.Month == now.Month && x.JoinDate.Year == now.Year);

                ViewBag.ItemSiteTodayCount = ItemSites.Count(x => x.ModifiedDate.Day == now.Day && x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);
                ViewBag.ItemSiteMonthCount = documents.Count(x => x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);

                ViewBag.ItemTodayCount = items.Count(x => x.ModifiedDate.Day == now.Day && x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);
                ViewBag.ItemMonthCount = items.Count(x => x.ModifiedDate.Month == now.Month && x.ModifiedDate.Year == now.Year);

                // new patient

                var locationsNew = workScope.Locations.Query(x => x.Status).OrderByDescending(x => x.JoinDate).Take(6).ToList();

                if (user.Role == RoleKey.Director)
                {
                    var locationOfDirectors =
                        workScope.LocationDirectors.Query(x => x.DirectorId == user.DirectorId) ?? new List<LocationDirector>();

                    var locationOfDirectorIds = locationOfDirectors.Select(x => x.LocationId);

                    locationsNew = locationsNew.Where(x => locationOfDirectorIds.Contains(x.Id)).ToList();
                }

                ViewBag.locationsNew = locationsNew;

                var categories = workScope.Categories.GetAll().ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
            }

            return View();
        }
    }
}