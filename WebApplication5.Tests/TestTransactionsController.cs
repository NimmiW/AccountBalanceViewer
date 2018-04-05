using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication5.Controllers;
using WebApplication5.Models;

namespace WebApplication5.Tests
{
    [TestClass]
    public class TestTransactionsController
    {
        [TestMethod]
        public void TestGetTransactions()
        {
            var Controller = new TransactionsController();
            bool IsPass = true;

            int TestMonth = 0;
            int TestYear = 2017;
            IHttpActionResult actionResult = Controller.GetTransactions(TestMonth, TestYear);
            var ContentResult = actionResult as OkNegotiatedContentResult<Balance[]>;
            Balance[] Balances = ContentResult.Content;

            for (int i = 0; i < Balances.Length; i++)
            {
                if(Balances[i].month != TestMonth)
                {
                    IsPass = false;
                    break;
                }
            }
            Assert.AreEqual(IsPass, true);
        }
    }
}
