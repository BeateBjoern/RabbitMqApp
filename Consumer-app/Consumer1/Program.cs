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
using NLog;
using NLog.Web; 

public class Program
{
    public static void Main(string[] args)
    {
        LoadEnvironmentVariables();
        // Get the environment variable to determine the environment from the command line
        var environment = args[0];

        // Create the host builder and build the host
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
                // Worker class injected with IRabbitMQConnectionFactory instance for connection 
                services.AddSingleton<IRabbitMQConnectionFactory>(provider =>
                {
                    // Setting hostname for RabbitMQ server
                    string hostName;
                    if (environment == "dev")
                    {
                        // Use localhost as the default in development mode
                        hostName = "localhost";
                    }
                    else
                    {
                        // Use the environment variable in production mode
                        hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
                    }

                    // Return a new RabbitMQConnectionFactory instance
                    return new RabbitMQConnectionFactory(hostName);

                });



                // Inject the IMongoClient service into the MongoDBContext service
                services.AddSingleton<IMongoClient>(provider =>
                {
                    var connectionString = Environment.GetEnvironmentVariable("MONGODB_STRING");
                    return new MongoClient(connectionString);
                });



                // Injecting MongoDBContext (uses IMongoClient)
                services.AddSingleton<MongoDBContext>();

                //Injecting MessageHandler
                services.AddSingleton<MessageHandler>();

                // Adding a singleton service for the StreamWriter
                services.AddSingleton(provider =>
                {
                    var filePath = "./Shared/Messages.csv";
                    return new StreamWriter(filePath, append: true);
                });

                // Injecting the CsvWriterService (uses StreamWriter instance)
                services.AddSingleton<CsvWriterService>();

                //Injecting MessageProcessor
                services.AddSingleton<MessageProcessor>();


            }).UseNLog(); //Adding logging to the hostbuilder 






    // Load environment variables from the .env file
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
