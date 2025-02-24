
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Confluent.Kafka;
using Nest;
using ConstructionProjectManagement.Data;
using ConstructionProjectManagement.Middleware;
using ConstructionProjectManagement.Services;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;


namespace ConstructionProjectManagement
{
    public class Startup 
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ConstructionDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    builder => 
                    {
                        builder.WithOrigins("http://localhost:5173") // Update with your frontend URL
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddControllers();

            var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            var settings = new ConnectionSettings(new Uri(Configuration["ElasticSearch:Uri"]))
             .DefaultIndex(Configuration["ElasticSearch:DefaultIndex"])
                .DisableDirectStreaming() // Enable capturing request and response streams
            .PrettyJson()
            .RequestTimeout(TimeSpan.FromMinutes(2));
            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);

            //services.AddSingleton<IElasticClient>(new ElasticClient(new ConnectionSettings(new Uri(Configuration["ElasticSearch:Uri"]))));

            services.AddSingleton<IProducer<Null, string>>(provider =>
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = Configuration["Kafka:BootstrapServers"],
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.Plain,
                    SaslUsername = Configuration["Kafka:SaslUsername"],
                    SaslPassword = Configuration["Kafka:SaslPassword"],
                    SslCaLocation = Configuration["Kafka:SslCaLocation"]
                };
                return new ProducerBuilder<Null, string>(config).Build();
            });

            services.AddSingleton<IConsumer<Null, string>>(provider =>
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = Configuration["Kafka:BootstrapServers"],
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.Plain,
                    SaslUsername = Configuration["Kafka:SaslUsername"],
                    SaslPassword = Configuration["Kafka:SaslPassword"],
                    SslCaLocation = Configuration["Kafka:SslCaLocation"],
                    GroupId = "construction-consumer-group",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };
                return new ConsumerBuilder<Null, string>(config).Build();
            });

            services.AddSingleton<KafkaProducerService>();
            services.AddHostedService<KafkaConsumerService>();
            services.AddSingleton<ElasticSearchService>();

            var serviceProvider = services.BuildServiceProvider();
            var elasticSearchService = serviceProvider.GetRequiredService<ElasticSearchService>();
                elasticSearchService.EnsureIndexExistsAsync("constructionprojects").Wait();

            //var postgresConnectionString = Configuration.GetConnectionString("PostgresConnection");
            //services.AddSingleton(new PostgresServices(postgresConnectionString));

            //services.AddControllers();

            // Add Swagger services
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Construction Project Management API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRequestLogging();
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Construction Project Management API V1");
                c.RoutePrefix = string.Empty;  // Set Swagger UI at the app's root (default is /swagger)
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        }
    }
}