using F360.Domain.Dtos.Messages;
using F360.Domain.Enums;
using F360.Domain.Interfaces.Database.Repositories;
using F360.Infrastructure.Configuration;
using F360.Infrastructure.Database.Configuration;
using F360.Infrastructure.Database.Repositories;
using F360.Infrastructure.HttpClients;
using F360.Workers.JobConsumer.Consumers;
using MassTransit;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IJobRepository, JobRepository>();

builder.Services.AddHttpClient<ViaCepClient>();

var rabbitMqSettings = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>()!;

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<JobMessageConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqSettings.Host, h =>
        {
            h.Username(rabbitMqSettings.Username);
            h.Password(rabbitMqSettings.Password);
        });

        cfg.Message<JobMessage>(e =>
        {
            e.SetEntityName("JobMessage");
        });

        cfg.Publish<JobMessage>(p =>
        {
            p.ExchangeType = "direct";
        });

        cfg.Send<JobMessage>(s =>
        {
            s.UseRoutingKeyFormatter(context =>
            {
                var message = context.Message;
                return message.Priority.ToString();
            });
        });

        cfg.ReceiveEndpoint("f360.job.high", e =>
        {
            e.ConfigureConsumeTopology = false;
            e.Bind<JobMessage>(b =>
            {
                b.RoutingKey = nameof(JobPriority.High);
                b.ExchangeType = "direct";
            });

            e.ConfigureConsumer<JobMessageConsumer>(context);
        });

        cfg.ReceiveEndpoint("f360.job.low", e =>
        {
            e.ConfigureConsumeTopology = false;
            e.Bind<JobMessage>(b =>
            {
                b.RoutingKey = nameof(JobPriority.Low);
                b.ExchangeType = "direct";
            });

            e.ConfigureConsumer<JobMessageConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();
