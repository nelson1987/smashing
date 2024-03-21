namespace Smashing.Core;
public class StudentEvent
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }

    public static implicit operator StudentEvent(Student v)
    {
        return new StudentEvent()
        {
            Id = v.Id,
            UserName = v.UserName,
            Title = v.Title,
            CreatedAt = v.CreatedAt,
        };
    }
}
public class Student
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }

    public static implicit operator Student(StudentEvent v)
    {
        return new Student()
        {
            Id = v.Id,
            UserName = v.UserName,
            Title = v.Title,
            CreatedAt = v.CreatedAt,
        };
    }
}
