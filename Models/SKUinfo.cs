using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WIndowApp.Models
{
    public class SKUinfo
    {
        public int? Id {get;set;}
        [Required]
        public string? Name {get;set;}
        [Required]
        public string? Code {get;set;}
        [Required]
        public decimal? UnitPrice {get;set;}
        public DateTime? DateCreated {get;set;}
        public string? CreatedBy {get;set;} = "";
        public DateTime? TimeStamp {get;set;}
        public string? UserId {get;set;} = "";
        public bool? IsActive {get;set;}
    }
}