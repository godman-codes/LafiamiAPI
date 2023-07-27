namespace LafiamiAPI.Models.Responses
{

    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IconURl { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public int? ParentId { get; set; }
        public int OrderBy { get; set; }
        //public bool ShowInMenu { get; set; }
        public bool UseAsFilter { get; set; }
        public bool UseforVariation { get; set; }
        public string DefaultVariations { get; set; }
    }

    public class LiteCategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public long? ParentId { get; set; }
        //public bool ShowInMenu { get; set; }
        public int OrderBy { get; set; }
        public bool UseAsFilter { get; set; }
    }

    public class LiteCategoryForProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public long? ParentId { get; set; }
        public int OrderBy { get; set; }
    }
}
