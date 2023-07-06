using DomainModel.Queries;

namespace DomainModel.QueryServices
{
    public interface IAsyncQueryService<TQuery, TResult> : IQueryService<TQuery, TResult>
        where TQuery : ServiceQuery<TResult>
    {
        Task<TResult> ExecuteAsync(TQuery query);
    }
}