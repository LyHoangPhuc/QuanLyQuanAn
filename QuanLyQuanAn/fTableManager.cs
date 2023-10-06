using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyQuanAn.DAO;
using QuanLyQuanAn.DTO;
using static System.Windows.Forms.ListViewItem;
using Menu = QuanLyQuanAn.DTO.Menu;
using System.Threading;

namespace QuanLyQuanAn
{
    public partial class fTableManager : Form
    {
        private Account loginAccount;       

        public Account LoginAccount
        {
            get { return loginAccount; }
            set { loginAccount = value; changeAccount(loginAccount.Type); }
        }

        public fTableManager(Account acc)
        {
            InitializeComponent();
            this.LoginAccount = acc;        //chuyền tài khoản đăng nhập vào 
            LoadTable();
            LoadCategory();
        }

        void changeAccount(int type)     //thay đổi account 
        {
            adminToolStripMenuItem.Enabled = type == 1;     //adminToolStripMenuItem sử dụng được khi type bằng 1, khác 1 thì không dùng được 
            thôngTinTàiKhoảnToolStripMenuItem.Text += " (" + loginAccount.DisplayName + ")";        //hiện thị thêm displayName bên phần thông tin tài khoản 
        }


        #region Method
        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "Name";
        }
        void LoadFoodListByCategoryID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbfood.DataSource = listFood;
            cbfood.DisplayMember = "Name";
        }
        void LoadTable()
        {
            flpTable.Controls.Clear();
            List<Table> tableList =  TableDAO.Instance.LoadTableList();
            foreach (Table item in tableList)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += btn_Click;
                btn.Tag = item;

                switch (item.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.Aqua;
                        break;
                    default:
                        btn.BackColor = Color.OrangeRed;
                        break;

                }
                flpTable.Controls.Add(btn);
                
            }
        }

        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            float totalPrice = 0;
            foreach (Menu item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;
                lsvBill.Items.Add(lsvItem);
            }
            CultureInfo culture = new CultureInfo("vi-VN");
            Thread.CurrentThread.CurrentCulture = culture;
            txbTotalPrice.Text = totalPrice.ToString("c",culture);
            LoadTable();
        }
        #endregion

        #region Events
        private void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag;
            ShowBill(tableID);
        }
        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FAccountProfile f = new FAccountProfile(loginAccount);
            f.UpdateAccountt += F_UpdateAccountt;
            f.ShowDialog();
        }

        private void F_UpdateAccountt(object sender, AccountEvent e)        //(đổ dữ liệu từ form con đến form cha) cập nhật lại DisplayName ở phần test thông tin tài khoản 
        {
            thôngTinTàiKhoảnToolStripMenuItem.Text = "Thông tin tài khoản (" + e.Acc.DisplayName + ")";
        }
         
        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FAdmin f = new FAdmin();
            f.loginAccount = LoginAccount;
            f.ShowDialog();
        } 
        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;
            ComboBox cb = sender as ComboBox;
            if(cb.SelectedItem == null)
            {
                return;
            }
            Category selected = cb.SelectedItem as Category;
            id = selected.ID;
            LoadFoodListByCategoryID(id);
        }
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int foodID = (cbfood.SelectedItem as Food).ID;
            int count = (int)nmFoodCount.Value;
            if(idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(),foodID,count);
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }
            ShowBill(table.ID);
        }
        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);

            if (idBill != -1)
            {
                if (MessageBox.Show("Bạn có chắc thanh toán hóa đơn cho bàn " + table.Name, "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    BillDAO.Instance.CheckOut(idBill);
                    ShowBill(table.ID);

                    LoadTable();
                }
            }
        }

        #endregion


    }
}
