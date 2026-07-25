namespace BuildingBlocks.Domain.Exceptions.Processors;

public interface IExceptionMapper
{
    ExceptionContext Map(Exception exception);
}