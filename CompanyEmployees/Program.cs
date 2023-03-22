using AspNetCoreRateLimit;
using CompanyEmployees.Extensions;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Utility;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using NLog;
using Service.DataShaping;
using Shared.DataTransferObjects;

var builder = WebApplication.CreateBuilder(args);

// nlog register
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(),
    "/nlog.config"));
// Add services to the container.
// ServiceExtension Class
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
// Suppress default validation
builder.Services.Configure<ApiBehaviorOptions>(option => { option.SuppressModelStateInvalidFilter = true; });
// Filters action like services
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<ValidateMediaTypeAttribute>();
// Shape data
builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();
// Data Shaped Links
builder.Services.AddScoped<IEmployeeLinks, EmployeeLinks>();
// Caching
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
// Rate Limit
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();
// Identity
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
// JWT
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
// Program Class
builder.Services.AddControllers(config =>
    {
        config.RespectBrowserAcceptHeader = true;
        config.ReturnHttpNotAcceptable = true;
        config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
        config.CacheProfiles.Add("120SecondsDuration", new CacheProfile { Duration = 120 });
    })
    .AddXmlDataContractSerializerFormatters()
    .AddCustomCSVFormatter()
    .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);
// Custom Media type
builder.Services.AddCustomMediaType();

var app = builder.Build();

// custom exception
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

if (app.Environment.IsProduction())
    app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseIpRateLimiting();

app.UseCors("CorsPolicy");

app.UseResponseCaching();
app.UseHttpCacheHeaders();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
    new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson().Services.BuildServiceProvider()
        .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
        .OfType<NewtonsoftJsonPatchInputFormatter>().First();