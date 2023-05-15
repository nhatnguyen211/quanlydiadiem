using BELibrary.Core.Entity;
using BELibrary.DbContext;
using BELibrary.Entity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class ItemSiteController : BaseController
    {
        // GET: Admin/Attachment
        private const string KeyElement = "CTLH";

        // GET: Admin/Event
        public ActionResult Index(Guid detailRecordId)
        {
            ViewBag.Feature = "Danh sách";
            ViewBag.Element = KeyElement;
            ViewBag.DetailRecordId = detailRecordId;

            ViewBag.BaseURL = "#";

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var Movements = workScope.Movements.GetAll().ToList();
                ViewBag.Movements = new SelectList(Movements, "Id", "Name");

                var listData = workScope.ItemSites
                    .Include(x => x.DetailItemSite)
                    .Where(x => x.DetailRecordId == detailRecordId)
                    .ToList();

                return View(listData);
            }
        }

        [HttpPost]
        public JsonResult GetJson(Guid? id)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var detail = workScope.DetailItemSites.FirstOrDefault(x => x.Id == id);

                return detail == default ?
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
                            detail.Id,
                            detail.MovementId,
                            detail.Amount,
                            detail.Unit,
                            detail.Note
                        }
                    });
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult CreateOrEdit(DetailItemSite input, ItemSite itemSite, bool isEdit)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    if (isEdit)
                    {
                        var elm = workScope.DetailItemSites.Get(input.Id);

                        if (elm != null) //update
                        {
                            elm = input;
                            workScope.DetailItemSites.Put(elm, elm.Id);
                            workScope.Complete();

                            //var attachmentAssign = workScope.AttachmentAssigns
                            //    .FirstOrDefault(x =>
                            //        x.DetailRecordId == assign.DetailRecordId && x.AttachmentId == elm.Id);

                            //if (attachmentAssign == null)
                            //{
                            //    workScope.AttachmentAssigns.Add(new AttachmentAssign
                            //    {
                            //        Id = Guid.NewGuid(),
                            //        DetailRecordId = assign.DetailRecordId,
                            //        AttachmentId = elm.Id
                            //    });

                            //    workScope.Complete();
                            //}
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

                        workScope.DetailItemSites.Add(input);
                        workScope.Complete();

                        workScope.ItemSites.Add(new ItemSite
                        {
                            Id = Guid.NewGuid(),
                            DetailRecordId = itemSite.DetailRecordId,
                            DetailItemSiteId = input.Id,
                            CreatedBy = GetCurrentUser().FullName,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            ModifiedBy = GetCurrentUser().FullName
                        });

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
                    var elm = workScope.DetailItemSites.Get(id);
                    if (elm != null)
                    {
                        workScope.DetailItemSites.Remove(elm);
                        //del
                        var ItemSites = workScope.ItemSites.Query(x => x.DetailItemSiteId == elm.Id);
                        foreach (var itemSite in ItemSites)
                        {
                            workScope.ItemSites.Remove(itemSite);
                        }
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