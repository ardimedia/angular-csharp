using System;
using AngularCsharp.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AngularCsharp.Tests
{
    [TestClass]
    public class NgForOperationTest
    {
        #region FixHtml

        [TestMethod]
        public void NgForOperation_FixHtml()
        {
            // Act
            string result = NgForOperation.FixHtml(" *ngFor=");

            // Assert
            Assert.AreEqual<string>(" ngfor=", result, "result must match [ ngfor=].");
        }

        #endregion

        #region GetParameters

        [TestMethod]
        public void NgForOperation_GetParameters()
        {
            // Act
            string result = NgForOperation.GetParameters(Resources.usecase1_html);

            // Assert
            Assert.AreEqual<string>("#printApproval of customerPrintApprovals", result, "result must match [#printApproval of customerPrintApprovals].");
        }

        #endregion

        #region GetParameterCollectionName

        [TestMethod]
        public void NgForOperation_GetParameterCollectionName_1()
        {
            // Act
            string result = NgForOperation.GetParameterCollectionName(Resources.usecase1_html);

            // Assert
            Assert.AreEqual<string>("customerPrintApprovals", result, "result must match [customerPrintApprovals].");
        }

        [TestMethod]
        public void NgForOperation_GetParameterCollectionName_2()
        {
            // Assign
            string html = Resources.usecase1_html;
            html = html.Replace("#printApproval of customerPrintApprovals", "#printApproval of customerPrintApprovals | xxx");

            // Act
            string result = NgForOperation.GetParameterCollectionName(html);

            // Assert
            Assert.AreEqual<string>("customerPrintApprovals", result, "result must match [customerPrintApprovals].");
        }


        [TestMethod]
        public void NgForOperation_GetParameterCollectionName_3()
        {
            try
            {
                // Assign
                string html = Resources.usecase1_html;
                html = html.Replace("#printApproval of customerPrintApprovals", "customerPrintApprovals");

                // Act
                string result = NgForOperation.GetParameterCollectionName(html);

                // Assert
                Assert.Fail("An exception should have been thrown.");
            }
            catch (Exception ex)
            {
                // Assert
                Assert.AreEqual<string>("Could not parse *ngFor parameters (of not found): customerPrintApprovals", ex.Message);
            }
        }

        #endregion

        #region GetParameterCollectionName

        [TestMethod]
        public void NgForOperation_GetParameterItemName_1()
        {
            // Act
            string result = NgForOperation.GetParameterItemName(Resources.usecase1_html);

            // Assert
            Assert.AreEqual<string>("printApproval", result, "result must match [printApproval].");
        }


        [TestMethod]
        public void NgForOperation_GetParameterItemName_3()
        {
            try
            {
                // Assign
                string html = Resources.usecase1_html;
                html = html.Replace("#printApproval of customerPrintApprovals", "customerPrintApprovals");

                // Act
                string result = NgForOperation.GetParameterItemName(html);

                // Assert
                Assert.Fail("An exception should have been thrown.");
            }
            catch (Exception ex)
            {
                // Assert
                Assert.AreEqual<string>("Could not parse *ngFor parameters (of not found): customerPrintApprovals", ex.Message);
            }
        }

        #endregion
    }
}
