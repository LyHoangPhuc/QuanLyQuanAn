﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanAn.DTO
{
    public class Table
    {
        public Table(int iD,string name,string status)
        {
            this.ID = iD;
            this.Name = name;
            this.Status = status;
        }
        public Table(DataRow row)
        {
            this.ID = (int)row["iD"];
            this.Name = row["name"].ToString(); 
            this.Status = row["status"].ToString();
        }
        private int iD;
        private string name;
        private string status;


        public string Name { get => name; set => name = value; }
        public int ID { get => iD; set => iD = value; }
        public string Status { get => status; set => status = value; }
    }
}
