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
    public class EditOrder : PageModel
    {
        [BindProperty]
        public OrderInfo orderInfo {get;set;} = new OrderInfo();
        public List<PurchaseItem> purchaseItemsList {get;set;} = new List<PurchaseItem>();

        public List<CustomerInfo>? CustomerLists {get;set;} = new List<CustomerInfo>();

        [BindProperty]
        public DateTime dateformat {get;set;}
        public string? purchaseOrderId {get;set;}

        [BindProperty]
        public decimal? totalAmount {get;set;}
        public DateTime datepost {get;set;}
        public DateTime dateTomorrow {get;set;}
        public string ErrorMessage { get; private set; }



        public void OnGet(int id)
        {
            try
            {
                purchaseOrderId= Request.Query["id"];
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();
                    string sqlQuery = "SELECT * FROM PurchaseOrder WHERE Id=@id";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection)){
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader()){
                            if(reader.Read()){
                                orderInfo.Id = reader.GetInt32(0);
                                orderInfo.CustomerName = reader.IsDBNull(1) ? null : reader.GetString(1);
                                orderInfo.DeliveryDate = reader.IsDBNull(2) ? null : reader.GetString(2);
                                dateformat = DateTime.Parse(orderInfo.DeliveryDate);
                                orderInfo.OrderStatus = reader.IsDBNull(3) ? null : reader.GetString(3);
                                orderInfo.AmountDue = reader.IsDBNull(4) ? null : reader.GetDecimal(4);     
                                orderInfo.DateCreated = reader.IsDBNull(5) ? null : reader.GetDateTime(5);    
                                orderInfo.CreatedBy = reader.IsDBNull(6) ? null : reader.GetString(6);
                                orderInfo.TimeStamp =  reader.IsDBNull(7) ? null : reader.GetDateTime(7) ;
                                orderInfo.UserId = reader.IsDBNull(8) ? null : reader.GetString(8);
                                orderInfo.IsActive = reader.IsDBNull(9) ? false : reader.GetBoolean(9);
                            }
                        }
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();
                    string sql = "SELECT * FROM PurchaseItems where purchaseorderid=@id";

                    using (SqlCommand command = new SqlCommand(sql, connection)){
                        command.Parameters.AddWithValue("@id", id);
                        using(SqlDataReader reader = command.ExecuteReader()){
                            while(reader.Read()){
                                PurchaseItem purchaseItem = new PurchaseItem();
                                purchaseItem.Id = reader.GetInt32(0);
                                purchaseItem.PurchaseOrderID = reader.IsDBNull(1) ? null : reader.GetInt32(1);
                                purchaseItem.SkuID = reader.IsDBNull(2) ? null : reader.GetInt32(2);
                                purchaseItem.Quantity = reader.IsDBNull(3) ? null : reader.GetInt32(3);
                                purchaseItem.Price = reader.IsDBNull(4) ? null : reader.GetDecimal(4); 
                                purchaseItem.UserId =  reader.IsDBNull(6) ? null : reader.GetString(6) ;
                                purchaseItem.ItemName =  reader.IsDBNull(7) ? null : reader.GetString(7);
                                
                                purchaseItemsList?.Add(purchaseItem);
                            }    
                        }
                    }
                }
                 totalAmount = purchaseItemsList.Sum(item => item.Price);
            }
            catch(Exception e){
                Console.WriteLine($"Error: {e}");
            }
        }

        public void OnPostSubmit(){
                purchaseOrderId= Request.Query["id"];
                purchaseItemsList = GetPurchaseItems(orderInfo.Id);
            try
            {   
                string formattedDate = dateformat.ToString("MMM dd yyyy hh:mmtt");
                datepost = DateTime.Parse(formattedDate);
                dateTomorrow = DateTime.Now.Date.AddDays(1);    
                if(datepost < dateTomorrow){
                    ErrorMessage = "Delivery date must NOT be less than tomorrow's date.";
                    return;
                }
                //datepost = DateTime.Parse(dateformat);
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using(SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();

                    string sqlQuery = "UPDATE PurchaseOrder SET customername=@customername, deliverydate=@deliverydate, orderstatus=@orderstatus, amountdue=@amountdue, timestamp=@timestamp WHERE id=@id";
                    using(SqlCommand command = new SqlCommand(sqlQuery, connection)){
                        command.Parameters.AddWithValue("@id", orderInfo.Id);
                        command.Parameters.AddWithValue("@customername", orderInfo.CustomerName);
                        command.Parameters.AddWithValue("@deliverydate", formattedDate);
                        command.Parameters.AddWithValue("@orderstatus", orderInfo.OrderStatus);
                        command.Parameters.AddWithValue("@amountdue", totalAmount);
                        command.Parameters.AddWithValue("@timestamp", DateTime.Now);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (System.Exception)
            {
                return;
            }

            Response.Redirect($"/OrderLists/Index");
        }

        public List<PurchaseItem> GetPurchaseItems(int id){

            string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
            using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();
                    string sql = "SELECT * FROM PurchaseItems where purchaseorderid=@id";

                    using (SqlCommand command = new SqlCommand(sql, connection)){
                        command.Parameters.AddWithValue("@id", id);
                        using(SqlDataReader reader = command.ExecuteReader()){
                            while(reader.Read()){
                                PurchaseItem purchaseItem = new PurchaseItem();
                                purchaseItem.Id = reader.GetInt32(0);
                                purchaseItem.PurchaseOrderID = reader.IsDBNull(1) ? null : reader.GetInt32(1);
                                purchaseItem.SkuID = reader.IsDBNull(2) ? null : reader.GetInt32(2);
                                purchaseItem.Quantity = reader.IsDBNull(3) ? null : reader.GetInt32(3);
                                purchaseItem.Price = reader.IsDBNull(4) ? null : reader.GetDecimal(4); 
                                purchaseItem.UserId =  reader.IsDBNull(6) ? null : reader.GetString(6) ;
                                purchaseItem.ItemName =  reader.IsDBNull(7) ? null : reader.GetString(7);
                                
                                purchaseItemsList?.Add(purchaseItem);
                            }    
                        }
                    }
                }
                 totalAmount = purchaseItemsList.Sum(item => item.Price);
            return purchaseItemsList;
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