using DomainModel.Entities;

namespace DomainModel.Repositories
{
    public  interface IAppSettingsRepository
    {
        AppSettings Get();

        void Save(AppSettings appSettings);
    }
}
