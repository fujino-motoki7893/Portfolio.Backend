using Microsoft.AspNetCore.Mvc;
using ToDoList.Services;

namespace ToDoList.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    /// <summary>
    ///  DB の活性をヘルスチェックするためのコントローラクラス
    /// </summary>
    public class HealthController(IDynamoDBService dynamoDBService, ILogger<HealthController> logger) : ControllerBase
    {
        private readonly IDynamoDBService _dynamoDBService = dynamoDBService;
        private readonly ILogger<HealthController> _logger = logger;

        /// <summary>
        ///  DynamoDB のヘルスチェック
        /// </summary>
        /// <returns>DBのヘルス</returns>
        /// <exception cref="">500エラーの場合</exception>
        [HttpGet("dynamodb")]
        public async Task<IActionResult> CheckDynamoDB()
        {
            try
            {
                // 簡単な接続テスト
                await _dynamoDBService.ScanAsync();
                return Ok("DynamoDB connection successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DynamoDB health check failed");
                return StatusCode(500, $"DynamoDB connection failed: {ex.Message}");
            }
        }
    }
}
