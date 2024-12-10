namespace Core.Entities
{
    public class Author
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public List<Book> Books { get; set; }

        public Author() { }

        public Author(string name)
        {
            Name = name;
        }
    }
}