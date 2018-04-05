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
    public class TestReportController
    {
        [TestMethod]
        public void TestPopulate()
        {
            var Controller = new ReportController();
            bool IsPass = true;

            double[] TestArray = new double[12];
            double TestValue = 1;
            Controller.Populate(TestArray, TestValue);

            for (int i = 0; i < TestArray.Length; i++)
            {
                if (TestArray[i] != TestValue)
                {
                    IsPass = false;
                    break;
                }
            }
            Assert.AreEqual(IsPass, true);
        }

        [TestMethod]
        public void TestGetReport()
        {
            var Controller = new ReportController();
            bool IsPass = true;

            IHttpActionResult actionResult = Controller.GetReport(2017);
            var ContentResult = actionResult as OkNegotiatedContentResult<Hashtable>;
            Hashtable Hash = ContentResult.Content;

            IsPass = (Hash.Count == 5);

            Assert.AreEqual(IsPass, true);
        }
    }
}
