using QuanLyQuanAn.DAO;
using QuanLyQuanAn.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanAn
{

    public partial class FAdmin : Form
    {
        BindingSource accountList = new BindingSource();        //tạo binding accountList (liên kết dữ liệu từ ngồn dữ liệu đến điều khiển giao diện người dùng) 
        public Account loginAccount;
        public FAdmin()
        {
            InitializeComponent();
            Load();
        }

        void Load()
        {
            dtgvAccount.DataSource = accountList;
            LoadAccount();
            AddAccountBinding();
        }

         void AddAccountBinding()       //add account vào binding 
        {
            txbUserName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));        //true, DataSourceUpdateMode.Never để tránh textbox chuyển đổi dữ liệu ngược về dtgv
            txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            nubType.DataBindings.Add(new Binding("value", dtgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));
        }

        private void btnShowAccount_Click(object sender, EventArgs e)       //xem account
        {
            LoadAccount();
        }
        void LoadAccount()                                          
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount(); 
        }

        void AddAccount(string username, string displayname, int type)
        {
            if(AccountDAO.Instance.InsertAccount(username, displayname, type))
            {
                MessageBox.Show("Thêm tài khoảng thành công");
            }
            else
            {
                MessageBox.Show("Thêm tài khoản thất bại");
            }
            LoadAccount();          //sau khi thêm xong thì load lại account
        }

        private void btnAddAccount_Click(object sender, EventArgs e)        //thêm account
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)nubType.Value;

            AddAccount(userName, displayName, type);
        }

        void EditAccount(string username, string displayname, int type)
        {
            if (AccountDAO.Instance.UpdateAccount(username, displayname, type))
            {
                MessageBox.Show("Cập nhật tài khoảng thành công");
            }
            else
            {
                MessageBox.Show("Cập nhật tài khoản thất bại");
            }
            LoadAccount();          //sau khi cập nhật xong thì load lại account
        }

        private void btnEditAccount_Click(object sender, EventArgs e)       //sửa  account 
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)nubType.Value;
            EditAccount(userName, displayName, type); 
        }

        void DeleteAccount(string username)
        {
            if(loginAccount.UserName.Equals(username))
            {
                MessageBox.Show("Vui lòng không xóa tài khoản đang đăng nhập");
                return;             //kết thúc 
            }
            if (AccountDAO.Instance.DeleteAccount(username))
            {
                MessageBox.Show("Xóa tài khoảng thành công");
            }
            else
            {
                MessageBox.Show("Xóa tài khoản thất bại");
            }
            LoadAccount();          //sau khi xóa xong thì load lại account
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)     //xóa account
        {
            string userName = txbUserName.Text;
            DeleteAccount(userName);
        }

        void ResetPass(string userName)
        {
            if (AccountDAO.Instance.ResetPassword(userName))
            {
                MessageBox.Show("Đặt lại mật khẩu  thành công");
            }
            else
            {
                MessageBox.Show("Đặt lại mật khẩu thất bại");
            }
        }
        private void btnResetPassword_Click(object sender, EventArgs e)     //đặt lại mật khẩu 
        {
            string userName = txbUserName.Text;
            ResetPass(userName);
        }
    }
}