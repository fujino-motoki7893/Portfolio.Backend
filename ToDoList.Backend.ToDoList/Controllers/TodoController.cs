using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Backend.ToDoList.Models.DbContexts;
using ToDoList.Backend.ToDoList.Models.Entities;

namespace ToDoList.Backend.ToDoList.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var items = await _context.Items.ToListAsync(); // Todos → Items に変更
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodo(int id)
        {
            var item = await _context.Items.FindAsync(id); // Todos → Items に変更
            if (item == null)
                return NotFound();
            
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] CreateTodoRequest request)
        {
            var item = new Item
            {
                // Itemエンティティのプロパティに合わせて設定
                // 例：Title = request.Title, Description = request.Description
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetTodo), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] UpdateTodoRequest request)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound();

            // Itemのプロパティを更新
            // 例：item.Title = request.Title;

            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class CreateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Itemエンティティに合わせてプロパティを調整してください
    }

    public class UpdateTodoRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Itemエンティティに合わせてプロパティを調整してください
    }
}
