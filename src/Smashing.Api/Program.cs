using Smashing.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddDependencies();
//var mySqlConnectionString = builder
//    .Configuration
//    .GetConnectionString("mysql");

//builder.Services.AddContexts(mySqlConnectionString);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program
{
}