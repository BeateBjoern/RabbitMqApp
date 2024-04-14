using RabbitMQ.Client; 
using Services;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;


var logger = NLog.LogManager.Setup().GetCurrentClassLogger();


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string csvFilePath = builder.Configuration.GetValue<string>("CsvFilePath");
builder.Services.AddSingleton<IProducerService>(provider =>
    new ProducerService(
        provider.GetRequiredService<IConnectionFactory>(),
        csvFilePath
    )
);
builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>(provider => // Registering ConnectionFactory as the implementation of IConnectionFactory
{
    // Configure RabbitMQ connection settings from appsettings.json
    // If the RABBITMQ_HOST environment variable is not set, use localhost as the default value
    var hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
    return new ConnectionFactory { HostName = hostName };
});


builder.Logging.ClearProviders();
builder.Host.UseNLog(); 





var app = builder.Build();



   

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/*app.UseHttpsRedirection();*/

/*app.UseCors("CorsPolicy");*/
/*app.UseAuthorization();*/
app.UseRouting();
app.MapControllers();

app.Run();