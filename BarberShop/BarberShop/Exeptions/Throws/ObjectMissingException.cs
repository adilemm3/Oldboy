namespace BarberShop.Exeptions.Throws
{
    public class ObjectMissingException : ServiceOperationException
    {
        public ObjectMissingException(string message) :
            base(message)
        {
        }
    }
}
