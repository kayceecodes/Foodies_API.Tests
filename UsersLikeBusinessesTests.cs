using Moq;
using Microsoft.Extensions.Logging;
using foodies_api.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using foodies_api.Repositories;
using Microsoft.EntityFrameworkCore;
using foodies_api.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
        _userLikes =
            UserLikeBusinessTestData.UserLikeBusinesses as List<UserLikeBusiness> ?? [];
        CreateContext();        

        _mapper = new Mock<IMapper>();
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        var factory = serviceProvider.GetService<ILoggerFactory>();
        if (factory != null)
            _mockLogger = factory.CreateLogger<UserLikeBusiness>();

    }

    private void CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDb")
        .Options;
        var context = new AppDbContext(options);

            foreach (var userLike in _userLikes)
                context.UserLikeBusinesses.Add(userLike);

        context.SaveChanges();
        _context = context;
    }

    /// <summary>
    /// Creates a list of Users
    /// </summary>
    private List<UserLikeBusiness> CreateUserLikes()
    {
        var userLikes =
            UserLikeBusinessTestData.UserLikeBusinesses as List<UserLikeBusiness>;

        return userLikes ?? new();
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
    private UsersLikeBusinessesRepository CreateUserLikeBusinessRepository(AppDbContext context, IMapper mapper, ILogger<UserLikeBusiness> logger)
    {
        var repository = new UsersLikeBusinessesRepository(context, mapper, logger); //TODO: setup mapper & logger to not be class members of each Test classes
        return repository;
    }

    [Test]
    public void AddUserLikes_Valid()
    {
        var userLikeRepository =
            CreateUserLikeBusinessRepository(_context, _mapper.Object, _mockLogger);

        var result = userLikeRepository.AddUserLikes(new() { BusinessId = "4", Id = 4, UserId = Guid.NewGuid() }).Result;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data.BusinessId, Is.EqualTo("4"));
        Assert.That(result.Data.Id, Is.EqualTo(4));
    }

    [Test, TestCaseSource(typeof(UserLikeBusinessTestData), nameof(UserLikeBusinessTestData.UserLikeBusinesses))]
    public async Task GetAllUserLikeBusinesses_Valid(UserLikeBusiness userLike)
    {
        var userLikeRepository =
        CreateUserLikeBusinessRepository(_context, _mapper.Object, _mockLogger);

        var result = userLikeRepository.AddUserLikes(userLike).Result;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data.BusinessName, Is.EqualTo($"Business Name #{userLike.Id}"));
            Assert.That(result.Data.Id, Is.EqualTo(userLike.Id));
        });
    }
}

public static class UserLikeBusinessTestData
{
    public static IEnumerable<UserLikeBusiness> UserLikeBusinesses
    {
        get
        {
            return new List<UserLikeBusiness>
            {
                new UserLikeBusiness
                {
                    Id = 1,
                    BusinessName = "Business Name #1",
                    BusinessId = "Business ID: 1",
                    UserId = Guid.NewGuid(),
                    Username = "username #1"
                },
                new UserLikeBusiness
                {
                    Id = 2,
                    BusinessName = "Business Name #2",
                    BusinessId = "Business ID: 2",
                    UserId = Guid.NewGuid(),
                    Username = "username #2"
                },
                new UserLikeBusiness
                {
                    Id = 3,
                    BusinessName = "Business Name #3",
                    BusinessId = "Business ID: 3",
                    UserId = Guid.NewGuid(),
                    Username = "username #3"
                }
            };
        }
    }
}
