using Application.Commands.Contracts;
using Application.Requests;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands
{
    public class UpdateBookCommand : IUpdateBookCommand
    {
        private readonly AppDbContext _context;

        public UpdateBookCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task UpdateAsync(UpdateBookRequest updateBookRequest, long tenantId)
        {
            var book = await _context.Books
                .Where(x => x.Id == updateBookRequest.Id && x.TenantId == tenantId)
                .SingleAsync();

            book.Title = updateBookRequest.Title;
            book.Annotation = updateBookRequest.Annotation;
            book.Language = (Language)Enum.Parse(typeof(Language), updateBookRequest.Language);
            var authorsList = await AddAuthorIfItNotExist(updateBookRequest, tenantId);
            book.Authors = authorsList;
            book.ArtworkUrl = updateBookRequest.ArtworkUrl;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        private async Task<List<Author>> AddAuthorIfItNotExist(UpdateBookRequest updateBookRequest, long tenantId)
        {
            var authors = new List<Author>();
            foreach(var authorFullName in updateBookRequest.Authors)
            {
                if(!_context.Authors.Any(x => x.FullName == authorFullName && x.TenantId == tenantId))
                {
                    _context.Authors.Add(new Author(tenantId, authorFullName));
                    await _context.SaveChangesAsync();
                }

                var existAuthor = _context.Authors.Single(x => x.FullName == authorFullName && x.TenantId == tenantId);
                authors.Add(existAuthor);
                await _context.SaveChangesAsync();
            }

            return authors;
        }
    }
}