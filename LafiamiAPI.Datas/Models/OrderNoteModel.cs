using System;

namespace LafiamiAPI.Datas.Models
{
    public partial class OrderNoteModel : EntityBase<Guid>
    {
        public OrderNoteModel() : base()
        {

        }
        public Guid OrderId { get; set; }
        public string Note { get; set; }
        public bool? Closedorder { get; set; }

        public virtual OrderModel Order { get; set; }
    }
}
