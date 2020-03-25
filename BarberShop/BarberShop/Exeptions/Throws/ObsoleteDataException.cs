namespace BarberShop.Exeptions.Throws
{
    public class ObsoleteDataException : ServiceOperationException
    {
        public ObsoleteDataException(string message) :
            base(message)
        {
        }
    }
}
