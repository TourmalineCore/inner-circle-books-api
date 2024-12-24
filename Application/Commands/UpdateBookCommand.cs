using Application.Commands.Contracts;
using Application.Requests;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class UpdateBookCommand : IUpdateBookCommand
{
    private readonly AppDbContext _context;

    public UpdateBookCommand(AppDbContext context)
    {
        _context = context;
    }

    public async Task UpdateAsync(long id, UpdateBookRequest updateBookRequest, long tenantId)
    {
        var book = await _context.Books
            .Where(x => x.Id == id && x.TenantId == tenantId)
            .SingleAsync();

        book.Title = updateBookRequest.Title;
        book.Annotation = updateBookRequest.Annotation;
        book.Language = (Language)Enum.Parse(typeof(Language), updateBookRequest.Language);
        book.Authors = updateBookRequest.Authors.Select(x => new Author() { FullName = x.FullName }).ToList();
        book.ArtworkUrl = updateBookRequest.ArtworkUrl;

        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }
}