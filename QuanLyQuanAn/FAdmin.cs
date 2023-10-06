using QuanLyQuanAn.DAO;
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
            txbUserName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName"));
            txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName"));
        }

        void LoadAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount(); 
        }

        private void btnShowAccount_Click(object sender, EventArgs e)       //xem account
        {
            LoadAccount();
        }
    }
}