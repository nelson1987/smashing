namespace Smashing.Core;

public class BaseEvent
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }

    public static implicit operator BaseEvent(BaseEntity v)
    {
        return new BaseEvent
        {
            Id = v.Id,
            UserName = v.UserName,
            Title = v.Title,
            CreatedAt = v.CreatedAt
        };
    }
}

public class BaseEntity
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }

    public static implicit operator BaseEntity(BaseEvent v)
    {
        return new BaseEntity
        {
            Id = v.Id,
            UserName = v.UserName,
            Title = v.Title,
            CreatedAt = v.CreatedAt
        };
    }
}
