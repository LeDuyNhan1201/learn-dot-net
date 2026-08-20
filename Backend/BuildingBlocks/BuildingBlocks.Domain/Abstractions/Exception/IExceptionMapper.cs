using BuildingBlocks.Domain.Exceptions;

namespace BuildingBlocks.Domain.Abstractions.Exception;

public interface IExceptionMapper
{
    ExceptionContext Map(System.Exception exception);
}