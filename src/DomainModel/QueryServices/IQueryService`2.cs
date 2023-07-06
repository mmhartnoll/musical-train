using DomainModel.Queries;

namespace DomainModel.QueryServices
{
    public interface IQueryService<TQuery, TResult> 
        where TQuery : ServiceQuery<TResult>
    {
        TResult Execute(TQuery query);
    }
}