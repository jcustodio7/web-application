using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using WIndowApp.Models;

namespace WIndowApp.Pages.OrderLists.PurchaseItemFolder
{
    public class CreatePurchaseItem : PageModel
    {
        [BindProperty]
        public PurchaseItem PurchaseItem {get;set;} = new PurchaseItem();
        public List<SKUinfo> StockKeepingUnitLists {get;set;} = new List<SKUinfo>();
        public string ErrorMessage { get; private set; }
        public string? idQuery {get;set;}
        public int? StockKeepingUNitId {get;set;}
        [BindProperty]
        public int PurchaseOrderId {get;set;}
        public decimal? Total {get;set;}

        public void OnGet(int purchaseOrderId)
        {
            StockKeepingUnitLists = GetSKULists();
            PurchaseOrderId = purchaseOrderId;
        }

        public void OnPostSubmit()
        {
            try {
                StockKeepingUnitLists = GetSKULists();
                decimal? unitprice = StockKeepingUnitLists.FirstOrDefault(x => x.Name == PurchaseItem.ItemName).UnitPrice;
                Total = PurchaseItem.Quantity * unitprice;
                StockKeepingUNitId = StockKeepingUnitLists.FirstOrDefault(x => x.Name == PurchaseItem.ItemName).Id;
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();
                    string sqlQuery = "INSERT INTO PurchaseItems " + "(purchaseOrderid, skuid, quantity, price, timestamp, userid, itemname) " + 
                                        "VALUES (@purchaseorderid, @skuid, @quantity, @price, @timestamp, @userid, @itemname);";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection)){
                        command.Parameters.AddWithValue("@purchaseorderid", PurchaseOrderId);
                        command.Parameters.AddWithValue("@skuid", StockKeepingUNitId);
                        command.Parameters.AddWithValue("@quantity", PurchaseItem.Quantity);
                        command.Parameters.AddWithValue("@price", Total);
                        command.Parameters.AddWithValue("@timestamp", DateTime.Now);
                        command.Parameters.AddWithValue("@userid", PurchaseItem.UserId);
                        command.Parameters.AddWithValue("@itemname", PurchaseItem.ItemName);

                        command.ExecuteNonQuery();
                    }
                }

            } catch  (Exception e){
                ErrorMessage = e.Message;
                @ViewData["ErrorMessage"] = ErrorMessage;
                return;
            }
            Response.Redirect($"/OrderLists/EditOrder?id={PurchaseOrderId}");    
        }

           public List<SKUinfo> GetSKULists(){
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
                                
                                StockKeepingUnitLists?.Add(stockKeepingUnit);
                            }    
                        }
                    }
                }

                return StockKeepingUnitLists;
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }
}

