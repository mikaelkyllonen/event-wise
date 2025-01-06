namespace EventWise.Api.Common;

public interface IRule<T> where T : class
{
    Task<Result> CheckAsync(T entity);
}