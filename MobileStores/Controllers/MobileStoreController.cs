using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MobileStores.Models;


namespace MobileStores.Controllers
{
    public class MobileStoreController : Controller
    {
        dbQLDTDataContext data = new dbQLDTDataContext();

        // GET: MobileStore
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                iTongSoLuong = lstCart.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }
        private double Tongtien()
        {
            double iTongtien = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                iTongtien = lstCart.Sum(n => n.dThanhtien);
            }
            return iTongtien;
        }

        public ActionResult Index()
        {
            var sanpham = from cd in data.DIENTHOAIs select cd;
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = Tongtien();
            return View(sanpham);

        }

        public ActionResult Chude()
        {
            var chude = from cd in data.CHUDEs select cd;
            return PartialView(chude);
        }

        public ActionResult SPTheochude(int id)
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = Tongtien();
            var dienthoai = from s in data.DIENTHOAIs where s.MaCD == id select s;
            return View(dienthoai);
        }

        public ActionResult Dienthoai()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = Tongtien();
            var dienthoai = from cd in data.DIENTHOAIs select cd;
            return View(dienthoai);
        }

        public ActionResult MTB()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = Tongtien();
            var mtb = from cd in data.MTBs select cd;
            return View(mtb);
        }

        public ActionResult Details(int id)
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = Tongtien();
            var dienthoai = from s in data.DIENTHOAIs where s.MaDT == id select s;
            return View(dienthoai.Single());
        }
        public ActionResult DetailsMTB(int id)
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = Tongtien();
            var MTB = from s in data.MTBs where s.MaMTB == id select s;
            return View(MTB.Single());
        }

        private List<DIENTHOAI> Laydienthoaimoi(int count)
        {
            return data.DIENTHOAIs.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }
        public ActionResult Laydienthoaimoi()
        {
            var dienthoai = Laydienthoaimoi(5);
            return PartialView(dienthoai);
        }

        public ActionResult Dienthoais()
        {
          
            var dienthoai = from cd in data.DIENTHOAIs select cd;
            return PartialView(dienthoai);
        }

        public ActionResult MTBs()
        {
        
           
            var mtb = from cd in data.MTBs select cd;
            return PartialView(mtb);
        }

    }
}