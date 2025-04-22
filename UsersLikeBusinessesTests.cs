using foodies_api.Services;
using Moq;
using Microsoft.Extensions.Logging;
using foodies_api.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using foodies_api.Interfaces.Repositories;
using AutoMapper;
using foodies_api.Repositories;
using Microsoft.EntityFrameworkCore;
using foodies_api.Data;

namespace foodies_api.tests;

[TestFixture]
public class UserLikeBusinessesTests 
{
    private List<UserLikeBusiness> _userLikes;
    private ILogger<UserLikeBusiness>? _mockLogger;
    private Mock<IMapper> _mapper;
    private AppDbContext? _context;

    [SetUp]
    public void Setup()
    {
        CreateUserLikes();
        CreateContext();        

        _mapper = new Mock<IMapper>();
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        var factory = serviceProvider.GetService<ILoggerFactory>();
        if (factory != null)
            _mockLogger = factory.CreateLogger<UserLikeBusiness>();

    }

    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDb")
        .Options;
        var context = new AppDbContext(options);

        foreach(var userLike in _userLikes)
            context.UserLikeBusinesses.Add(userLike);

        context.SaveChanges();
  
        return context; 
    }
    
    /// <summary>
    /// Creates a list of Users
    /// </summary>
    private void CreateUserLikes()
    {
        _userLikes = new List<UserLikeBusiness>();
        var ids = CreateIDs(3);
        var guids = CreateGuids(3);

        for (var i = 0; i < 3; i++)
            _userLikes.Add(new()
            {
                Id = ids[i],
                BusinessName = $"Business Name #{i}",
                BusinessId = $"Business ID: {i}",
                UserId = guids[i],
                Username = $"username #{i}"
            });
    }

    private static List<int> CreateIDs(int num)
    {
        var ids = new List<int>();

        for (int i = 0; i < num; i++)
        {
            ids.Add(new int());
        }
        return ids;
    }

    private static List<Guid> CreateGuids(int num)
    {
        var guids = new List<Guid>();

        for (int i = 0; i < num; i++)
        {
            guids.Add(Guid.NewGuid());
        }
        return guids;
    }

    /// <summary>
    /// Creates a UsersService instance
    /// </summary>
    private UsersLikeBusinessesRepository CreateUserLikeBusinessRepository()
    {
        var repository = new UsersLikeBusinessesRepository(_context, _mapper.Object, _mockLogger); //TODO: setup mapper & logger to not be class members of each Test classes
        return repository;
    }

    [Test]
    public void AddUserLikes_Valid()
    {
        var userLikeRepository = CreateUserLikeBusinessRepository();

        var result = userLikeRepository.AddUserLikes(_userLikes[0]).Result;

        Assert.That(result.Data.BusinessName, Is.EqualTo("Business Name #1"));
        Assert.That(result.Data.Id, Is.EqualTo(1));
    }

    [Test]
    public void GetAllUsers_Valid()
    {
    //    var usersService = CreateUsersService();
        //MockUsersRepository(_users, true);

        //var result = usersService.GetAllUsers();

        //Assert.That(result.Result.IsSuccess);
        //Assert.That(result.Result.Data.Count(), Is.EqualTo(3));
    }
}
