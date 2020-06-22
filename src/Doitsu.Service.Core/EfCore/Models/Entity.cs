namespace EFCore.Abstractions.Models
{
    public abstract class Entity
    {
    }

    public abstract class Entity<T> : Entity
    {
        public T Id { get; set; }
    }
}
