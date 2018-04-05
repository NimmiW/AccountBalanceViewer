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
using System.Web.Http.Cors;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    public class BusinessAccountsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Accounts
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IQueryable<BusinessAccount> GetAccounts()
        {

            return db.Accounts;       
        }
    }
}