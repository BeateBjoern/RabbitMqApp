using Consumer1;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Data;
using Util;
using Services;
using RabbitMQ.Client;
using Interfaces;
using System.IO;
using System;

public class Program
{
    public static void Main(string[] args)
    {
        LoadEnvironmentVariables();

        // Get the environment variable to determine the environment from the command line
        var environment = args[0];

        // Feed the environment variable to the CreateHostBuilder method
        CreateHostBuilder(args, environment).Build().Run();
    }

    // Configure the host and services with environment variables using CreateHostBuilder method
    public static IHostBuilder CreateHostBuilder(string[] args, string environment) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                // Hosted services are automatically created and started by the host
                services.AddHostedService<Worker>();

                // Register the ConnectionFactory as the implementation of IConnectionFactory 
                // Worker class depends on this instance and uses the RabbitMQConnectionFactory class to create a connection to RabbitMQ
                services.AddSingleton<IRabbitMQConnectionFactory>(provider =>
                {
                    // Create a new instance of the RabbitMQConnectionFactory class with the host name
                    string hostName;
                    if (environment == "dev")
                    {
                        // Use localhost as the default in dev mode
                        hostName = "localhost";
                    }
                    else
                    {
                      
                        hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"); 
                    }

                    return new RabbitMQConnectionFactory(hostName);
                });

                // Client connection to MongoDB server - reusing the same client connection. Uses IDisposable, but the DI container manages the lifetime
                services.AddSingleton<IMongoClient>(provider =>
                {
                    var connectionString = Environment.GetEnvironmentVariable("MONGODB_STRING");
                    return new MongoClient(connectionString);
                });


                // This service always takes the same instance of the IMongoClient service on construction
                services.AddSingleton<MongoDBContext>();
                services.AddSingleton<MessageHandler>();

                // Create a StreamWriter instance for writing to a CSV file
                services.AddSingleton(provider =>
                {
                    var filePath = "./Shared/Messages.csv";
                    return new StreamWriter(filePath, append: true);
                });

                services.AddSingleton<CsvWriterService>();
                services.AddSingleton<MessageProcessor>();
            });

    private static void LoadEnvironmentVariables()
    {
        var root = Directory.GetCurrentDirectory();
        var dotenv = Path.Combine(root, ".env");
        if (File.Exists(dotenv))
        {
            DotEnv.Load(dotenv);
        }
    }
};
