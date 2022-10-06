using System.Threading;
using System.Threading.Tasks;
using Bogus;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Repositories;
using DevUp.Domain.Organization.Services;
using DevUp.Domain.Organization.Services.Exceptions;
using DevUp.Domain.Organization.ValueObjects;
using Moq;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Organization.Services
{
    public class TeamServiceTests
    {
        private readonly Faker<TeamId> _idFaker;
        private readonly Faker<TeamName> _nameFaker;
        private readonly Faker<Team> _teamFaker;

        private Mock<ITeamRepository> _teamRepositoryMock;
        private ITeamService _teamService;

        public TeamServiceTests()
        {
            _idFaker = new Faker<TeamId>().CustomInstantiator(f => new TeamId(f.Random.Guid()));
            _nameFaker = new Faker<TeamName>().CustomInstantiator(f => new TeamName(f.Company.CompanyName()));
            _teamFaker = new Faker<Team>().CustomInstantiator(_ => new Team(_idFaker.Generate(), _nameFaker.Generate()));
        }

        [SetUp]
        public void SetUp()
        {
            _teamRepositoryMock = new Mock<ITeamRepository>();
            _teamService = new TeamService(_teamRepositoryMock.Object);
        }

        [Test]
        public void CreateAsync_WhenGivenDuplicateId_ThrowsTeamIdTakenException()
        {
            var id = _idFaker.Generate();
            var name = _nameFaker.Generate();
            var otherTeam = new Team(id, _nameFaker.Generate());

            _teamRepositoryMock.Setup(tr => tr.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(otherTeam);
            _teamRepositoryMock.Setup(tr => tr.GetByNameAsync(name, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team?)null);

            Assert.ThrowsAsync<TeamIdTakenException>(() => _teamService.CreateAsync(id, name, CancellationToken.None));
        }

        [Test]
        public void CreateAsync_WhenGivenDuplicateName_ThrowsTeamNameTakenException()
        {
            var id = _idFaker.Generate();
            var name = _nameFaker.Generate();
            var otherTeam = new Team(_idFaker.Generate(), name);

            _teamRepositoryMock.Setup(tr => tr.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team?)null);
            _teamRepositoryMock.Setup(tr => tr.GetByNameAsync(name, It.IsAny<CancellationToken>()))
                .ReturnsAsync(otherTeam);

            Assert.ThrowsAsync<TeamNameTakenException>(() => _teamService.CreateAsync(id, name, CancellationToken.None));
        }

        [Test]
        public async Task CreateAsync_WhenGivenUniqueIdAndName_AddsTeamToRepository()
        {
            var id = _idFaker.Generate();
            var name = _nameFaker.Generate();

            _teamRepositoryMock.Setup(tr => tr.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team?)null);
            _teamRepositoryMock.Setup(tr => tr.GetByNameAsync(name, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team?)null);

            await _teamService.CreateAsync(id, name, CancellationToken.None);

            var team = new Team(id, name);
            _teamRepositoryMock.Verify(tr => tr.CreateAsync(team, It.IsAny<CancellationToken>()));
        }

        [Test]
        public void GetAsync_WhenIdWasNotFoundInTheRepository_ThrowsTeamIdNotFoundException()
        {
            var id = _idFaker.Generate();

            _teamRepositoryMock.Setup(tr => tr.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team?)null);

            Assert.ThrowsAsync<TeamIdNotFoundException>(() => _teamService.GetAsync(id, CancellationToken.None));
        }

        [Test]
        public async Task GetAsync_WhenIdWasFoundInTheRepository_ReturnsTheTeam()
        {
            var id = _idFaker.Generate();
            var team = new Team(id, _nameFaker.Generate());

            _teamRepositoryMock.Setup(tr => tr.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            var result = await _teamService.GetAsync(id, CancellationToken.None);
            Assert.AreEqual(team, result);
        }

        [Test]
        public void DeleteAsync_WhenIdWasNotFoundInTheRepository_ThrowsTeamIdNotFoundException()
        {
            var id = _idFaker.Generate();

            _teamRepositoryMock.Setup(tr => tr.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team?)null);

            Assert.ThrowsAsync<TeamIdNotFoundException>(() => _teamService.DeleteAsync(id, CancellationToken.None));
        }

        [Test]
        public async Task DeleteAsync_WhenIdWasFoundInTheRepository_DeletesTheTeam()
        {
            var id = _idFaker.Generate();
            var team = new Team(id, _nameFaker.Generate());

            _teamRepositoryMock.Setup(tr => tr.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            await _teamService.DeleteAsync(id, CancellationToken.None);

            _teamRepositoryMock.Verify(tr => tr.DeleteAsync(team, It.IsAny<CancellationToken>()));
        }

    }
}
