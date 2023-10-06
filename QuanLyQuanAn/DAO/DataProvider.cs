using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanAn.DAO
{
    public class DataProvider       
    {
                
                private static DataProvider instance;   //tạo Singleton: được sử dụng để đảm bảo rằng một lớp chỉ có duy nhất một thể hiện (instance)
                public static DataProvider Instance     //đóng gói 
                {
                    get { if (instance == null) instance = new DataProvider(); return DataProvider.instance; }
                    private set {DataProvider.instance = value; }          //private để chỉ nội bộ class này được set dữ liệu vào 
                }
                private DataProvider() { }       
        
                private string connectionSTR = "Data Source=.\\SQLEXPRESS;Initial Catalog=QUANLYQUANAN;Integrated Security=True";   //tao chuoi "connectionSTR" de lay duong dan ket noi den csdl

                public DataTable ExecuteQuery(string query, object[] parameter = null)      //tra ra cac dong ket qua
                {
                    DataTable data = new DataTable();
                    using (SqlConnection connection = new SqlConnection(connectionSTR))        //dung SqlConnection de ket noi den csdl va chuyen chuoi "connectionSTR" vao
                    {                                                                          //dùng using để khi kết thúc khối lệch thì nó sẻ được giải phóng
                        connection.Open();                                              //dùng để mở cái conection 
                        SqlCommand command = new SqlCommand(query, connection);         //SqlCommand de thuc thi cau query tren cai ket noi  connection 
                        if(parameter != null)                                       //nếu có paremeter 
                        {
                            string[] listpara = query.Split(' ');                  //split(' ') để tách chuỗi dựa trên khoảng trắng và lưu trữ chuỗi đã tách  vào list 
                            int i = 0;
                            foreach(string item in listpara)                    // với mỗi item trong listpara 
                            {
                                if (item.Contains('@'))                 //nếu item có '@' thì nó có chứa parameter 
                                {
                                    command.Parameters.AddWithValue(item, parameter[i]);   //truyền tham số và giá trị tương ứng  vào  
                                    i++;                                                        //i++ để có thể add được nhiều parameter
                                }
                            }
                        }
                        SqlDataAdapter adapter = new SqlDataAdapter(command);           //SqlDataAdapter (trung gian) de lây du lieu ra
                        adapter.Fill(data);             //sau đó đổ dữ liệu lấy ra được vào cái data
                        connection.Close();             //dùng xong đóng kết nối để tránh tình trạng nhiều dữ liệu đổ về                       
                    }   
                    return data;     
                }

                public int ExecuteNonQuery(string query, object[] parameter = null)    //tra ra cac dong duoc thuc thi: dung cho truong hop insert,update,delete
                {
                    int data = 0;
                    using (SqlConnection connection = new SqlConnection(connectionSTR))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(query, connection);
                        if (parameter != null)
                        {
                            string[] listPara = query.Split(' ');
                            int i = 0;
                            foreach (string item in listPara)
                            {
                                if (item.Contains('@'))
                                {
                                    command.Parameters.AddWithValue(item, parameter[i]);
                                    i++;
                                }
                            }
                        }
                        data = command.ExecuteNonQuery();       //ExecuteNonQuery để trả về số hàng bị ảnh hưởng bởi câu lệch SQL như thêm.sửa xóa. không trả về bất kỳ dữ liệu nào từ csdl
                        connection.Close();
                    }
                    return data;
                }

                public object ExecuteScalar(string query, object[] parameter = null)   //tra ra 1 ket qủa duy nhất như select count(*),....
                {
                    object data = 0;
                    using (SqlConnection connection = new SqlConnection(connectionSTR))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(query, connection);
                        if (parameter != null)
                        {
                            string[] listPara = query.Split(' ');
                            int i = 0;
                            foreach (string item in listPara)
                            {
                                if (item.Contains('@'))
                                {
                                    command.Parameters.AddWithValue(item, parameter[i]);
                                    i++;
                                }
                            }
                        }
                        data = command.ExecuteScalar();     //ExecuteScalar trả về một giá trị duy nhất từ kết quả truy vấn
                        connection.Close();
                    }
                    return data;
                }
    }
}
