using Application.Commands.Contracts;
using Application.Requests;
using Core.Entities;

namespace Application.Commands
{
    public class CreateAuthorCommand : ICreateAuthorCommand
    {
        private readonly AppDbContext _context;

        public CreateAuthorCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task<long> CreateAsync(CreateAuthorRequest createAuthorRequest, long tenantId)
        {
            var author = new Author(
                tenantId, 
                createAuthorRequest.FullName);
            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();
            return author.Id;
        }
    }
}