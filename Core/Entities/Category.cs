namespace Core.Entities
{
    public class Category
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public Category() { }

        public Category(string name)
        {
            Name = name;
        }
    }
}
