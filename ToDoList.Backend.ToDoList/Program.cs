using Microsoft.EntityFrameworkCore;
using ToDoList.Backend.ToDoList.Controllers;
using ToDoList.Backend.ToDoList.Models.DbContexts;

var builder = WebApplication.CreateBuilder(args);

// サービスの登録
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=conferences.db"));

// REST API用のサービスを追加
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

var app = builder.Build();

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
