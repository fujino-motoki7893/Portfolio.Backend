namespace ToDoList.Models
{
    /// <summary>
    /// DynamoDB の設定クラス
    /// </summary>
    public class DynamoDBSettings
    {
        /// <summary>
        /// テーブル名
        /// </summary>
        public string TableName { get; set; } = string.Empty;
    }
}
