using Microsoft.EntityFrameworkCore;
using Amazon;
using Amazon.DynamoDBv2;
using ToDoList.Controllers;
using ToDoList.Domains;
using ToDoList.Models;
using ToDoList.Models.DbContexts;
using ToDoList.Usecases;
using ToDoList.Services;
using ToDoList.Profiles;
using ToDoList.Infrastructures;
using ToDoList.Infrastructures.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

namespace ToDoList
{
    /// <summary>
    /// プログラムのエントリーポイント
    /// </summary>
    public class Program
    {
        /// <summary>
        /// メインメソッド
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            var isLambdaEnvironment = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME"));
            if (isLambdaEnvironment)
            {
                builder.WebHost.ConfigureKestrel(options =>
                {
                    options.AllowSynchronousIO = true;
                });
                builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());
            }

            builder.Services.AddHttpClient();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddLogging();

            builder.Services.Configure<AWSSettings>(builder.Configuration.GetSection("AWS"));
            builder.Services.Configure<DynamoDBSettings>(builder.Configuration.GetSection("DynamoDB"));
            builder.Services.Configure<GoogleSettings>(builder.Configuration.GetSection("Google"));

            var awsSettings = builder.Configuration.GetSection("AWS").Get<AWSSettings>();
            if (awsSettings != null)
            {
                builder.Services.AddSingleton<IAmazonDynamoDB>(provider =>
                {
                    var config = new AmazonDynamoDBConfig
                    {
                        RegionEndpoint = RegionEndpoint.GetBySystemName(awsSettings.Region)
                    };

                    return new AmazonDynamoDBClient(awsSettings.AccessKey, awsSettings.SecretKey, config);
                });
            }

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                var googleAuthNSection = builder.Configuration.GetSection("Google").Get<GoogleSettings>();

                options.ClientId = googleAuthNSection!.ClientId;
                options.ClientSecret = googleAuthNSection.ClientSecret;
                options.Scope.Add("https://www.googleapis.com/auth/photoslibrary");
                options.Scope.Add("https://www.googleapis.com/auth/photoslibrary.readonly");
                options.Scope.Add("https://www.googleapis.com/auth/photoslibrary.appendonly");
                options.Scope.Add("https://www.googleapis.com/auth/photoslibrary.sharing");
                options.Scope.Add("https://www.googleapis.com/auth/photoslibrary.edit.appcreateddata");
                options.Scope.Add("https://www.googleapis.com/auth/photoslibrary.readonly.appcreateddata");
                options.SaveTokens = true;
                options.CallbackPath = "/signin-google";
            });

            builder.Services.AddScoped<IDynamoDBService, DynamoDBService>();

            // サービスの登録
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite("Data Source=conferences.db"));

            // リポジトリを登録
            builder.Services.AddScoped<IReadItemRepository, ReadItemRepository>();

            // ユースケースを登録
            builder.Services.AddScoped<IReadTodoUsecase, ReadTodoInteractor>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            
            builder.Services.AddSwaggerGen();

            builder.Services
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>();

            builder.Services.AddCors(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    options.AddDefaultPolicy(policyBuilder =>
                    {
                        policyBuilder
                            .SetIsOriginAllowed(_ => true)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
                }
                else
                {
                    options.AddDefaultPolicy(policyBuilder =>
                    {
                        policyBuilder
                            .WithOrigins("http://localhost:3000")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
                }

                options.AddPolicy("SwaggerPolicy", policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment() && !app.Environment.EnvironmentName.Equals("Lambda"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!isLambdaEnvironment && !app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseCors();
            if (!isLambdaEnvironment)
            {
                app.UseSession();
                app.UseAuthentication();
                app.UseAuthorization();
            }

            app.MapGraphQL();
            app.MapControllers();
            app.MapGet("/", () => "Hello World!");
            app.MapGet("/health", () => "OK");

            app.Run();
        }
    }
}
