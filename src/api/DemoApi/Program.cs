using DemoApi.Configuration;
using DemoApi.Services;
using QuestPDF.Infrastructure;

// QuestPDF community license — free for open source / demo projects
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// ── Configuration ────────────────────────────────────────────────────────────
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// ── Services ─────────────────────────────────────────────────────────────────
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<PdfService>();

// ── CORS (allows Angular dev server to call the API) ─────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",  // Angular dev server
                "http://demo.local")      // Kubernetes ingress
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
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

// ── Middleware ────────────────────────────────────────────────────────────────
app.UseCors("AllowAngular");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo API v1");
    c.RoutePrefix = "swagger";
});

// ── Routes ────────────────────────────────────────────────────────────────────
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithTags("Health")
   .WithSummary("Health check endpoint");

app.MapGet("/api/products", async (ProductService svc) =>
{
    var products = await svc.GetAllAsync();
    return Results.Ok(products);
})
.WithTags("Products")
.WithSummary("Get all products");

app.MapGet("/api/products/{id}", async (string id, ProductService svc) =>
{
    var product = await svc.GetByIdAsync(id);
    return product is null ? Results.NotFound() : Results.Ok(product);
})
.WithTags("Products")
.WithSummary("Get a product by ID");

app.MapGet("/api/report/download", async (PdfService pdfSvc, ProductService productSvc) =>
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