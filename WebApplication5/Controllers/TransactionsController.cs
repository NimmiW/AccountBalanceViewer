using System;
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
using System.Threading.Tasks;

namespace WebApplication5.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class TransactionsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/Transactions/ExcelData")]
        public async Task<IHttpActionResult> ExcelData(ExcelDataBinding model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DateTime time = DateTime.Now;

            Transaction t1 = new Transaction();
            t1.Month = model.month;
            t1.Year = model.year;
            t1.AccountId = 1;
            t1.Amount = model.RandD;
            t1.EnteredDateTime = time;
 

            Transaction t2 = new Transaction();
            t2.Month = model.month;
            t2.Year = model.year;
            t2.AccountId = 2;
            t2.Amount = model.Canteen;
            t2.EnteredDateTime = time;

            Transaction t3 = new Transaction();
            t3.Month = model.month;
            t3.Year = model.year;
            t3.AccountId = 3;
            t3.Amount = model.CEOCar;
            t3.EnteredDateTime = time;

            Transaction t4 = new Transaction();
            t4.Month = model.month;
            t4.Year = model.year;
            t4.AccountId = 4;
            t4.Amount = model.Marketing;
            t4.EnteredDateTime = time;

            Transaction t5 = new Transaction();
            t5.Month = model.month;
            t5.Year = model.year;
            t5.AccountId = 5;
            t5.Amount = model.ParkingFines;
            t5.EnteredDateTime = time;

            Transaction[] transactionArray = { t1, t2, t3, t4, t5 };

            try
            {
                foreach (Transaction t in transactionArray)
                {
                    db.Transactions.Add(t);
                    db.SaveChanges();
                }
                return Ok("All the trasactions were added.");
            }
            catch (Exception e)
            {
                return BadRequest();
            }
           
        }

        // GET: api/Transactions
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [ResponseType(typeof(Balance[]))]
        public IHttpActionResult GetTransactions(int month, int year)
        {

            List<BusinessAccount> accounts = db.Accounts.ToList();

            List<Balance> balances = new List<Balance>();

            if (!(month >= 0 && month <= 11 && year > 0))
            {
                return NotFound();
            }

            List<Transaction> transactions = (from p in db.Transactions
                                              where (p.Year == year && p.Month == month)
                                              select p).ToList();

            foreach (Transaction transaction in transactions)
            {
                BusinessAccount[] accountList = accounts.Where(p => (p.Id == transaction.AccountId)).ToArray();

                if (accountList.Length != 0)
                {
                    BusinessAccount account = accountList[0];
                    Balance balance = new Balance();
                    balance.accountName = account.AccountDisplayName;
                    balance.accountId = account.Id;
                    balance.transactionId = transaction.Id;
                    balance.amount = transaction.Amount;
                    balance.enteredDateTime = transaction.EnteredDateTime;
                    balance.month = month;
                    balance.year = year;
                    balances.Add(balance);
                }
                
            }

            return Ok(balances);
        }

        // GET: api/Transactions/5
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [ResponseType(typeof(Transaction))]
        public IHttpActionResult GetTransaction(long id)
        {
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // PUT: api/Transactions/5
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTransaction(long id, Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transaction.Id)
            {
                return BadRequest();
            }

            db.Entry(transaction).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Transactions
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [ResponseType(typeof(Transaction))]
        public IHttpActionResult PostTransaction(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            transaction.EnteredDateTime = DateTime.Now;

            try
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();

                return CreatedAtRoute("DefaultApi", new { id = transaction.Id }, transaction);
            }
            catch (Exception e)
            {
                return BadRequest(ModelState);
            }

        }

        // DELETE: api/Transactions/5
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [ResponseType(typeof(Transaction))]
        public IHttpActionResult DeleteTransaction(long id)
        {
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return NotFound();
            }


            try
            {
                db.Transactions.Remove(transaction);
                db.SaveChanges();

                return CreatedAtRoute("DefaultApi", new { id = transaction.Id }, transaction);
            }
            catch (Exception e)
            {
                return BadRequest(ModelState);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionExists(long id)
        {
            return db.Transactions.Count(e => e.Id == id) > 0;
        }

    }
}