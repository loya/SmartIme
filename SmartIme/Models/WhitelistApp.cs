namespace SmartIme.Models
{
    public class WhitelistApp
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Path { get; set; }

        public override string ToString()
        {
            return $"{DisplayName ?? Name} ({Path})";
        }
    }
}
