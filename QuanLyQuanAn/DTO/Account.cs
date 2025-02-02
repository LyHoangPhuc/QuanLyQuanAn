﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

                
namespace QuanLyQuanAn.DTO                
{
    public class Account                    //tạo model 
    {
        private string userName;
        private string displayName;
        private string password;
        private int type;
        public string UserName 
        { 
            get { return userName; }
            set { userName = value; }
        }
        public string DisplayName 
        {
            get { return displayName; }
            set { displayName = value; } 
        }
        public string PassWord
        {   get { return password; }
            set { password = value; }
        }
        public int Type 
        {
            get { return type; }
            set { type = value; }
        }

        public Account(string userName, string displayName, int type, string password = null)
        {
            this.UserName = userName;
            this.DisplayName = displayName;
            this.PassWord = password;
            this.Type = type;
        }

        public Account(DataRow row)  
        {
            this.UserName = row["userName"].ToString();
            this.DisplayName = row["displayName"].ToString();
            this.Type = (int)row["type"];
            this.PassWord = row["password"].ToString();

        }
    }
}