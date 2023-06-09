﻿using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class ItemSupplyController : BaseController
    {
        // GET: Admin/Attachment
        private const string KeyElement = "TTCT";

        // GET: Admin/Event
        public ActionResult Index(Guid LocationId)
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;
            ViewBag.LocationId = LocationId;

            ViewBag.BaseURL = "#";

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var listData = workScope.ItemSupplies
                    .Include(x => x.Item).Where(x => x.LocationId == LocationId).ToList();

                var items = workScope.Items.GetAll().ToList();
                ViewBag.Items = new SelectList(items, "Id", "Name");

                var lstStatus = StatusMedical.GetDic();
                ViewBag.ListStatus = new SelectList(lstStatus, "Value", "Text");
                return View(listData);
            }
        }

        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var itemSupply = workScope.ItemSupplies.FirstOrDefault(x => x.Id == id);

                return itemSupply == default ?
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
                            itemSupply.Id,
                            itemSupply.Amount,
                            itemSupply.ItemId,
                            DateHide = itemSupply.DateOfHire.ToString("g"),
                            itemSupply.Status
                        }
                    });
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(ItemSupply input, AttachmentAssign assign, bool isEdit)
        {
            try
            {
                if (input.Amount <= 0)
                {
                    return Json(new { status = false, mess = "Lỗi số lượng" });
                }
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var item = workScope.Items.FirstOrDefault(x => x.Id == input.ItemId);
                    var ItemSupplies = workScope.ItemSupplies.Query(x => x.ItemId == input.ItemId).ToList();

                    var hireCount = ItemSupplies.Where(x => x.Status == StatusMedical.Hired).Sum(x => x.Amount);
                    var availabilityCount = ItemSupplies.Where(x => x.Status == StatusMedical.Availability).Sum(x => x.Amount);
                    var expiredCount = ItemSupplies.Where(x => x.Status == StatusMedical.Expired).Sum(x => x.Amount);
                    var unavailableCount = ItemSupplies.Where(x => x.Status == StatusMedical.Unavailable).Sum(x => x.Amount);
                    var maintenanceCount = ItemSupplies.Where(x => x.Status == StatusMedical.Maintenance).Sum(x => x.Amount);

                    var availabilityItem = item.Amount - hireCount - expiredCount - unavailableCount - maintenanceCount;

                    if (availabilityItem < 0)
                    {
                        return Json(new { status = false, mess = "Lỗi, dữ liệu âm" + KeyElement });
                    }

                    if (input.Amount > availabilityItem)
                    {
                        return Json(new { status = false, mess = $"Lỗi, kho đã hết" + KeyElement });
                    }
                    if (isEdit)
                    {
                        var elm = workScope.ItemSupplies.Get(input.Id);

                        if (elm != null) //update
                        {
                            elm = input;
                            workScope.ItemSupplies.Put(elm, elm.Id);
                            workScope.Complete();

                            return Json(new { status = true, mess = "Cập nhập thành công " });
                        }
                        else
                        {
                            return Json(new { status = false, mess = "Không tồn tại " + KeyElement });
                        }
                    }
                    else
                    {
                        input.Id = Guid.NewGuid();

                        workScope.ItemSupplies.Add(input);
                        workScope.Complete();

                        return Json(new { status = true, mess = "Thêm thành công " + KeyElement });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, mess = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.ItemSupplies.Get(id);
                    if (elm != null)
                    {
                        workScope.ItemSupplies.Remove(elm);
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