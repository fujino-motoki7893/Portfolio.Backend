namespace ToDoList.Models
{
    /// <summary>
    /// AWSの設定をするためのクラス
    /// </summary>
    public class AWSSettings
    {
        /// <summary>
        /// ユーザのプロファイル
        /// </summary>
        public string Profile { get; set; } = string.Empty;

        /// <summary>
        /// リージョン（地域）
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// ユーザのアクセスキー
        /// </summary>
        public string AccessKey { get; set; } = string.Empty;

        /// <summary>
        /// ユーザのシークレットキー
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;
    }
}
