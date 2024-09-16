using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using WIndowApp.Models;
namespace WIndowApp.Pages.OrderLists
{
    public class CreateOrder : PageModel
    {
        public List<PurchaseItem> purchaseItemsList {get;set;} = new List<PurchaseItem>();

        [BindProperty]
        public PurchaseItem PurchaseItem {get;set;} = new PurchaseItem();

        [BindProperty]
        public OrderInfo orderInfo {get;set;} = new OrderInfo();

        public DateTime datePost {get;set;}

        public DateTime dateTomorrow {get;set;}
        public string ErrorMessage { get; private set; }
        public List<CustomerInfo>? CustomerLists {get;set;} = new List<CustomerInfo>();

        public void OnGet()
        {
             try
            {
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();
                    string sql = "SELECT * FROM CustomersLists";

                    using (SqlCommand command = new SqlCommand(sql, connection)){
                        using(SqlDataReader reader = command.ExecuteReader()){
                            while(reader.Read()){
                                CustomerInfo customerInfo = new CustomerInfo();
                                customerInfo.Id = reader.GetInt32(0);
                                customerInfo.FirstName = reader.IsDBNull(1) ? null : reader.GetString(1);
                                customerInfo.LastName = reader.IsDBNull(2) ? null : reader.GetString(2);
                                customerInfo.MobileNumber = reader.IsDBNull(3) ? null : reader.GetString(3);
                                customerInfo.City = reader.IsDBNull(4) ? null : reader.GetString(4);
                                customerInfo.DateCreated = reader.IsDBNull(5) ? null : reader.GetDateTime(5);     
                                customerInfo.TimeStamp = reader.IsDBNull(6) ? null : reader.GetDateTime(6);
                                customerInfo.CreatedBy = reader.IsDBNull(7) ? null : reader.GetString(7);
                                customerInfo.UserId =  reader.IsDBNull(8) ? null : reader.GetString(8) ;
                                customerInfo.isActive = reader.IsDBNull(9) ? false : reader.GetBoolean(9);
                                customerInfo.FullName = reader.IsDBNull(10) ? null : reader.GetString(10);
                                
                                CustomerLists?.Add(customerInfo);
                            }    
                        }
                    }
                }
            }
            catch(Exception e){
                Console.WriteLine($"Error: {e}");
            }

        }
        
        public void OnPostSubmit()
        {
            CustomerLists = GetCustomerList();
            try {
                datePost = DateTime.Parse(orderInfo.DeliveryDate);
                dateTomorrow = DateTime.Now.Date.AddDays(1);    
                if(datePost < dateTomorrow){
                    ErrorMessage = "Delivery date must NOT be less than tomorrow's date.";
                    return;
                }
                string formattedDate = datePost.ToString("MMM dd yyyy hh:mmtt");
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";

                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();

                    string checkQueryForFullname = "SELECT COUNT(*) FROM PurchaseOrder WHERE customername=@fullname";
                    using (SqlCommand command = new SqlCommand(checkQueryForFullname, connection)){
                        command.Parameters.AddWithValue("@fullname", orderInfo.CustomerName);
                        int count = (int)command.ExecuteScalar();
                        if(count > 0){
                            ErrorMessage = "An order with this Customer already exists.";
                            return;
                        }
                    }
            
                    string sqlQuery = "INSERT INTO PurchaseOrder " + "(customername, deliverydate, orderstatus, datecreated, timestamp) " + 
                                        "VALUES (@customername, @deliverydate, @orderstatus, @datecreated, @timestamp);";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection)){
                        command.Parameters.AddWithValue("@customername", orderInfo.CustomerName);
                        command.Parameters.AddWithValue("@deliverydate", formattedDate);
                        command.Parameters.AddWithValue("@orderstatus", orderInfo.OrderStatus);
                        command.Parameters.AddWithValue("@datecreated", DateTime.Now);
                        command.Parameters.AddWithValue("@timestamp", DateTime.Now);

                        command.ExecuteNonQuery();
                    }
                }
            } 
            catch  (Exception e){
                return;
            }
            Response.Redirect("/OrderLists/Index");    
        }

        public List<CustomerInfo> GetCustomerList()
        {   
            string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
            using (SqlConnection connection = new SqlConnection(connectionString)){
                connection.Open();
                string sql = "SELECT * FROM CustomersLists";

                using (SqlCommand command = new SqlCommand(sql, connection)){
                    using(SqlDataReader reader = command.ExecuteReader()){
                        while(reader.Read()){
                            CustomerInfo customerInfo = new CustomerInfo();
                            customerInfo.Id = reader.GetInt32(0);
                            customerInfo.FirstName = reader.IsDBNull(1) ? null : reader.GetString(1);
                            customerInfo.LastName = reader.IsDBNull(2) ? null : reader.GetString(2);
                            customerInfo.MobileNumber = reader.IsDBNull(3) ? null : reader.GetString(3);
                            customerInfo.City = reader.IsDBNull(4) ? null : reader.GetString(4);
                            customerInfo.DateCreated = reader.IsDBNull(5) ? null : reader.GetDateTime(5);     
                            customerInfo.TimeStamp = reader.IsDBNull(6) ? null : reader.GetDateTime(6);
                            customerInfo.CreatedBy = reader.IsDBNull(7) ? null : reader.GetString(7);
                            customerInfo.UserId =  reader.IsDBNull(8) ? null : reader.GetString(8) ;
                            customerInfo.isActive = reader.IsDBNull(9) ? false : reader.GetBoolean(9);
                            customerInfo.FullName = reader.IsDBNull(10) ? null : reader.GetString(10);
                            
                            CustomerLists?.Add(customerInfo);
                        }    
                    }
                }
                return CustomerLists;
            }
        }
    }
}