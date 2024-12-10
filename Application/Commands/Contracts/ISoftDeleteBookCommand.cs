namespace Application.Commands.Contracts;

public interface ISoftDeleteBookCommand
{
    Task SoftDeleteAsync(long id);
}