using NUnit.Framework;
using foodies_api;
using foodies_api.Services;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.Logging;
using foodies_api.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Newtonsoft.Json;
using foodies_api.Interfaces.Repositories;
using foodies_api.Models.Dtos.Responses;
using foodies_api.Models.Dtos.Requests;

namespace foodies_api.tests;

[TestFixture]
public class UsersTests
{
    private List<User> _users;
    private ILogger<UsersService> _mockLogger;
    private Mock<IUsersRepository> _mockUsersRepository;


    [SetUp]
    public void Setup()
    {
        CreateUsers();

        _mockUsersRepository = new Mock<IUsersRepository>();

        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        var factory = serviceProvider.GetService<ILoggerFactory>();
        _mockLogger = factory.CreateLogger<UsersService>();

    }

    public void MockUsersRepository(List<User> users, bool success, Guid id = new Guid())
    {
        // Setup mock UsersRepository methods if needed
        _mockUsersRepository.Setup(repo => repo.GetUsers()).Returns(Task.FromResult(new RepositoryResponse<IEnumerable<User>>()
        { Success = success, Data = users, Exception = null }));
        _mockUsersRepository.Setup(repo => repo.GetUserById(It.IsAny<Guid>())).Returns(Task.FromResult(new RepositoryResponse<User>()
        { Success = success, Data = users[0], Exception = null }));
        _mockUsersRepository.Setup(repo => repo.DeleteUser(It.IsAny<Guid>())).Returns(Task.FromResult(new RepositoryResponse<User>()
        { Success = success, Data = users[0], Exception = null }));
        _mockUsersRepository.Setup(repo => repo.UpdateUser(It.IsAny<Guid>(), It.IsAny<UserUpdateRequest>())).Returns(Task.FromResult(new RepositoryResponse<User>()
        { Success = success, Data = users[0], Exception = null }));
    }

    /// <summary>
    /// Creates a list of Users
    /// </summary>
    private void CreateUsers()
    {
        _users = new List<User>();
        var guids = CreateGuids(3);
        
        for (var i = 0; i < 3; i++)
            _users.Add(new()
            {
                Id = guids[i],
                FirstName = "business #" + (1 + i).ToString(),
                LastName = "SomeLastName",
                Username = $"username-{i.ToString()}",
                Email = "test@email.com"
            });  
    }

    private static List<Guid> CreateGuids(int num)
    {
        var guids = new List<Guid>();
        
        for(int i = 0; i < num; i++)
        {
            guids.Add(new Guid());
        }
        return guids;
    }

    /// <summary>
    /// Creates a UsersService instance
    /// </summary>
    private UsersService CreateUsersService()
    {
        var service = new UsersService(_mockLogger, _mockUsersRepository.Object);
        return service;
    }

    [Test]
    public void GetUserById_Valid()
    {   
        var usersService = CreateUsersService();
        List<Guid> guids = CreateGuids(1);
        MockUsersRepository(_users, true);
        var result = usersService.GetUserById(guids[0]);

        Assert.That(guids[0], Is.EqualTo(result.Result.Data.Id));
    }

    [Test]
    public void GetUsers_Valid()
    {   
        var usersService = CreateUsersService();
        MockUsersRepository(_users, true);
        
        var result = usersService.GetUsers();

        Assert.That(result.Result.IsSuccess);
        Assert.That(result.Result.Data.Count(), Is.EqualTo(3));
    }

        [Test]
        public void Update_Valid()
        {
            var guid = _users[0].Id;
            var userUpdateRequest = new UserUpdateRequest() { Email = "changed@email.com" };
            _users[0].Email = userUpdateRequest.Email;
            var usersService = CreateUsersService();
            MockUsersRepository(_users, true);

            var result = usersService.UpdateUser(guid, userUpdateRequest);

            Assert.That(result.Result.IsSuccess);
            Assert.That(result.Result.Data.Email, Is.EqualTo("changed@email.com"));
        }
}
