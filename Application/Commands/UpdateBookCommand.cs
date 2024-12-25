using Application.Commands.Contracts;
using Application.Requests;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class EditBookCommand : IEditBookCommand
{
    private readonly AppDbContext _context;

    public EditBookCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task EditAsync(long id, EditBookRequest editBookRequest, long tenantId)
    {
        var book = await _context.Books
            .Where(x => x.TenantId == tenantId)
            .Where(x => x.Id == id)
            .SingleAsync();

        book.Title = editBookRequest.Title;
        book.Annotation = editBookRequest.Annotation;
        book.Language = (Language)Enum.Parse(typeof(Language), editBookRequest.Language);
        book.Authors = editBookRequest.Authors.Select(x => new Author() { FullName = x.FullName }).ToList();
        book.BookCoverUrl = editBookRequest.BookCoverUrl;

        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }
}