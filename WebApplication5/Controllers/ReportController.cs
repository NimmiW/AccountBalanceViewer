using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplication5.Models;
using System.Web.Http.Cors;

namespace WebApplication5.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class ReportController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private string[] accountNames = new string[] { "RandD", "Canteen", "CEOCar", "Marketing", "ParkingFines" };


        private void Populate<T>(T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        // GET: api/Report/5
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [ResponseType(typeof(Hashtable))]
        public IHttpActionResult GetReport(long year)
        {
            BusinessAccount[] accounts = db.Accounts.ToArray();

            if (year < 0)
            {
                return NotFound();
            }

            Hashtable report = new Hashtable();

            foreach (BusinessAccount account in accounts)
            {
                string accountName = account.AccountName;
                long accountId = account.Id;

                var data = from p in db.Transactions
                           where (p.Year == year && p.AccountId == accountId)
                           select new
                           {
                               Month = p.Month,
                               Amount = p.Amount
                           };

                double[] arr = new double[12];
                Populate(arr, 0);
                foreach (var item in data)
                {
                    arr[item.Month] = item.Amount;
                }
                report.Add(accountName, arr);
            }

            return Ok(report);
        }


    }
}
