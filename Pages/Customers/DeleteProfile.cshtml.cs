using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace WIndowApp.Pages.Customers
{
    public class DeleteProfile : PageModel
    {

        public void OnGet()
        {
        }

        public void OnPost(int id){
            deleteProfile(id);
            Response.Redirect("/Customers/Index");
        }

        private void deleteProfile(int id)
        {
            try
            {
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();
                    
                    string sqlQuery = "DELETE FROM CustomersLists WHERE id=@id";
                    using(SqlCommand command = new SqlCommand(sqlQuery, connection)){
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
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