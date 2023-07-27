namespace LafiamiAPI.Models.Responses
{
    public class CityResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class EditableCityResponse : CityResponse
    {
        public bool Enable { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
    }
}
