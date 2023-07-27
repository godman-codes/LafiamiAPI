using System;

namespace LafiamiAPI.Models.Internals
{
    public class LiteObjectId<T>
    {
        public T Id { get; set; }
    }

    public class ObjectId<T>
    {
        public T Id { get; set; }
        public bool GenerateEmail { get; set; }
    }

    public class LiteOrderInfo
    {
        public Guid OrderId { get; set; }
        public bool GenerateEmail { get; set; }
    }
}
