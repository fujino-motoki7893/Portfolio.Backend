using System.Text.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ToDoList.Controllers;
using ToDoList.Domains;
using ToDoList.Infrastructures;
using ToDoList.Infrastructures.Interface;
using ToDoList.Models;
using ToDoList.Models.DbContexts;
using ToDoList.Profiles;
using ToDoList.Services;
using ToDoList.Usecases;

namespace ToDoList
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));

            // 設定の読み込み
            services.Configure<AWSSettings>(configuration.GetSection("AWS"));
            services.Configure<DynamoDBSettings>(configuration.GetSection("DynamoDB"));

            // AWS DynamoDB クライアントの設定
            var awsSettings = configuration.GetSection("AWS").Get<AWSSettings>();
            if (awsSettings != null)
            {
                services.AddSingleton<IAmazonDynamoDB>(provider =>
                {
                    var config = new AmazonDynamoDBConfig
                    {
                        RegionEndpoint = RegionEndpoint.GetBySystemName(awsSettings.Region)
                    };

                    return new AmazonDynamoDBClient(awsSettings.AccessKey, awsSettings.SecretKey, config);
                });
            }

            services.AddScoped<IDynamoDBService, DynamoDBService>();

            // サービスの登録
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite("Data Source=conferences.db"));

            // リポジトリを登録
            services.AddScoped<IReadItemRepository, ReadItemRepository>();

            // ユースケースを登録
            services.AddScoped<IReadTodoUsecase, ReadTodoInteractor>();

            // REST API用のサービスを追加
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 詳細なエラー情報を表示
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;

                    var response = new
                    {
                        error = "Internal Server Error",
                        message = exception?.Message,
                        stackTrace = env.IsDevelopment() ? exception?.StackTrace : null
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                });
            });

            // Lambda環境ではDeveloperExceptionPageを使用しない
            if (env.IsDevelopment() && !env.EnvironmentName.Equals("Lambda"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();
            
            // Lambda環境ではHTTPSリダイレクションを使用しない
            if (!env.EnvironmentName.Equals("Lambda"))
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
                endpoints.MapControllers();
                endpoints.MapGet("/", () => "Hello World!");
                endpoints.MapGet("/health", () => "OK"); // ヘルスチェック用
            });
        }
    }
}
