using DomainModel.Entities;
using DomainModel.Queries;
using DomainModel.Repositories;

namespace DomainModel.QueryServices
{
    public class GetSavedAppSettingsService : IQueryService<GetSavedAppSettingsQuery, AppSettings>
    {
        private readonly IAppSettingsRepository repository;

        public GetSavedAppSettingsService(IAppSettingsRepository repository)
            => this.repository = repository;

        public AppSettings Execute(GetSavedAppSettingsQuery query)
            => repository.Get();
    }
}