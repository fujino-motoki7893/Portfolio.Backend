namespace ToDoList.Models
{
    /// <summary>
    /// Googleの設定をするためのクラス
    /// </summary>
    public class GoogleSettings
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// シークレット
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;
    }
}
