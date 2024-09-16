using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WIndowApp.Models
{
    public class OrderInfo
    {
        public int Id {get;set;}
        [Required]
        public string? CustomerName {get;set;}
        [Required]
        public string? DeliveryDate {get;set;}
        [Required]
        public string? OrderStatus {get;set;}
        public decimal? AmountDue {get;set;}
        public DateTime? DateCreated {get;set;}
        public string? CreatedBy {get;set;}
        public DateTime? TimeStamp {get;set;}
        public string? UserId {get;set;}
        public bool? IsActive {get;set;}
    }
}