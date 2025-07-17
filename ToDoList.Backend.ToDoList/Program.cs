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

            // AutoMapperの登録
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // 設定の読み込み
            builder.Services.Configure<AWSSettings>(builder.Configuration.GetSection("AWS"));
            builder.Services.Configure<DynamoDBSettings>(builder.Configuration.GetSection("DynamoDB"));

            // AWS DynamoDB クライアントの設定
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

            builder.Services.AddScoped<IDynamoDBService, DynamoDBService>();

            // サービスの登録
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite("Data Source=conferences.db"));

            // リポジトリを登録
            builder.Services.AddScoped<IReadItemRepository, ReadItemRepository>();

            // ユースケースを登録
            builder.Services.AddScoped<IReadTodoUsecase, ReadTodoInteractor>();

            // REST API用のサービスを追加
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            app.UseCors();

            // Swagger設定（開発環境のみ）
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ルーティングのミドルウェアを追加
            app.UseRouting();

            // エンドポイントのミドルウェアを設定
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
                endpoints.MapControllers(); // REST APIコントローラーをマップ
            });

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
