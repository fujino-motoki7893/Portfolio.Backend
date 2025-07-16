using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Options;
using ToDoList.Models;

namespace ToDoList.Services
{
    /// <summary>
    /// DynamoDB を操作するためのサービス
    /// </summary>
    public class DynamoDBService(
        IAmazonDynamoDB dynamoDBClient,
        IOptions<DynamoDBSettings> dynamoDBSettings,
        ILogger<DynamoDBService> logger) : IDynamoDBService
    {
        private readonly IAmazonDynamoDB _dynamoDBClient = dynamoDBClient ?? throw new ArgumentNullException(nameof(dynamoDBClient));
        private readonly DynamoDBSettings _dynamoDBSettings = dynamoDBSettings?.Value ?? throw new ArgumentNullException(nameof(dynamoDBSettings));
        private readonly ILogger<DynamoDBService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        ///  partitionKey を指定してitem を取得する
        /// </summary>
        /// <param name="partitionKey">パーティションキー</param>
        /// <returns>取得したItem </returns>
        /// <exception cref="">Item が取得できない場合</exception>
        public async Task<GetItemResponse> GetItemAsync(int partitionKey)
        {
            try
            {
                var request = new GetItemRequest
                {
                    TableName = _dynamoDBSettings.TableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "PartitionKey", new AttributeValue { N = partitionKey.ToString() } }
                    }
                };

                return await _dynamoDBClient.GetItemAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting item with partition key: {partitionKey}");
                throw;
            }
        }

        /// <summary>
        ///  Item を作成する
        /// </summary>
        /// <param name="request">Item を作成するためのリクエスト</param>
        /// <returns>Item が作成されたか </returns>
        /// <exception cref="">Item が作成されなかった場合</exception>
        public async Task<PutItemResponse> PutItemAsync(Dictionary<string, AttributeValue> item)
        {
            try
            {
                var request = new PutItemRequest
                {
                    TableName = _dynamoDBSettings.TableName,
                    Item = item
                };

                return await _dynamoDBClient.PutItemAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error putting item");
                throw;
            }
        }

        /// <summary>
        ///  Item を更新する
        /// </summary>
        /// <param name="partitionKey">更新するItem のpartitionKey </param>
        /// <param name="updates">更新するItem の内容 </param>
        /// <returns>更新されたItem </returns>
        /// <exception cref="">Item が更新されなかった場合</exception>
        public async Task<UpdateItemResponse> UpdateItemAsync(int partitionKey, Dictionary<string, AttributeValueUpdate> updates)
        {
            try
            {
                var request = new UpdateItemRequest
                {
                    TableName = _dynamoDBSettings.TableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "PartitionKey", new AttributeValue { N = partitionKey.ToString() } }
                    },
                    AttributeUpdates = updates
                };

                return await _dynamoDBClient.UpdateItemAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating item with partition key: {partitionKey}");
                throw;
            }
        }

        /// <summary>
        ///  Item を削除する
        /// </summary>
        /// <param name="partitionKey">削除するItem のpartitionKey </param>
        public async Task<DeleteItemResponse> DeleteItemAsync(int partitionKey)
        {
            try
            {
                var request = new DeleteItemRequest
                {
                    TableName = _dynamoDBSettings.TableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "PartitionKey", new AttributeValue { N = partitionKey.ToString() } }
                    }
                };

                return await _dynamoDBClient.DeleteItemAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting item with partition key: {partitionKey}");
                throw;
            }
        }

        /// <summary>
        ///  item を取得する
        /// </summary>
        public async Task<ScanResponse> ScanAsync()
        {
            try
            {
                var request = new ScanRequest
                {
                    TableName = _dynamoDBSettings.TableName
                };

                return await _dynamoDBClient.ScanAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning table");
                throw;
            }
        }
    }
}
