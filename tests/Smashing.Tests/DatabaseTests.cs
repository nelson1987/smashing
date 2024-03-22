/*
using Smashing.Core.Bases;

namespace Smashing.Tests;

[CollectionDefinition("DatabaseCollection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    // This class is used to define a shared collection of DatabaseFixture instances.
}

[Collection("DatabaseCollection")]
public class DatabaseTests
{
    private readonly DatabaseFixture _databaseFixture;

    public DatabaseTests(DatabaseFixture fixture)
    {
        _databaseFixture = fixture;
    }

    [Fact]
    public void Test1()
    {
        // Use _databaseFixture to access the shared database instance
    }

    [Trait("Category", "Unit")]
    public void Add_SimpleAddition()
    {
        // Unit test logic
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void Multiply_SimpleMultiplication()
    {
        // Integration test logic
    }
    //[InlineData(-1, 1, 0)]
    //public void Add_SimpleAddition(int a, int b, int expected)
    //{
    //    // Arrange & Act
    //    int result = MathHelper.Add(a, b);
    //    // Assert
    //    Assert.Equal(expected, result);
    //}
}

public class DatabaseFixture
{
}

public class PostDummyData
{
    public IEnumerable<BaseEntity> GetAllPost()
    {
        return new List<BaseEntity>
        {
            new()
            {
                Id = Guid.Parse("FBFF1432-05BC-4686-A888-90B86A70D07C"), Title = "test1", UserName = "test1",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("FBFF1432-05BC-4686-A888-90B86A70D07D"), Title = "test2", UserName = "test2",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("FBFF1432-05BC-4686-A888-90B86A70D07E"), Title = "test3", UserName = "test3",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("FBFF1432-05BC-4686-A888-90B86A70D07F"), Title = "test4", UserName = "test4",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("FBFF1432-05BC-4686-A888-90B86A70D07G"), Title = "test5", UserName = "test5",
                CreatedAt = DateTime.UtcNow
            }
        };
    }
}*/

