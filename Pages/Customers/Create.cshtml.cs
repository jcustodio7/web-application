using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using WIndowApp.Models;

namespace WIndowApp.Pages.Customers
{
    public class Create : PageModel
    {

        [BindProperty]
        public CustomerInfo Customer { get; set; } = new CustomerInfo();

        public List<CustomerInfo>? CustomerLists {get;set;} = new List<CustomerInfo>();

        public string ErrorMessage {get;set;} 
        
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
                                customerInfo.FullName = reader.IsDBNull(3) ? null : reader.GetString(3);
                                customerInfo.MobileNumber = reader.IsDBNull(4) ? null : reader.GetString(4);
                                customerInfo.City = reader.IsDBNull(5) ? null : reader.GetString(5);
                                customerInfo.DateCreated = reader.IsDBNull(6) ? null : reader.GetDateTime(6);     
                                customerInfo.TimeStamp = reader.IsDBNull(7) ? null : reader.GetDateTime(7);
                                customerInfo.CreatedBy = reader.IsDBNull(8) ? null : reader.GetString(8);
                                customerInfo.UserId =  reader.IsDBNull(9) ? null : reader.GetString(9) ;
                                customerInfo.isActive = reader.IsDBNull(10) ? false : reader.GetBoolean(10);
                                
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
        public void OnPost()
        {
            if(!ModelState.IsValid){
                return;
            }
            try {
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();

                    string checkQueryForMobile = "SELECT COUNT(*) FROM CustomersLists WHERE mobilenumber=@mobilenumber";
                    using (SqlCommand checkCommand = new SqlCommand(checkQueryForMobile, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@mobilenumber", Customer.MobileNumber);
                        int count = (int)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            // Mobile number already exists
                            ErrorMessage = "A customer with this mobile number already exists.";
                            return;
                        }
                    }

                    string checkQueryForFullname = "SELECT COUNT(*) FROM CustomersLists WHERE fullname=@fullname";
                    using (SqlCommand checkCommand = new SqlCommand(checkQueryForFullname, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@fullname", Customer.LastName + ", " + Customer.FirstName);
                        int count = (int)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            ErrorMessage = "A customer with this name already exists.";
                            return;
                        }
                    }

                    string sqlQuery = "INSERT INTO CustomersLists " + "(firstname, lastname, mobilenumber, city, datecreated, timestamp, createdby, userid, isactive) " + 
                                    "VALUES (@firstname, @lastname, @mobilenumber, @city, @datecreated, SYSDATETIME(), @createdby, @userid, @isactive);";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection)){
                        command.Parameters.AddWithValue("@firstname", Customer.FirstName);
                        command.Parameters.AddWithValue("@lastname", Customer.LastName);
                        command.Parameters.AddWithValue("@mobilenumber", Customer.MobileNumber);
                        command.Parameters.AddWithValue("@city", Customer.City);
                        command.Parameters.AddWithValue("@datecreated", DateTime.Now);
                        command.Parameters.AddWithValue("@createdby", Customer.CreatedBy);
                        command.Parameters.AddWithValue("@userid", Customer.UserId);
                        command.Parameters.AddWithValue("@isactive", true);
                        command.ExecuteNonQuery();
                    }
                }

            } catch  (Exception e){
                ErrorMessage = e.Message;
                return;
            }

            Response.Redirect("/Customers/Index");
        }
    }
}