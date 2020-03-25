using BarberShop.Exeptions.Throws;

namespace BarberShop.Exeptions
{
    public class ErrorFactory
    {
        public static Error IdentifyExceptionByType(ServiceOperationException exception)
        {
            switch (exception)
            {
                case ObsoleteDataException _:
                    return new Error(ErrorCodes.OldData, exception.Message);
                case ResourceAlreadyExistException _:
                    return new Error(ErrorCodes.ResourceAlreadyExist, exception.Message);
                case ObjectMissingException _:
                    return new Error(ErrorCodes.ObjectMissing, exception.Message);
                default:
                    return new Error(ErrorCodes.Undefined, exception.Message);
            }
        }
    }
}
