namespace Core.Entities
{
    public class Status
    {
        public long Id { get; set; }

        public string Value { get; set; }

        public Status() { }

        public Status(string value)
        {
            Value = value;
        }
    }
}
