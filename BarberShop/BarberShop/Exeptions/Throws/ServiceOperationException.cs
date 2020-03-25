using System;

namespace BarberShop.Exeptions.Throws
{
    public class ServiceOperationException : InvalidOperationException
    {
        public ServiceOperationException(string message) :
            base(message)
        {
        }

        public ServiceOperationException(string message, Exception innerException) :
            base(message, innerException)
        {

        }
    }
}
