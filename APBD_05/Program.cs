
using APBD_05.repository;
using APBD_05.service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddXmlSerializerFormatters();
builder.Services.AddScoped<IAnimalDatabase, RuntimeAnimalDb>();
builder.Services.AddScoped<IVisitDatabase, RuntimeVisitDb>();
builder.Services.AddTransient<MockDbPopulationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

using var scope = app.Services.CreateScope();

var mockDbPopulationService = scope.ServiceProvider.GetRequiredService<MockDbPopulationService>();

mockDbPopulationService.MockAnimalRecords();
mockDbPopulationService.MockAnimalVisits();

app.MapControllers();

app.Run();