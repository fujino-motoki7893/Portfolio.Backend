using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2.Model;
using ToDoList.Services;
using ToDoList.Domains.DTOs;

namespace ToDoList.Controllers
{
    /// <summary>
    ///  Item のコントローラクラス（DynamoDB接続用）
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController(IDynamoDBService dynamoDBService) : ControllerBase
    {
        private readonly IDynamoDBService _dynamoDBService = dynamoDBService;

        /// <summary>
        ///  partitionKey を指定してitem を取得する
        /// </summary>
        /// <param name="partitionKey">パーティションキー</param>
        /// <returns>取得したItem </returns>
        /// <exception cref="">Item が取得できない場合</exception>
        [HttpGet("{partitionKey}")]
        public async Task<IActionResult> GetItem(int partitionKey)
        {
            try
            {
                var response = await _dynamoDBService.GetItemAsync(partitionKey);
                
                if (response.Item.Count == 0)
                {
                    return NotFound();
                }

                return Ok(response.Item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///  Item を作成する
        /// </summary>
        /// <param name="request">Item を作成するためのリクエスト</param>
        /// <returns>Item が作成されたか </returns>
        /// <exception cref="">Item が作成されなかった場合</exception>
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemRequest request)
        {
            try
            {
                var item = new Dictionary<string, AttributeValue>
                {
                    { "PartitionKey", new AttributeValue { N = request.PartitionKey.ToString() } },
                    { "Name", new AttributeValue { S = request.Name } },
                    { "Description", new AttributeValue { S = request.Description } }
                };

                await _dynamoDBService.PutItemAsync(item);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///  Item を削除する
        /// </summary>
        /// <param name="partitionKey">削除するItem のpartitionKey </param>
        /// <returns>Item が削除されたか </returns>
        /// <exception cref="">Item が削除されなかった場合</exception>
        [HttpDelete]
        public async Task<IActionResult> DeleteItem(int partitionKey)
        {
            try
            {
                var response = await _dynamoDBService.GetItemAsync(partitionKey);
                
                if (response.Item.Count == 0)
                {
                    return NotFound();
                }

                await _dynamoDBService.DeleteItemAsync(partitionKey);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///  item を取得する
        /// </summary>
        /// <returns>取得したItem </returns>
        /// <exception cref="">Item が取得できない場合</exception>
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            try
            {
                var response = await _dynamoDBService.ScanAsync();
                return Ok(response.Items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
