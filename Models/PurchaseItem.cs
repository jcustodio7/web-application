using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WIndowApp.Models
{
    public class PurchaseItem
    {
        public int? Id {get;set;}
        [Required]
        public string? ItemName {get;set;}
        [Required]
        public int? PurchaseOrderID {get;set;}
        [Required]
        public int? SkuID {get;set;}
        [Required]
        public int? Quantity {get;set;}
        public decimal? Price {get;set;}
        public DateTime? Timestamp {get;set;}
        public string? UserId {get;set;} = "";
    }
}