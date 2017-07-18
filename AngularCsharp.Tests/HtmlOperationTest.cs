using AngularCsharp.Tests.Properties;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AngularCsharp.Tests
{
    [TestClass]
    public class HtmlOperationTest
    {
        #region GetNgFors

        [TestMethod]
        public void HtmlOperation_TemplateGetNgFors_NotFound()
        {
            // Act
            HtmlNodeCollection result = HtmlOperation.GetNodesNgFor(Resources.simple1_html);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void HtmlOperation_TemplateGetNgFors_Found()
        {
            // Act
            HtmlNodeCollection result = HtmlOperation.GetNodesNgFor(Resources.usecase1_html);

            // Assert
            Assert.IsNotNull(result, "result must not be null.");
            Assert.AreEqual<int>(2, result.Count, "result.Count must be 1.");
        }

        #endregion
    }
}
