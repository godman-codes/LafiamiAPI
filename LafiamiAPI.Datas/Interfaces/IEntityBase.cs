using System;

namespace LafiamiAPI.Datas.Interfaces
{
    public interface IEntityBase<T>
    {
        T Id { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime UpdatedDate { get; set; }
        bool IsDeleted { get; set; }
    }
}
