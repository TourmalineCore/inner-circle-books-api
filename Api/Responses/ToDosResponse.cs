namespace Api.Responses;

public class ToDosResponse
{
    public List<ToDo> ToDos { get; set; }
}

public class ToDo
{
    public long Id { get; set; }
    public string Name { get; set; }
}