namespace LafiamiAPI.Models.Responses
{
    public class StateResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class EditableStateResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enable { get; set; }
        public int CountryId { get; set; }
    }
}
