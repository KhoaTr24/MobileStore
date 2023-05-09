using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MobileStores.Models;

namespace MobileStores.Controllers
{
    public class CartController : Controller
    {
        dbQLDTDataContext data = new dbQLDTDataContext();

        //Lay gio hang
        public List<Cart> Laygiohang()
        {
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart == null)
            {
                lstCart = new List<Cart>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
        }
        //Them gio hang
        public ActionResult AddCart(int iMaDT, string strURL)
        {
            //Lay session gio hang
            List<Cart> lstCart = Laygiohang();
            //Kiem tra có tồn tại chưa
            Cart sanpham = lstCart.Find(n => n.iMaDT == iMaDT);
            if (sanpham == null)
            {
                sanpham = new Cart(iMaDT);
                lstCart.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        //TongSL
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
        //Tong Tien
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

        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cart()
        {
            List<Cart> lstCart = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = Tongtien();
            return View(lstCart);
        }

        public ActionResult CartDel(int iMaSP)
        {
            List<Cart> lstCart = Laygiohang();
            //Kiem tra dth da co trong session Cart o tren
            Cart sanpham = lstCart.SingleOrDefault(n => n.iMaDT == iMaSP);
            //Neu ton tai thi cho sua so luong
            if (sanpham != null)
            {
                lstCart.RemoveAll(n => n.iMaDT == iMaSP);
                return RedirectToAction("Cart");
            }
            return RedirectToAction("Cart");
        }

        public ActionResult CartUpd(int iMaSP, FormCollection f)
        {
            List<Cart> lstCart = Laygiohang();
            Cart sanpham = lstCart.SingleOrDefault(n => n.iMaDT == iMaSP);
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Cart");
        }
        [HttpGet]
        public ActionResult Order()
        {
            //KT Login
            if (Session["Account"] == null || Session["Account"].ToString() == "")
            {
                return RedirectToAction("Signin", "Account");
            }
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "MobileStore");
            }
            //Lay gio hang tu session
            List<Cart> lstCart = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = Tongtien();
            return View(lstCart);
        }
        [HttpPost, ActionName("Order")]
        public ActionResult Xacnhandathang()
        {
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Account"];
            List<Cart> lstCart = Laygiohang();
            kh.MaKH = ddh.MaDH;
            ddh.Ngaydat = DateTime.Now;
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Chi tiet don hang
            foreach(var item in lstCart)
            {
                CHITIETDH ctdh = new CHITIETDH();
                ctdh.MaDH = ddh.MaDH;
                ctdh.MaDT = item.iMaDT;
                ctdh.Soluong = item.iSoluong;
                ctdh.Dongia =(Decimal) item.dThanhtien;
                data.CHITIETDHs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["Cart"] = null;
            return RedirectToAction("OrderConfirm","Cart");
        }

        public ActionResult OrderConfirm()
        {
            return View();
        }
    }
}