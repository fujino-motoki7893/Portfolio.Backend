namespace ToDoList.Domains.DTOs
{
    /// <summary>
    /// Item を作成するためのリクエスト
    /// </summary>
    public class CreateItemRequest
    {
        /// <summary>
        /// DynamoDB のパーティションキー
        /// </summary>
        public int PartitionKey { get; set; }

        /// <summary>
        /// アイテム名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// アイテムの説明
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
