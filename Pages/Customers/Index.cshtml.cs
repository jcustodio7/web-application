using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using WIndowApp.Models;

namespace WIndowApp.Pages.Customers
{
    public class Index : PageModel
    {
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
    }
}