namespace Core.Entities
{
    public class Author
    {
        public long Id { get; set; }

        public long TenantId { get; set; }
        
        public string FullName { get; set; }
        
        public List<Book> Books { get; set; }

        public Author() { }

        public Author(
            long tenantId,
            string fullName)
        {
            TenantId = tenantId;
            FullName = fullName;
        }
        public async Task<bool> DeleteBook(Book book)
        {
            return Books.Remove(book);
        }
    }
}