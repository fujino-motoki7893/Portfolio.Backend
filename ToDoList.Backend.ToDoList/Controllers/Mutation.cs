using ToDoList.Backend.ToDoList.Domains.DTOs;
using ToDoList.Backend.ToDoList.Models.Entities;
using ToDoList.Backend.ToDoList.Domains.DTOs;
using ToDoList.Backend.ToDoList.Models.DbContexts;

namespace ToDoList.Backend.ToDoList.Controllers
{
    /// <summary>
    /// ミューテーションクラス
    /// </summary>
    public class Mutation
    {
        /// <summary>
        /// アイテムを追加するメソッド
        /// </summary>
        /// <param name="input">アイテムを追加するインプット</param>
        /// <param name="context">アイテムを追加するためのコンテキスト</param>
        /// <returns>追加されたアイテム</returns>
        public async Task<AddItemPayload> AddItemAsync(
            AddItemInput input,
            [Service] ApplicationDbContext context)
        {
            var item = new Item
            {
                Name = input.Name,
                Content = input.Content
            };

            context.Items.Add(item);
            await context.SaveChangesAsync();

            // TODO: 処理をインタラクタに切り出す
            // TODO: マッパープロファイルを作成して、マッピングを行う
            // TODO: 例外処理を行う
            return new AddItemPayload
            {
                Item = new ItemDTO 
                {
                    Id = item.Id,
                    Name = item.Name,
                    Content = item.Content
                }
            };
        }
    }
}
