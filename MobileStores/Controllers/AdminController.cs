using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MobileStores.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace MobileStores.Controllers
{
    public class AdminController : Controller
    {
        dbQLDTDataContext data = new dbQLDTDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("SigninAdmin", "Admin");
            }
            return View();
        }

        [HttpGet]
        public ActionResult SigninAdmin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SigninAdmin(FormCollection collection)
        {
            var username = collection["UserAdmin"];
            var passadmin = collection["PassAdmin"];
            if (String.IsNullOrEmpty(username))
            {
                ViewBag.ThongBao1 = "Phải Nhập username";
            }
            if (String.IsNullOrEmpty(passadmin))
            {
                ViewBag.ThongBao2 = "Mật khẩu bắt buộc";
            }
            {
                Admin ad = data.Admins.SingleOrDefault(n => n.UserAdmin == username && n.PassAdmin == passadmin);
                if (ad != null)
                {
                    Session["AdminAccount"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.ThongBao = "Username hoặc Password không đúng";
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("SigninAdmin", "Admin");
        }

        public ActionResult Dienthoai(int ?page)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("SigninAdmin", "Admin");
            }
            return View();
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.DIENTHOAIs.ToList().OrderBy(n => n.MaDT).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult MTB(int ? page)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("SigninAdmin", "Admin");
            }
            return View();
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.MTBs.ToList().OrderBy(n=> n.MaMTB).ToPagedList(pageNumber,pageSize));
        }

        [HttpGet]

        public ActionResult CreateNew()
        {
            ViewBag.MaCD = new SelectList(data.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");// Đổ dropdown từ id chủ đề sang tên chủ đề bên các view
            return View();
        }

        //Chức năng upload hình ảnh

        [HttpPost]
        public ActionResult CreateNew(DIENTHOAI dienthoai, HttpPostedFileBase fileupload)
        {
            ViewBag.MaCD = new SelectList(data.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");

            if (fileupload == null)
            {
                ViewBag.ThongBao = "Chọn 1 ảnh";
                return View();
            }

            else
            {
                if (ModelState.IsValid)
                {
                    //Luư tên file
                    var fileName = Path.GetFileName(fileupload.FileName);
                    //Lưu đường dẫn file
                    var path = Path.Combine(Server.MapPath("~/Content/img/user/mobiles"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình đã tồn tại";
                    }
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    dienthoai.AnhDT = fileName;
                    //Luư vào CSDL
                    data.DIENTHOAIs.InsertOnSubmit(dienthoai);
                    data.SubmitChanges();
         
                }
                return RedirectToAction("Dienthoai","Admin");
            }
        }

        //Xóa sản phẩm admin
        [HttpGet]
        public ActionResult Delete(int id)
        {
            //Xóa điện thoại theo id
            DIENTHOAI dienthoai = data.DIENTHOAIs.SingleOrDefault(n => n.MaDT == id);//Lấy dữ liệu điện thoại từ DB theo id (Mã điện thoại)
            ViewBag.MaDT = dienthoai.MaDT;
            if(dienthoai==null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dienthoai);
        }

        [HttpPost,ActionName("Delete")]

        public ActionResult Xacnhanxoa(int id) // cho BeginForm nút xác nhận xóa bên view Delete
        {
            DIENTHOAI dienthoai = data.DIENTHOAIs.SingleOrDefault(n => n.MaDT == id);//Lấy dữ liệu điện thoại từ DB theo id (Mã điện thoại)
            ViewBag.MaDT = dienthoai.MaDT;
            if (dienthoai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.DIENTHOAIs.DeleteOnSubmit(dienthoai);
            data.SubmitChanges();
            return RedirectToAction("Dienthoai", "Admin");
        }
        //Hết xóa sản phẩm admin

        //Sửa sản phẩm
        [HttpGet]
        public ActionResult Edit(int id)
        {

            
            DIENTHOAI dienthoai = data.DIENTHOAIs.SingleOrDefault(n => n.MaDT == id);//Lấy dữ liệu điện thoại từ DB theo id (Mã điện thoại)
            ViewBag.MaCD = new SelectList(data.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", dienthoai.MaCD ); // Đổ dropdown từ id chủ đề sang tên chủ đề bên các view, thêm (dienthoai.MaCD)--> Là lấy đúng theo tên chủ đề hiện tại của sản phẩm
            if (dienthoai==null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dienthoai);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(DIENTHOAI dienthoai, HttpPostedFileBase fileupload)
        {
            if (fileupload == null)
            {
                ViewBag.ThongBao = "Chọn 1 ảnh";
                return View();
            }

            else
            {
                if (ModelState.IsValid)
                {
                    //Luư tên file
                    var fileName = Path.GetFileName(fileupload.FileName);
                    //Lưu đường dẫn file
                    var path = Path.Combine(Server.MapPath("~/Content/img/user/mobiles"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình đã tồn tại";
                    }
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    dienthoai.AnhDT = fileName;
                    //Luư vào CSDL
                    UpdateModel(dienthoai);
                    data.SubmitChanges();

                }
                return RedirectToAction("Dienthoai", "Admin");
            }
        }
        [HttpGet]
        public ActionResult Details(int id)
        {
            DIENTHOAI dienthoai = data.DIENTHOAIs.SingleOrDefault(n => n.MaDT == id);//Lấy dữ liệu điện thoại từ DB theo id (Mã điện thoại)
            if (dienthoai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dienthoai);
        }

    }
}
