using System;
using System.Collections;
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
    public class TestBusinessAccountController
    {
        [TestMethod]
        public void TestGetBusinessAccounts()
        {
            var Controller = new BusinessAccountsController();
            bool IsPass = true;

            IQueryable<BusinessAccount> actionResult = Controller.GetAccounts();
            BusinessAccount[] array = actionResult.ToArray();

            IsPass = (array.Length == 5);

            Assert.AreEqual(IsPass, true);

        }
        
    }
}
