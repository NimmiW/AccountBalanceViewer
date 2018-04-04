using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication5.Models
{
    public class Transaction
    {

        public int Id { get; set; }
        public DateTime EnteredDateTime { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int AccountId { get; set; }
        public int Amount { get; set; }
    }

    public class ExcelDataBinding
    {
        public int month { get; set; }
        public int year { get; set; }
        public int RandD { get; set; }
        public int CEOCar { get; set; }
        public int Canteen { get; set; }
        public int Marketing { get; set; }
        public int ParkingFines { get; set; }
    }
}