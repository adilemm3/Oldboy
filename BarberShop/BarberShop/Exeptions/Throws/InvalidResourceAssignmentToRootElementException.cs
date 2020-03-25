namespace BarberShop.Exeptions.Throws
{
    public class InvalidResourceAssignmentToRootElementException : ServiceOperationException
    {
        public InvalidResourceAssignmentToRootElementException(string message) :
            base(message)
        {
        }
    }
}
