namespace LafiamiAPI.Models.Responses
{
    public class CountryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TwoLetterIsoCode { get; set; }
        public string ThreeLetterIsoCode { get; set; }
    }

    public class EditableCountryResponse : CountryResponse
    {
        public bool Enable { get; set; }
    }
}
