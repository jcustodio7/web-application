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
    public class EditProfile : PageModel
    {
        [BindProperty]
        public CustomerInfo Customer { get; set; } = new CustomerInfo();
        public string ErrorMessage { get; private set; }

        public void OnGet(int id)
        {
            try
            {
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();
                    
                    string sqlQuery = "SELECT * from CustomersLists WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection)){
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader()){
                            if(reader.Read()){
                                Customer.Id = reader.GetInt32(0);
                                Customer.FirstName = reader.IsDBNull(1) ? null : reader.GetString(1);
                                Customer.LastName = reader.IsDBNull(2) ? null : reader.GetString(2);
                                Customer.MobileNumber = reader.IsDBNull(3) ? null : reader.GetString(3);
                                Customer.City = reader.IsDBNull(4) ? null : reader.GetString(4);
                                Customer.DateCreated = reader.IsDBNull(5) ? null : reader.GetDateTime(5);     
                                Customer.TimeStamp = reader.IsDBNull(6) ? null : reader.GetDateTime(6);
                                Customer.CreatedBy = reader.IsDBNull(7) ? null : reader.GetString(7);
                                Customer.UserId =  reader.IsDBNull(8) ? null : reader.GetString(8) ;
                                Customer.isActive = reader.IsDBNull(9) ? false: reader.GetBoolean(9);
                                Customer.FullName = reader.IsDBNull(10) ? null : reader.GetString(10);
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

        public void OnPost(){
            try
            {
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();

                    string sqlQueryCheckForMobile = "SELECT mobilenumber FROM CustomersLists WHERE id=@id";
                    string existingMobileNumber = null;

                    using(SqlCommand command = new SqlCommand(sqlQueryCheckForMobile, connection)){
                        command.Parameters.AddWithValue("@id", Customer.Id);
                        existingMobileNumber = (string)command.ExecuteScalar();
                    }

                    if(Customer.MobileNumber != existingMobileNumber){
                        string checkMobileQuery = "SELECT COUNT(*) FROM CustomersLists WHERE mobilenumber=@mobilenumber";
                        using(SqlCommand command = new SqlCommand(checkMobileQuery, connection)){
                            command.Parameters.AddWithValue("@mobilenumber", Customer.MobileNumber);
                            int count = (int)command.ExecuteScalar();

                            if(count > 0){
                                ErrorMessage = "A customer with this mobile number already exists.";
                                return;
                            }
                        }
                    }

                    string sqlQueryCheckForName = "SELECT fullname FROM CustomersLists WHERE id=@id";
                    string existingFullname = "";

                    using(SqlCommand command = new SqlCommand(sqlQueryCheckForName, connection)){
                        command.Parameters.AddWithValue("@id", Customer.Id);
                        existingFullname = (string)command.ExecuteScalar();
                    }
                    string fullnameCustomer = $"{Customer.LastName}, {Customer.FirstName}";
                    if(fullnameCustomer != existingFullname){
                        string checkMobileQuery = "SELECT COUNT(*) FROM CustomersLists WHERE fullname=@fullname";
                        using(SqlCommand command = new SqlCommand(checkMobileQuery, connection)){
                            command.Parameters.AddWithValue("@fullname", fullnameCustomer);
                            int count = (int)command.ExecuteScalar();

                            if(count > 0){
                                ErrorMessage = "A customer with this FULL NAME already exists.";
                                return;
                            }
                        }
                    }
                    
                    string sqlQuery = "UPDATE CustomersLists SET firstname=@firstname, lastname=@lastname, mobilenumber=@mobilenumber, city=@city, timestamp=@timestamp, createdby=@createdby, userid=@userid WHERE id=@id";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection)){
                        command.Parameters.AddWithValue("@firstname", Customer.FirstName);
                        command.Parameters.AddWithValue("@lastname", Customer.LastName);
                        command.Parameters.AddWithValue("@mobilenumber", Customer.MobileNumber);
                        command.Parameters.AddWithValue("@city", Customer.City);
                        command.Parameters.AddWithValue("@timestamp", DateTime.Now);
                        command.Parameters.AddWithValue("@createdby", Customer.CreatedBy);
                        command.Parameters.AddWithValue("@userid", Customer.UserId);
                        command.Parameters.AddWithValue("@id", Customer.Id);

                        command.ExecuteNonQuery();
                     }
                    }
            }
            catch (System.Exception)
            {
                
                throw;
            }

            Response.Redirect("/Customers/Index");
        }
    }
}