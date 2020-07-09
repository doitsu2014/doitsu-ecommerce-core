using System;

namespace Doitsu.Ecommerce.ApplicationCore
{
    public abstract class Entity
    {
    }

    public abstract class Entity<T> : Entity
    {
        public T Id { get; set; }
    }
}
