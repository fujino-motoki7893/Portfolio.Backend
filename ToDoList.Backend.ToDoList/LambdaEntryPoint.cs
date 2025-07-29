using Amazon.Lambda.AspNetCoreServer;

namespace ToDoList
{
    /// <summary>
    /// AWS Lambda用のエントリーポイント
    /// </summary>
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        /// <summary>
        /// Lambda関数の初期化
        /// </summary>
        /// <param name="builder">ホストビルダー</param>
        protected override void Init(IWebHostBuilder builder)
        {
            // 環境変数を設定してLambda環境であることを明示
            Environment.SetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME", "ToDoListFunction");
            
            builder
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Lambda環境用の設定
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", 
                        optional: true, reloadOnChange: false);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    // Program.csのサービス登録ロジックを呼び出すか、
                    // ここで直接Lambda用のサービスを登録
                    ConfigureLambdaServices(services, context.Configuration);
                })
                .Configure((context, app) =>
                {
                    // Lambda用のミドルウェア設定
                    ConfigureLambdaApp(app, context.HostingEnvironment);
                });
        }

        private static void ConfigureLambdaServices(IServiceCollection services, IConfiguration configuration)
        {
            // Lambda環境用のサービス登録
            services.AddHttpClient();
            services.AddControllers();
            
            // DynamoDB、その他のサービス
            // セッションや認証は除外
        }

        private static void ConfigureLambdaApp(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Lambda用のミドルウェア設定
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", () => "Hello from Lambda!");
                endpoints.MapGet("/health", () => "OK");
            });
        }
    }
}
