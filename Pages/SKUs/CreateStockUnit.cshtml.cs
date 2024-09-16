using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using WIndowApp.Models;

namespace WIndowApp.Pages.SKUs
{
    public class CreateStockUnit : PageModel
    {
        [BindProperty]
        public SKUinfo StockKeepingUnit {get;set;} = new SKUinfo();
        public string? ErrorMessage { get;set; }

        public void OnGet()
        {
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

                string checkQueryForName = "SELECT COUNT(*) FROM SkuInfo WHERE name=@name";
                using (SqlCommand command = new SqlCommand(checkQueryForName, connection)){
                    command.Parameters.AddWithValue("@name", StockKeepingUnit.Name);
                    int count = (int)command.ExecuteScalar();
                    if(count > 0){
                        ErrorMessage = "A STOCK with this NAME already exists.";
                        return;
                    }
                }

                string checkQueryForCode = "SELECT COUNT(*) FROM SkuInfo WHERE code=@code";
                using (SqlCommand command = new SqlCommand(checkQueryForCode, connection)){
                    command.Parameters.AddWithValue("@code", StockKeepingUnit.Code);
                    int count = (int)command.ExecuteScalar();
                    if(count > 0){
                        ErrorMessage = "A STOCK with this CODE already exists.";
                        return;
                    }
                }

                string sqlQuery = "INSERT INTO SkuInfo " + "(name, code, unitprice, DateCreated, createdby, TIMESTAMP, userid, isActive) " + 
                                    "VALUES (@name, @code, @unitprice, @DateCreated, @createdby, @timestamp, @userid, @isactive);";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection)){
                    command.Parameters.AddWithValue("@name", StockKeepingUnit.Name);
                    command.Parameters.AddWithValue("@code", StockKeepingUnit.Code);
                    command.Parameters.AddWithValue("@unitprice", StockKeepingUnit.UnitPrice);
                    command.Parameters.AddWithValue("@datecreated", DateTime.Now);
                    command.Parameters.AddWithValue("@createdby", StockKeepingUnit.CreatedBy);
                    command.Parameters.AddWithValue("@timestamp", DateTime.Now);
                    command.Parameters.AddWithValue("@userid", StockKeepingUnit.UserId);
                    command.Parameters.AddWithValue("@isactive", true);

                    command.ExecuteNonQuery();
                }
            }

            } catch  (Exception e){
                ErrorMessage = e.Message;
                return;
            }

            Response.Redirect("/SKUs/Index");
        }
    }
}