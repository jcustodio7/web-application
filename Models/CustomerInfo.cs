using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WIndowApp.Models
{
    public class CustomerInfo
    {
        public int? Id {get;set;}
        [Required]
        public string? FirstName {get; set;}

        [Required]
        public string? LastName {get;set;}

        public string? FullName {get;set;}

        [Required]
        [MaxLength(10), MinLength(10)]
        public string? MobileNumber {get;set;}

        [Required]
        public string? City {get;set;}

        public DateTime? DateCreated {get;set;}

        [Required]
        public string? CreatedBy {get;set;}

        public DateTime? TimeStamp {get;set;}

        [Required]
        public string? UserId {get;set;}

        public bool isActive {get;set;}
    }
}