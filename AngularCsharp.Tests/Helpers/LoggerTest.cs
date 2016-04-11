﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AngularCSharp.Helpers.Tests.Helpers
{
    [TestClass]
    public class LoggerTests
    {
        #region Helpers_Logger_Construct

        [TestMethod]
        public void Helpers_Logger_Construct()
        {
            // Act
            var sut = new Logger();

            // Assert
            Assert.IsFalse(sut.HasWarnings);
            Assert.AreEqual(0, sut.Warnings.Length);
        }

        #endregion

        #region Helpers_Logger_AddWarning

        [TestMethod]
        public void Helpers_Logger_AddWarning()
        {
            // Assign
            var sut = new Logger();
            var message = "Error";

            // Act
            sut.AddWarning(message);

            // Assert
            Assert.AreEqual(1, sut.Warnings.Length);
            Assert.AreEqual(message, sut.Warnings[0]);
            Assert.IsTrue(sut.HasWarnings);
        }

        #endregion
    }
}