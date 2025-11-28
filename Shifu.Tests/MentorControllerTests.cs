using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shifu.Controllers;
using Shifu.Models;
using Shifu.Services;
using Shifu.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Shifu.Tests
{
    [TestFixture]
    public class MentorControllerTests
    {
        private AppDbContext _db;
        private Mock<IHubContext<MentorUserHub>> _hubMock;
        private Mock<IClientProxy> _clientProxyMock;
        private MentorController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _db = new AppDbContext(options);

            _clientProxyMock = new Mock<IClientProxy>();
            var clientsMock = new Mock<IHubClients>();
            clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(_clientProxyMock.Object);

            _hubMock = new Mock<IHubContext<MentorUserHub>>();
            _hubMock.Setup(h => h.Clients).Returns(clientsMock.Object);

            _controller = new MentorController(_db, _hubMock.Object);
        }

        private void AddUserClaims(int id)
        {
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString())
            }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claims }
            };
        }

        [Test]
        public async Task AwardBadge_Should_AddBadge()
        {
            AddUserClaims(1);

            var dto = new MentorController.BadgeDto
            {
                UserId = 2,
                BadgeName = "Helper",
                ImageUrl = "/img/helper.png"
            };

            var result = await _controller.AwardBadge(dto);

            var badge = await _db.Badges.FirstOrDefaultAsync();
            NUnit.Framework.Assert.IsNotNull(badge);
            NUnit.Framework.Assert.AreEqual("Helper", badge.Name);
            NUnit.Framework.Assert.AreEqual("1", badge.AwardedById);
            NUnit.Framework.Assert.AreEqual("2", badge.AwardedToId);
            NUnit.Framework.Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public async Task Chat_Should_ReturnUsers()
        {
            AddUserClaims(1);

            var result = await _controller.Chat();
            NUnit.Framework.Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task MentorDashboard_Should_ReturnModel()
        {
            AddUserClaims(1);

            var result = await _controller.MentorDashboard();
            var viewResult = result as ViewResult;
            NUnit.Framework.Assert.IsNotNull(viewResult);
            NUnit.Framework.Assert.IsInstanceOf<MentorDashboardViewModel>(viewResult.Model);
        }

        [Test]
        public async Task LoadMessages_Should_ReturnJson()
        {
            var result = await _controller.LoadMessages(1);
            NUnit.Framework.Assert.IsInstanceOf<JsonResult>(result);
        }

        [Test]
        public async Task SendMessages_Should_ReturnOk_IfAssigned()
        {
            AddUserClaims(1);
            _db.UserMentorAssignments.Add(new UserMentorAssignment { UserId = 2, MentorId = 1 });
            await _db.SaveChangesAsync();

            var result = await _controller.SendMessages(2, "Hi");
            NUnit.Framework.Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public async Task SendMessages_Should_Forbidden_IfNotAssigned()
        {
            AddUserClaims(1);

            var result = await _controller.SendMessages(999, "Hi");
            NUnit.Framework.Assert.IsInstanceOf<ForbidResult>(result);
        }
    }
}
