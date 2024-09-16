using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace WIndowApp.Pages.OrderLists.PurchaseItemFolder
{
    public class DeletePurchaseItem : PageModel
    {
        private readonly ILogger<DeletePurchaseItem> _logger;

        public DeletePurchaseItem(ILogger<DeletePurchaseItem> logger)
        {
            _logger = logger;
        }
        public int id {get;set;}
        public string purchaseOrderId {get;set;}

        public void OnGet()
        {
            purchaseOrderId= Request.Query["purchaseorderid"].ToString();
            id = int.Parse(Request.Query["id"]);
        }

        public void OnPostDelete(int id, string purchaseorderid){
            //purchaseOrderId= Request.Query["purchaseorderid"];
            // id = Request.Query["id"];
            purchaseOrderId = Request.Query["purchaseorderid"].ToString();
            try
            {
                string connectionString = "Server=.;Database=crmdb;Trusted_Connection=True;TrustServerCertificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString)){
                    connection.Open();
                    
                    string sqlQuery = "DELETE FROM PurchaseItems WHERE id=@id";
                    using(SqlCommand command = new SqlCommand(sqlQuery, connection)){
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (System.Exception)
            {
                return;
            }

            Response.Redirect($"/OrderLists/EditOrder?id={purchaseorderid}");
        }
    }
}