using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ToDoList.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableCors]
    public class PhotosController(HttpClient httpClient, ILogger<PhotosController> logger) : ControllerBase
    {
        private readonly HttpClient _httpClient = httpClient;

        private readonly ILogger<PhotosController> _logger = logger;

        [HttpGet("token-info")]
        public async Task<IActionResult> GetTokenInfo()
        {
            var accessToken = HttpContext.Session.GetString("GoogleAccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("No access token found");
            }

            try
            {
                // Googleのトークン情報エンドポイントを呼び出し
                var tokenInfoUrl = $"https://www.googleapis.com/oauth2/v1/tokeninfo?access_token= {accessToken}";
                var response = await _httpClient.GetAsync(tokenInfoUrl);
                var content = await response.Content.ReadAsStringAsync();

                return Ok(new
                {
                    tokenInfo = JsonConvert.DeserializeObject(content),
                    rawResponse = content
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPhotos([FromQuery] int pageSize = 20, [FromQuery] string? pageToken = null)
        {
            // セッションからアクセストークンを取得
            var accessToken = HttpContext.Session.GetString("GoogleAccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("Access token not found. Please login again via /api/auth/login");
            }

            try
            {
                // Photos Library API のエンドポイント
                var baseUrl = "https://photoslibrary.googleapis.com/v1/mediaItems";
                var url = $"{baseUrl}?pageSize={pageSize}";

                if (!string.IsNullOrEmpty(pageToken))
                {
                    url += $"&pageToken={pageToken}";
                }

                // HTTPリクエストの準備
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // APIを呼び出し
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"API Error: {errorContent}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var photosResponse = JsonConvert.DeserializeObject<PhotosResponse>(content);

                // 結果をクライアントに返す
                var photoList = photosResponse?.MediaItems?.Select(item => new
                {
                    Id = item.Id,
                    Filename = item.Filename,
                    ProductUrl = item.ProductUrl,
                    BaseUrl = item.BaseUrl,
                    MimeType = item.MimeType,
                    CreationTime = item.MediaMetadata?.CreationTime
                }).ToList();

                return Ok(new
                {
                    Photos = photoList,
                    NextPageToken = photosResponse?.NextPageToken
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        [HttpPost("upload")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            var accessToken = HttpContext.Session.GetString("GoogleAccessToken");
            
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("Access token not found. Please login again.");
            }
            
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided or file is empty.");
            }

            try
            {
                // Step 1: 写真をアップロード
                var uploadUrl = "https://photoslibrary.googleapis.com/v1/uploads";
                string uploadToken;
                
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    
                    using var content = new ByteArrayContent(fileBytes);
                    
                    // Content-typeヘッダーを正しく設定
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    content.Headers.Add("X-Goog-Upload-Content-Type", file.ContentType ?? "application/octet-stream");
                    content.Headers.Add("X-Goog-Upload-Protocol", "raw");
                    
                    using var uploadRequest = new HttpRequestMessage(HttpMethod.Post, uploadUrl)
                    {
                        Content = content
                    };
                    uploadRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    
                    using var uploadResponse = await _httpClient.SendAsync(uploadRequest);
                    
                    if (!uploadResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await uploadResponse.Content.ReadAsStringAsync();
                        _logger?.LogError("Upload failed: {StatusCode} - {Error}", uploadResponse.StatusCode, errorContent);
                        return StatusCode((int)uploadResponse.StatusCode, $"Upload failed: {errorContent}");
                    }
                    
                    uploadToken = await uploadResponse.Content.ReadAsStringAsync();
                    
                    if (string.IsNullOrWhiteSpace(uploadToken))
                    {
                        return StatusCode(500, "Upload token is empty or null");
                    }
                }
                
                // Step 2: メディアアイテムを作成
                var createUrl = "https://photoslibrary.googleapis.com/v1/mediaItems:batchCreate "; // 末尾のスペースを削除
                
                var createRequestBody = new
                {
                    newMediaItems = new[]
                    {
                        new
                        {
                            description = "Uploaded from ToDoList App",
                            simpleMediaItem = new
                            {
                                fileName = string.IsNullOrWhiteSpace(file.FileName) ? "uploaded_image" : file.FileName,
                                uploadToken = uploadToken
                            }
                        }
                    }
                };
                
                // System.Text.Jsonを使用してより効率的なシリアライゼーション
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                };
                
                var jsonString = JsonSerializer.Serialize(createRequestBody, jsonOptions);
                
                using var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
                using var createRequest = new HttpRequestMessage(HttpMethod.Post, createUrl)
                {
                    Content = jsonContent
                };
                createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                
                using var createResponse = await _httpClient.SendAsync(createRequest);
                
                if (!createResponse.IsSuccessStatusCode)
                {
                    var errorContent = await createResponse.Content.ReadAsStringAsync();
                    _logger?.LogError("Create media item failed: {StatusCode} - {Error}", createResponse.StatusCode, errorContent);
                    return StatusCode((int)createResponse.StatusCode, $"Create media item failed: {errorContent}");
                }
                
                var resultJson = await createResponse.Content.ReadAsStringAsync();
                
                // レスポンスをパースして構造化されたデータとして返す
                var result = JsonSerializer.Deserialize<object>(resultJson, jsonOptions);
                
                _logger?.LogInformation("Photo uploaded successfully. File: {FileName}, Size: {FileSize} bytes", 
                    file.FileName, file.Length);
                
                return Ok(new 
                { 
                    message = "Photo uploaded successfully",
                    fileName = file.FileName,
                    fileSize = file.Length,
                    result = result
                });
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "HTTP request error during photo upload");
                return StatusCode(500, $"Network error occurred: {ex.Message}");
            }
            catch (JsonException ex)
            {
                _logger?.LogError(ex, "JSON serialization/deserialization error");
                return StatusCode(500, $"JSON processing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error during photo upload");
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }

    // レスポンス用のモデルクラス
    public class PhotosResponse
    {
        [JsonProperty("mediaItems")]
        public List<MediaItem>? MediaItems { get; set; }

        [JsonProperty("nextPageToken")]
        public string? NextPageToken { get; set; }
    }

    public class MediaItem
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("productUrl")]
        public string? ProductUrl { get; set; }

        [JsonProperty("baseUrl")]
        public string? BaseUrl { get; set; }

        [JsonProperty("mimeType")]
        public string? MimeType { get; set; }

        [JsonProperty("filename")]
        public string? Filename { get; set; }

        [JsonProperty("mediaMetadata")]
        public MediaMetadata? MediaMetadata { get; set; }
    }

    public class MediaMetadata
    {
        [JsonProperty("creationTime")]
        public string? CreationTime { get; set; }

        [JsonProperty("width")]
        public string? Width { get; set; }

        [JsonProperty("height")]
        public string? Height { get; set; }
    }
}
