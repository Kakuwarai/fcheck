using fcheck.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DTCHECKERContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddSingleton(connectionString);


builder.Services.AddCors(
       o =>
        o.AddPolicy(
            "Policy",
            builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }
         )
    );

var app = builder.Build();

app.UseCors("Policy");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}
app.UseSwagger();
app.UseSwaggerUI();
/*
app.UseHttpsRedirection();
*/
app.UseAuthorization();

app.MapControllers();

app.Run();
