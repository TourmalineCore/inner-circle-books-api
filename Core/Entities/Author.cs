namespace Core.Entities
{
    public class Author
    {
        public long Id { get; set; }

        public long TenantId { get; set; }
        
        public string Name { get; set; }
        
        public List<Book> Books { get; set; }

        public Author() { }

        public Author(
            long tenantId,
            string name)
        {
            TenantId = tenantId;
            Name = name;
        }
    }
}