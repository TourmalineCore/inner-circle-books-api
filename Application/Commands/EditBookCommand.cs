using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class EditBookCommandParams
{
    public string Title { get; set; }

    public string Annotation { get; set; }

    public string Language { get; set; }

    public List<Author> Authors { get; set; }

    public string BookCoverUrl { get; set; }
}

public class EditBookCommand
{
    private readonly AppDbContext _context;

    public EditBookCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task EditAsync(long id, EditBookCommandParams editBookCommandParams, long tenantId)
    {
        var book = await _context.Books
            .Where(x => x.TenantId == tenantId)
            .Where(x => x.Id == id)
            .SingleAsync();

        book.Title = editBookCommandParams.Title;
        book.Annotation = editBookCommandParams.Annotation;
        book.Language = (Language)Enum.Parse(typeof(Language), editBookCommandParams.Language);
        book.Authors = editBookCommandParams
            .Authors
            .Select(x => new Author() {
                FullName = x.FullName
            })
            .ToList();
        book.BookCoverUrl = editBookCommandParams.BookCoverUrl;

        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }
}