namespace Core.Entities
{
    public class Tag
    {
        public long Id { get; set; }

        public string Value { get; set; }

        public Tag() { }

        public Tag(string value) 
        {
            Value = value;
        }
    }
}
