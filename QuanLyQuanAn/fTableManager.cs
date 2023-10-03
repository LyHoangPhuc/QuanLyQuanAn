using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyQuanAn.DAO;
using QuanLyQuanAn.DTO;

namespace QuanLyQuanAn
{
    public partial class fTableManager : Form
    {
        public fTableManager()
        {
            InitializeComponent();
            LoadTable();
        }
        #region Method
        void LoadTable()
        {
            List<Table> tableList =  TableDAO.Instance.LoadTableList();
            foreach (Table item in tableList)
            {
                Button btn = new Button();
                
            }
        }
        #endregion

        #region Events

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FAccountProfile f = new FAccountProfile();
            f.ShowDialog();

        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FAdmin f = new FAdmin();
            f.ShowDialog();
        }
        #endregion

    }
}
