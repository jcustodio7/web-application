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
    public class Index : PageModel
    {
        public List<SKUinfo> StockKeepingUnit {get;set;} = new List<SKUinfo>();

        public void OnGet()
        {
            try
            {
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();
                    string sql = "SELECT * FROM SkuInfo";

                    using (SqlCommand command = new SqlCommand(sql, connection)){
                        using(SqlDataReader reader = command.ExecuteReader()){
                            while(reader.Read()){
                                SKUinfo stockKeepingUnit = new SKUinfo();
                                stockKeepingUnit.Id = reader.GetInt32(0);
                                stockKeepingUnit.Name = reader.IsDBNull(1) ? null : reader.GetString(1);
                                stockKeepingUnit.Code = reader.IsDBNull(2) ? null : reader.GetString(2);
                                stockKeepingUnit.UnitPrice = reader.IsDBNull(3) ? null : reader.GetDecimal(3);
                                stockKeepingUnit.DateCreated = reader.IsDBNull(4) ? null : reader.GetDateTime(4);     
                                stockKeepingUnit.CreatedBy = reader.IsDBNull(5) ? null : reader.GetString(5);    
                                stockKeepingUnit.TimeStamp = reader.IsDBNull(6) ? null : reader.GetDateTime(6);
                                stockKeepingUnit.UserId =  reader.IsDBNull(7) ? null : reader.GetString(7) ;
                                stockKeepingUnit.IsActive = reader.IsDBNull(8) ? false : reader.GetBoolean(8);
                                
                                StockKeepingUnit?.Add(stockKeepingUnit);
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