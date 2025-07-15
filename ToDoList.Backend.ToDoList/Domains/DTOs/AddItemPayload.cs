namespace ToDoList.Domains.DTOs
{
    /// <summary>
    ///  AddItemPayloadクラス
    /// </summary>
    public class AddItemPayload
    {
        /// <summary>
        /// ItemのDTO
        /// </summary>
        public ItemDTO Item { get; set; } = new ItemDTO();
    }
}
