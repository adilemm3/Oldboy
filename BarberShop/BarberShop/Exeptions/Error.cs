namespace BarberShop.Exeptions
{
    public class Error
    {
        public Error(uint identifier)
        {
            Identifier = identifier;
        }

        public Error(uint identifier, string description)
        {
            Identifier = identifier;
            Description = description;
        }

        public uint Identifier { get; }

        public string Description { get; }
    }
}
