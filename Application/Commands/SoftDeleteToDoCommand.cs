using Application.Commands.Contracts;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Application.Commands
{
    public class SoftDeleteToDoCommand : ISoftDeleteToDoCommand
    {
        private readonly AppDbContext _context;

        private readonly IClock _clock;

        public SoftDeleteToDoCommand(AppDbContext context, IClock clock)
        {
            _context = context;
            _clock = clock;
        }

        public async Task SoftDeleteAsync(long id)
        {
            var toDo = await _context.ToDos.Where(x => x.Id == id).SingleAsync();
            toDo.DeletedAtUtc = _clock.GetCurrentInstant().ToDateTimeUtc();
            _context.ToDos.Update(toDo);
            await _context.SaveChangesAsync();
        }
    }
}
