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
    [Authorize(Roles ="USER")]
    public class BalanceController : ApiController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        private void Populate<T>(T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        // GET: api/Balance/5
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [ResponseType(typeof(List<Balance>))]
        public IHttpActionResult GetBalance(int month, int year)
        {
            if (!(month >= 0 && month <= 11 && year > 0))
            {
                return NotFound();
            }

            List<BusinessAccount> accounts = db.Accounts.ToList();

            List<Balance> balances = new List<Balance>();

            if (!(month >= 0 && month <= 11 && year > 0))
            {
                return NotFound();
            }

            List<Transaction> transactions = (from p in db.Transactions
                                              where (p.Year == year && p.Month == month)
                                              select p).ToList();

            foreach (BusinessAccount account in accounts)
            {
                Transaction[] transactionList = transactions.Where(p => (p.AccountId == account.Id)).ToArray();
                Transaction transaction;
                Balance balance = new Balance();
                if (transactionList.Length > 0)
                {
                    transaction = transactionList[0];
                    balance.accountName = account.AccountDisplayName;
                    balance.amount = transaction.Amount;
                    balance.month = month;

                }
                else
                {
                    balance.accountName = account.AccountDisplayName;
                    balance.amount = 0;
                    balance.month = month;
                }

                balances.Add(balance);

            }




            return Ok(balances);
        }


    }
}
