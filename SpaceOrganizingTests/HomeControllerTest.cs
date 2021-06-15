using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpaceOrganizing.Controllers;
using SpaceOrganizing.Models;

namespace SpaceOrganizingTests
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();
            Assert.IsTrue(AuthorizationTest.IsAnonymous(
                controller,
                "Index",
                null));

            // Act
            // ViewResult result = controller.Index() as ViewResult;
            // Assert
            // Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();
            Assert.IsTrue(AuthorizationTest.IsAnonymous(
                controller,
                "About",
                null));

            // Act
            // ViewResult result = controller.About() as ViewResult;
            // Assert
            // Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();
            Assert.IsTrue(AuthorizationTest.IsAnonymous(
                controller,
                "Contact",
                null));

            // Act
            // ViewResult result = controller.Contact() as ViewResult;
            // Assert
            // Assert.IsNotNull(result);
        }

        [TestMethod]
        public void IndexMoq()
        {
            var identity = new GenericIdentity("tugberk");
            var controller = new HomeController();

            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Administrator")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("tugberk");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;

            //Assert.AreEqual(controller.Get(), identity.Name);
        }
    }
}
