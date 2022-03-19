namespace DevUp.Domain.Entities
{
    public interface IEntityId<TId>
    {
        TId Id { get; }
    }
}
