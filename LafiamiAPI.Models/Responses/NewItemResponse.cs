namespace LafiamiAPI.Models.Responses
{
    public class NewItemResponse<T>
    {
        public NewItemResponse(T id, string message)
        {
            Id = id;
            Message = message;
        }

        public T Id { get; set; }
        public string Message { get; set; }
    }
}
