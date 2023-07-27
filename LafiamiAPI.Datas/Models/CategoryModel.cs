using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LafiamiAPI.Datas.Models
{
    public partial class CategoryModel : EntityBase<int>
    {
        public CategoryModel() : base()
        {
            InsuranceCategories = new HashSet<InsuranceCategory>();
            InsuranceAuditCategories= new HashSet<InsuranceAuditCategoryModel>();
        }

        public string Name { get; set; }
        public string IconURl { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public int? ParentId { get; set; }
        public int OrderBy { get; set; }
        //public bool ShowInMenu { get; set; }
        public bool UseAsFilter { get; set; }
        public bool UseforVariation { get; set; }
        [Column(TypeName = "ntext")]
        public string DefaultVariations { get; set; }


        public virtual ICollection<InsuranceCategory> InsuranceCategories { get; set; }
        public virtual ICollection<InsuranceAuditCategoryModel> InsuranceAuditCategories { get; set; }
    }
}
