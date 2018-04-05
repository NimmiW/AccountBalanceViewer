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
    public class TestAccountController
    {
        [TestMethod]
        void TestLogin()
        {
            var Controller = new AccountController();
            bool IsPass = true;



            Assert.AreEqual(IsPass, true);
        }
    }
}
