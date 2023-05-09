using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileStores.Models
{
    public class Cart
    {
        dbQLDTDataContext data = new dbQLDTDataContext();
        public int iMaDT { set; get; }
        public string sTenDT { set; get; }
        public string sAnhDT { set; get; }
        public Double dGiaban { set; get; }
        public int iSoluong { set; get; }
        public Double dThanhtien {
            get { return iSoluong * dGiaban; }
        }

        //Khởi tạo giỏ hàng theo mã đth được truyền vào với số lượng mặc định là 1

        public Cart(int MaDT)
        {
            iMaDT = MaDT;
            DIENTHOAI dienthoai = data.DIENTHOAIs.Single(n => n.MaDT == iMaDT);
            sTenDT = dienthoai.TenDT;
            sAnhDT = dienthoai.AnhDT;
            dGiaban = double.Parse(dienthoai.Giaban.ToString());
            iSoluong = 1;
        }
    }
}