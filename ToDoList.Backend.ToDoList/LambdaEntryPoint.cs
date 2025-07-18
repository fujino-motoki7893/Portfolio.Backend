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
            builder
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseLambdaServer();
        }
    }
}
