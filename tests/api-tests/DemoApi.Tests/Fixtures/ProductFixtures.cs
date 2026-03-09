using DemoApi.Models;

namespace DemoApi.Tests.Fixtures;

public static class ProductFixtures
{
    public static List<Product> GetSampleProducts() =>
    [
        new Product
        {
            Id = "69ab8dde87baaa2e147c2907",
            Name = "Cloud Architecture Report",
            Category = "Infrastructure",
            Description = "A comprehensive overview of modern cloud architecture patterns.",
            Author = "Feres Mrad",
            PublishedAt = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            Tags = ["cloud", "architecture", "kubernetes"]
        },
        new Product
        {
            Id = "69ab8dde87baaa2e147c2908",
            Name = "Microservices Best Practices",
            Category = "Development",
            Description = "Key patterns and anti-patterns when building microservices.",
            Author = "Feres Mrad",
            PublishedAt = new DateTime(2025, 2, 20, 0, 0, 0, DateTimeKind.Utc),
            Tags = ["microservices", "dotnet", "api"]
        },
        new Product
        {
            Id = "69ab8dde87baaa2e147c2909",
            Name = "CI/CD Pipeline Design",
            Category = "DevOps",
            Description = "How to design robust CI/CD pipelines for containerized apps.",
            Author = "Feres Mrad",
            PublishedAt = new DateTime(2025, 3, 10, 0, 0, 0, DateTimeKind.Utc),
            Tags = ["cicd", "devops", "testing"]
        }
    ];

    public static Product GetSingleProduct() => GetSampleProducts()[0];
}