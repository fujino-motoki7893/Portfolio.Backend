using Amazon.DynamoDBv2.Model;

namespace ToDoList.Services
{
    /// <summary>
    /// DynamoDB を操作するためのインタフェース
    /// </summary>
    public interface IDynamoDBService
    {
        /// <summary>
        ///  partitionKey を指定してitem を取得する
        /// </summary>
        /// <param name="partitionKey">パーティションキー</param>
        Task<GetItemResponse> GetItemAsync(int partitionKey);

        /// <summary>
        ///  Item を作成する
        /// </summary>
        /// <param name="request">Item を作成するためのリクエスト</param>        
        Task<PutItemResponse> PutItemAsync(Dictionary<string, AttributeValue> item);

        /// <summary>
        ///  Item を更新する
        /// </summary>
        /// <param name="partitionKey">更新するItem のpartitionKey </param>
        /// <param name="updates">更新するItem の内容 </param>
        Task<UpdateItemResponse> UpdateItemAsync(int partitionKey, Dictionary<string, AttributeValueUpdate> updates);

        /// <summary>
        ///  Item を削除する
        /// </summary>
        /// <param name="partitionKey">削除するItem のpartitionKey </param>
        Task<DeleteItemResponse> DeleteItemAsync(int partitionKey);

        /// <summary>
        ///  item を取得する
        /// </summary>
        Task<ScanResponse> ScanAsync();
    }
}
