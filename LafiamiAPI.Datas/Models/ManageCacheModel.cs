using System;

namespace LafiamiAPI.Datas.Models
{
    public partial class ManageCacheModel : EntityBase<Guid>
    {
        public ManageCacheModel() : base()
        {
        }

        public string SourceName { get; set; }
        public bool IsCleared { get; set; }
    }
}
