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
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// ホストビルダーの作成
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        /// <returns>ホストビルダー</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
