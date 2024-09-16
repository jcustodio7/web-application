using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using WIndowApp.Models;
namespace WIndowApp.Pages.OrderLists
{
    public class Index : PageModel
    {
        public List<OrderInfo> OrderLists {get;set;} = new List<OrderInfo>();

        public DateTime dateFormat {get;set;}
        public void OnGet()
        {
            try
            {
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();
                    string sql = "SELECT * FROM PurchaseOrder";

                    using (SqlCommand command = new SqlCommand(sql, connection)){
                        using(SqlDataReader reader = command.ExecuteReader()){
                            while(reader.Read()){
                                OrderInfo orderList = new OrderInfo();
                                orderList.Id = reader.GetInt32(0);
                                orderList.CustomerName = reader.IsDBNull(1) ? null : reader.GetString(1);
                                orderList.DeliveryDate = reader.IsDBNull(2) ? null : reader.GetString(2);
                                orderList.OrderStatus = reader.IsDBNull(3) ? null : reader.GetString(3);
                                orderList.AmountDue = reader.IsDBNull(4) ? null : reader.GetDecimal(4); 
                                orderList.DateCreated = reader.IsDBNull(5) ? null : reader.GetDateTime(5);
                                orderList.CreatedBy = reader.IsDBNull(6) ? null : reader.GetString(6);    
                                orderList.TimeStamp = reader.IsDBNull(7) ? null : reader.GetDateTime(7);
                                orderList.UserId =  reader.IsDBNull(8) ? null : reader.GetString(8) ;
                                orderList.IsActive = reader.IsDBNull(9) ? false : reader.GetBoolean(9);
                                
                                OrderLists?.Add(orderList);
                            }    
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }
}