namespace Core.Entities
{
    public class Language
    {
        public long Id { get; set; }

        public string Value { get; set; }

        public Language() { }

        public Language(string value)
        {
            Value = value;
        }
    }
}
