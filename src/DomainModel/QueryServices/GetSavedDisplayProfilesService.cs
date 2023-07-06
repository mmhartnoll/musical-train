using DomainModel.Entities;
using DomainModel.Queries;
using DomainModel.Repositories;

namespace DomainModel.QueryServices
{
    public class GetSavedDisplayProfilesService : IAsyncQueryService<GetSavedDisplayProfilesQuery, IEnumerable<DisplayProfile>>
    {
        private readonly IProfileRepository repository;

        public GetSavedDisplayProfilesService(IProfileRepository repository)
            => this.repository = repository;

        public IEnumerable<DisplayProfile> Execute(GetSavedDisplayProfilesQuery query)
            => repository.GetAll();

        public Task<IEnumerable<DisplayProfile>> ExecuteAsync(GetSavedDisplayProfilesQuery query)
            => Task.FromResult(Execute(query));
    }
}