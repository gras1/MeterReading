var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ensek.Net.MeterReading.Api", Version = "v1" });
});

ConfigurationManager configuration = builder.Configuration;
builder.Services.AddTransient<IDbConnection>(db => new SqliteConnection(configuration.GetSection("DatabaseOptions").GetSection("ConnectionString").Value));

builder.Services.AddScoped<IFormFileToByteArrayConverter, FormFileToByteArrayConverter>();
builder.Services.AddScoped<IMeterReadingHandler, MeterReadingHandler>();
builder.Services.AddScoped<IFileProcessor, FileProcessor>();
builder.Services.AddScoped<IMeterReadingProcessor, MeterReadingProcessor>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IMeterReadingsRepository, MeterReadingsRepository>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader());
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ensek.Net.MeterReading.Api v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseCors("Open");

app.UseAuthorization();

app.MapControllers();

app.Run();
