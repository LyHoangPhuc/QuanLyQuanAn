using QuanLyQuanAn.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanAn.DAO                
{
    public class AccountDAO
    {
        private static AccountDAO instance;    //cấu trúc Singleton
        public static AccountDAO Instance
        {
            get { if (instance == null) instance = new AccountDAO(); return instance; }
            private set { instance = value; }
        }

        private AccountDAO() { }

        public bool Login(string userName, string passWord)         
        { 
            string query = "USP_Login @userName , @passWord  ";
            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[]{userName, passWord});       //trả về 1 cái datatable    
            return result.Rows.Count > 0;           //kết quả trả về với điều kiện số dòng trả ra phải lớn hơn 0 
        }

        public Account GetAccountByUserName(string userName)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("Select *from account where userName = '" + userName + "'");
            foreach (DataRow item in data.Rows)         //với mỗi item trong data row thì 
            {
                return new Account(item);           
            }
            
            return null;

        }

        public DataTable GetListAccount()               //hiển thị danh sách tài khoản 
        {
            return DataProvider.Instance.ExecuteQuery("select UserName, DisplayName, Type from dbo.Account");
        }


        public bool UpdateAccount(string userName, string displayName, string pass, string newPass)
        {
            int result = DataProvider.Instance.ExecuteNonQuery("USP_UpdateAccount @userName , @displayName , @password , @newPassword ", new object[]{ userName, displayName, pass, newPass});
            return result > 0;
        }

        public bool InsertAccount(string name, string displayName, int type)       //insert account
        {
            string query = string.Format("insert dbo.Account (Username, DisplayName, Type)values (N'{0}' , N'{1}' , {2})", name, displayName, type);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        public bool UpdateAccount(string name, string displayName, int type)    //update account
        {
            string query = string.Format("update dbo.Account set DisplayName = N'{1}', Type = {2} where UserName = N'{0}'", name, displayName, type);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public bool DeleteAccount(string name)           //delete account 
        {
            string query = string.Format("delete Account where UserName = '{0}'", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }
        
        public bool ResetPassword(string name)
        {
            string query = string.Format("update Account set password = N'0' where UserName = '{0}'", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

    }
}