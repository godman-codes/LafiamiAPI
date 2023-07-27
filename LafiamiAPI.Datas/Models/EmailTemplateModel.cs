using LafiamiAPI.Utilities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace LafiamiAPI.Datas.Models
{
    public partial class EmailTemplateModel : EntityBase<long>
    {
        public EmailTemplateModel() : base()
        {

        }
        [Column(TypeName = "ntext")]
        public string Template { get; set; }
        public string Subject { get; set; }
        public EmailTypeEnums EmailType { get; set; }
    }
}
