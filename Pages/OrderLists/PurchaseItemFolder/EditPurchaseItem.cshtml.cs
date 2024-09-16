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
    public class EditPurchaseItem : PageModel
    {
        [BindProperty]
        public PurchaseItem editPurchase {get;set;} = new PurchaseItem();
        
        public List<SKUinfo> StockKeepingUnitLists {get;set;} = new List<SKUinfo>();
        public int? StockKeepingUNitId {get;set;}
        public string ErrorMessage { get; private set; }

        public decimal? Total { get;set; }

        public void OnGet(int id)
        {
             try
            {
                StockKeepingUnitLists = GetSKULists();
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();
                    string sqlQuery = "SELECT * FROM PurchaseItems WHERE purchaseorderid=@id";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection)){
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader()){
                            if(reader.Read()){
                                editPurchase.Id = reader.GetInt32(0);
                                editPurchase.PurchaseOrderID = reader.IsDBNull(1) ? null : reader.GetInt32(1);
                                editPurchase.SkuID = reader.IsDBNull(2) ? null : reader.GetInt32(2);
                                editPurchase.Quantity = reader.IsDBNull(3) ? null : reader.GetInt32(3);
                                editPurchase.Price = reader.IsDBNull(4) ? null : reader.GetDecimal(4); 
                                editPurchase.Timestamp = reader.IsDBNull(5) ? null : reader.GetDateTime(5); 
                                editPurchase.UserId =  reader.IsDBNull(6) ? null : reader.GetString(6) ;
                                editPurchase.ItemName =  reader.IsDBNull(7) ? null : reader.GetString(7);
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
            StockKeepingUnitLists = GetSKULists();
            StockKeepingUNitId = StockKeepingUnitLists.FirstOrDefault(x => x.Name == editPurchase.ItemName).Id;
            decimal? unitprice = StockKeepingUnitLists.FirstOrDefault(x => x.Name == editPurchase.ItemName).UnitPrice;
            Total = unitprice * editPurchase.Quantity;
            try {
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();
                    string sqlQuery = "UPDATE PurchaseItems SET purchaseorderid=@purchaseorderid, skuid=@skuid, quantity=@quantity, price=@price, timestamp=@timestamp, itemname=@itemname WHERE id=@id;";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection)){
                        command.Parameters.AddWithValue("@id", editPurchase.Id);
                        command.Parameters.AddWithValue("@purchaseorderid", editPurchase.PurchaseOrderID);
                        command.Parameters.AddWithValue("@skuid", StockKeepingUNitId);
                        command.Parameters.AddWithValue("@quantity", editPurchase.Quantity);
                        command.Parameters.AddWithValue("@price", Total);
                        command.Parameters.AddWithValue("@timestamp", DateTime.Now);
                        command.Parameters.AddWithValue("@itemname", editPurchase.ItemName);

                        command.ExecuteNonQuery();
                    }
                }

            } catch  (Exception e){
                ErrorMessage = e.Message;
                @ViewData["ErrorMessage"] = ErrorMessage;
                return;
            }
            Response.Redirect($"/OrderLists/EditOrder?id={editPurchase.PurchaseOrderID}");    
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