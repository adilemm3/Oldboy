namespace BarberShop.Exeptions.Throws
{
    public class ResourceAlreadyExistException : ServiceOperationException
    {
        public ResourceAlreadyExistException(string message) :
            base(message)
        {
        }
    }
}
