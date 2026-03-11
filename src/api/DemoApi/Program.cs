using DemoApi.Configuration;
using DemoApi.Services;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<PdfService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "http://demo.local")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Demo API",
        Version = "v1",
        Description = "Backend API for the k8s-testkube-demo project"
    });
});

var app = builder.Build();

app.UseCors("AllowAngular");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo API v1");
    c.RoutePrefix = "swagger";
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithTags("Health")
   .WithSummary("Health check endpoint");

app.MapGet("/api/products", async (IProductService svc) =>
{
    var products = await svc.GetAllAsync();
    return Results.Ok(products);
})
.WithTags("Products")
.WithSummary("Get all products");

app.MapGet("/api/products/{id}", async (string id, IProductService svc) =>
{
    var product = await svc.GetByIdAsync(id);
    return product is null ? Results.NotFound() : Results.Ok(product);
})
.WithTags("Products")
.WithSummary("Get a product by ID");

app.MapGet("/api/report/download", async (PdfService pdfSvc, IProductService productSvc) =>
{
    var products = await productSvc.GetAllAsync();
    var pdfBytes = pdfSvc.GenerateProductReport(products);

    return Results.File(
        pdfBytes,
        contentType: "application/pdf",
        fileDownloadName: $"product-report-{DateTime.UtcNow:yyyyMMdd}.pdf"
    );
})
.WithTags("Report")
.WithSummary("Download a PDF report of all products");

app.Run();

public partial class Program { }