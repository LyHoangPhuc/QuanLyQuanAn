using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLyQuanAn.DTO;

namespace QuanLyQuanAn.DAO
{
    public class BillInfoDAO
    {
        private static BillInfoDAO instance;
        public static BillInfoDAO Instance
        {
            get { if (instance == null) instance = new BillInfoDAO(); return BillInfoDAO.instance; }
            private set { BillInfoDAO.instance = value; }
        }
        private BillInfoDAO() { }

        public List<BillInfo> GetListBillInfo(int id)
        {
            List<BillInfo> ListBillInfo = new List<BillInfo>();

            DataTable data = DataProvider.Instance.ExecuteQuery("Select * from dbo.BillInfo where idBill =" + id);
            foreach (DataRow item in data.Rows) 
            {
                BillInfo info = new BillInfo(item);
                ListBillInfo.Add(info);
            } 
            return ListBillInfo;
        }

    }
}
