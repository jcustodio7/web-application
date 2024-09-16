using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using WIndowApp.Models;
namespace WIndowApp.Pages.SKUs
{
    public class EditStock : PageModel
    {
        [BindProperty]
        public SKUinfo StockKeepingUnit {get;set;} = new SKUinfo();
        public string? ErrorMessage { get; private set; }

        public void OnGet(int id)
        {
             try
            {
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();
                    string sqlQuery = "SELECT * FROM SkuInfo WHERE Id=@id";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection)){
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader()){
                            if(reader.Read()){
                                StockKeepingUnit.Id = reader.GetInt32(0);
                                StockKeepingUnit.Name = reader.IsDBNull(1) ? null : reader.GetString(1);
                                StockKeepingUnit.Code = reader.IsDBNull(2) ? null : reader.GetString(2);
                                StockKeepingUnit.UnitPrice = reader.IsDBNull(3) ? null : reader.GetDecimal(3);
                                StockKeepingUnit.DateCreated = reader.IsDBNull(4) ? null : reader.GetDateTime(4);     
                                StockKeepingUnit.CreatedBy = reader.IsDBNull(5) ? null : reader.GetString(5);    
                                StockKeepingUnit.TimeStamp = reader.IsDBNull(6) ? null : reader.GetDateTime(6);
                                StockKeepingUnit.UserId =  reader.IsDBNull(7) ? null : reader.GetString(7) ;
                                StockKeepingUnit.IsActive = reader.IsDBNull(8) ? false : reader.GetBoolean(8);
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
            try
            {
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();

                    string sqlQueryName = "SELECT name FROM SkuInfo where id=@id";
                    string existingName = "";

                    using(SqlCommand command = new SqlCommand(sqlQueryName, connection)){
                        command.Parameters.AddWithValue("@id", StockKeepingUnit.Id);
                        existingName = (string)command.ExecuteScalar();
                    }

                    if(StockKeepingUnit.Name != existingName){
                        string checkNameQuery = "SELECT COUNT(*) FROM SkuInfo where name=@name";
                        using(SqlCommand command = new SqlCommand(checkNameQuery, connection)){
                            command.Parameters.AddWithValue("@name", StockKeepingUnit.Name);
                            int count = (int)command.ExecuteScalar();

                            if(count > 0){
                                ErrorMessage = "A STOCK with this NAME already exists.";
                                return;
                            }
                        }
                    }

                    string sqlQueryCode = "SELECT code FROM SkuInfo where id=@id";
                    string existingCode = "";

                    using(SqlCommand command = new SqlCommand(sqlQueryCode, connection)){
                        command.Parameters.AddWithValue("@id", StockKeepingUnit.Id);
                        existingCode = (string)command.ExecuteScalar();
                    }

                    if(StockKeepingUnit.Code != existingCode){
                        string checkNameQuery = "SELECT COUNT(*) FROM SkuInfo where code=@code";
                        using(SqlCommand command = new SqlCommand(checkNameQuery, connection)){
                            command.Parameters.AddWithValue("@code", StockKeepingUnit.Code);
                            int count = (int)command.ExecuteScalar();

                            if(count > 0){
                                ErrorMessage = "A STOCK with this CODE already exists.";
                                return;
                            }
                        }
                    }


                    string sqlQuery = "UPDATE SkuInfo SET name=@name, code=@code, unitprice=@unitprice, timestamp=@timestamp WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection)){
                        command.Parameters.AddWithValue("@id", StockKeepingUnit.Id);
                        command.Parameters.AddWithValue("@name", StockKeepingUnit.Name);
                        command.Parameters.AddWithValue("@code", StockKeepingUnit.Code);
                        command.Parameters.AddWithValue("@unitprice", StockKeepingUnit.UnitPrice);
                        command.Parameters.AddWithValue("@timestamp", DateTime.Now);

                        command.ExecuteNonQuery();
                     }
                    }
            }
            catch (System.Exception)
            {
                throw;
            }

            Response.Redirect("/SKUs/Index");
        }
    }
}