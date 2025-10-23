using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries
{
    public class GetRecordOfTakingBookCopyByCopyIdQuery
    {
        private readonly AppDbContext _context;

        public GetRecordOfTakingBookCopyByCopyIdQuery(AppDbContext context)
        {
            _context = context;
        }

        public Task<BookCopyReadingHistory?> GetAsync(long bookCopyId, long tenantId)
        {
            return _context
                .BooksCopiesReadingHistory
                //.Where(x => x.TenantId == tenantId)
                .Where(x => x.BookCopyId == bookCopyId && x.ActualReturnedAtUtc == null)
                .FirstOrDefaultAsync();
        }
    }
}
