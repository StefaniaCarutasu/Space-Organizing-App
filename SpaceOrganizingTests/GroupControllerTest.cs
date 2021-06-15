using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpaceOrganizing.Controllers;
using SpaceOrganizing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SpaceOrganizingTests
{
    public class SutController : Controller
    {
        public string Get()
        {
            return User.Identity.Name;
        }
    }

    public class TestableControllerContext : ControllerContext
    {
        public TestableHttpContext TestableHttpContext { get; set; }
    }

    public class TestableHttpContext : HttpContextBase
    {
        public override IPrincipal User { get; set; }
    }

    [TestClass]
    public class GroupControllerTest
    {

        [TestMethod]
        public void AccessShowAuthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsTrue(AuthorizationTest.IsAuthorized(
                controller,
                "Show",
                new Type[] { typeof(Int32) },
                new string[] { "Administrator", "User" }));
        }

        [TestMethod]
        public void AccessShowUnauthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsFalse(AuthorizationTest.IsAuthorized(
                controller,
                "Show",
                new Type[] { typeof(Int32) },
                new string[] { }));
        }

        /* _____________________________________________________________________ */

        [TestMethod]
        public void AccessNewAuthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsTrue(AuthorizationTest.IsAuthorized(
                controller,
                "New",
                null,
                new string[] { "Administrator", "User" }));
        }

        [TestMethod]
        public void AccessNewUnauthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsFalse(AuthorizationTest.IsAuthorized(
                controller,
                "New",
                null,
                new string[] { }));
        }

        [TestMethod]
        public void AccessNewPostAuthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsTrue(AuthorizationTest.IsAuthorized(
                controller,
                "New",
                new Type[] { typeof(Group) },
                new string[] { "Administrator", "User" }));
        }

        [TestMethod]
        public void AccessNewPostUnauthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsFalse(AuthorizationTest.IsAuthorized(
                controller,
                "New",
                new Type[] { typeof(Group) },
                new string[] { }));
        }

        /* _____________________________________________________________________ */

        [TestMethod]
        public void AccessEditAuthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsTrue(AuthorizationTest.IsAuthorized(
                controller,
                "Edit",
                new Type[] { typeof(Int32) },
                new string[] { "Administrator", "User" }));
        }

        [TestMethod]
        public void AccessEditUnauthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsFalse(AuthorizationTest.IsAuthorized(
                controller,
                "Edit",
                new Type[] { typeof(Int32) },
                new string[] { }));
        }

        [TestMethod]
        public void AccessEditPutAuthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsTrue(AuthorizationTest.IsAuthorized(
                controller,
                "Edit",
                new Type[] { typeof(Int32), typeof(Group) },
                new string[] { "Administrator", "User" }));
        }

        [TestMethod]
        public void AccessEditPutUnauthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsFalse(AuthorizationTest.IsAuthorized(
                controller,
                "Edit",
                new Type[] { typeof(Int32), typeof(Group) },
                new string[] { }));
        }

        /* _____________________________________________________________________ */

        [TestMethod]
        public void AccessDeleteAuthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsTrue(AuthorizationTest.IsAuthorized(
                controller,
                "Delete",
                new Type[] { typeof(Int32) },
                new string[] { "Administrator", "User" }));
        }

        [TestMethod]
        public void AccessDeleteUnauthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsFalse(AuthorizationTest.IsAuthorized(
                controller,
                "Delete",
                new Type[] { typeof(Int32) },
                new string[] { }));
        }

        /* _____________________________________________________________________ */

        [TestMethod]
        public void AccessNewMemberAuthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsTrue(AuthorizationTest.IsAuthorized(
                controller,
                "NewMember",
                new Type[] { typeof(Int32), typeof(string) },
                new string[] { "Administrator", "User" }));
        }

        [TestMethod]
        public void AccessNewMemberUnauthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsFalse(AuthorizationTest.IsAuthorized(
                controller,
                "NewMember",
                new Type[] { typeof(Int32), typeof(string) },
                new string[] { }));
        }

        /* _____________________________________________________________________ */

        [TestMethod]
        public void AccessLeaveGroupAuthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsTrue(AuthorizationTest.IsAuthorized(
                controller,
                "LeaveGroup",
                new Type[] { typeof(Int32) },
                new string[] { "Administrator", "User" }));
        }

        [TestMethod]
        public void AccessLeaveGroupUnauthorized()
        {
            Controller controller = new GroupsController();
            Assert.IsFalse(AuthorizationTest.IsAuthorized(
                controller,
                "LeaveGroup",
                new Type[] { typeof(Int32) },
                new string[] { }));
        }

        /* _____________________________________________________________________ */

        /*[TestMethod]
        public void TestDetailsRedirect()
        {
            var controller = new GroupsController();
            var result = (RedirectToRouteResult)controller.NewMember(2);
            Assert.AreEqual("/Groups/Show/2", result.RouteValues["action"]);

        }*/

        /*[TestMethod]
        public void AccessShowViewAuthorized()
        {
            var groupController = new GroupsController();

            Mock<IPrincipal> userMock = new Mock<IPrincipal>();
            userMock.Setup(p => p.IsInRole("Administrator")).Returns(true);

            var contextMock = new Mock<HttpContextBase>();
            contextMock.SetupGet(ctx => ctx.User).Returns(userMock.Object);

            Mock<ControllerContext> controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.SetupGet(con => con.HttpContext).Returns(contextMock.Object);

            groupController.ControllerContext = controllerContextMock.Object;
            var result = groupController.Index();
            userMock.Verify(p => p.IsInRole("Administrator"));
            Console.WriteLine(((ViewResult)result).ViewName);
            Assert.AreEqual(((ViewResult)result).ViewName, "Index");

        }*/

        /*[TestMethod]
        public void AccessShowViewAuthorized1()
        {
            Mock<IPrincipal> mockP = new Mock<IPrincipal>();
            mockP.SetupGet(p => p.Identity.Name).Returns("UnitTesting");
            mockP.Setup(p => p.IsInRole("Administrator")).Returns(false); //"Role" is not the actual role name.

            Mock<ControllerContext> mockC = new Mock<ControllerContext>();
            mockC.SetupGet(p => p.HttpContext.User).Returns(mockP.Object);
            mockC.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);

            GroupsController target = new GroupsController();
            target.ControllerContext = mockC.Object;

            // Act
            ViewResult result = target.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }*/
    }
}
