using System;
using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Models.Requests
{
    public class NewCategoryRequest
    {
        [Required]
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
    public class ExistingCategoryRequest : NewCategoryRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Id is required")]
        public int Id { get; set; }
    }

    public class CategoryPositionRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Id is required")]
        public int Id { get; set; }
        public int OrderBy { get; set; }
    }
}
