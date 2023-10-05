using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLyQuanAn.DTO;

namespace QuanLyQuanAn.DAO
{
    public class MenuDAO
    {
        private static MenuDAO instance;

        public static MenuDAO Instance
        {
            get { if (instance == null) instance = new MenuDAO(); return MenuDAO.instance; }
            private set { MenuDAO.instance = value; }
        }

        private MenuDAO() { }
        public List<Menu> GetListMenuByTable(int id)
        {
            List<Menu> ListMenu = new List<Menu>();

            string query = "SELECT f.name, bi.COUNT, f.price, f.price* bi.COUNT AS totalPrice From dbo.BillInfo as bi, dbo.Bill as b, dbo.Food as f where bi.idBill = b.id AND bi.idFood = f.id AND b.status = 0 AND b.idTable = " + id;
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Menu menu = new Menu(item);
                ListMenu.Add(menu);
            }

            return ListMenu;
        }
    }
}
