using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanAn.DTO
{
    public class BillInfo
    {
        public BillInfo(int iD, int billID,int foodID,int count)
        {
            this.ID = iD;
            this.BillID = billID;
            this.FoodID = foodID;
            this.Count = count;
        }
        public BillInfo(DataRow row)
        {
            this.ID = (int)row["iD"];
            this.BillID = (int)row["idbill"];
            this.FoodID = (int)row["idfood"];
            this.Count = (int)row["count"];
        }
        private int iD;
        private int foodID;
        private int billID;
        private int count; //food count
        public int ID { get => iD; set => iD = value; }
        public int FoodID { get => foodID; set => foodID = value; }
        public int BillID { get => billID; set => billID = value; }
        public int Count { get => count; set => count = value; }
    }
}
