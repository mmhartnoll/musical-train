using DomainModel.Entities;

namespace DomainModel.Repositories
{
    public interface IProfileRepository
    {
        IEnumerable<DisplayProfile> GetAll();

        void Add(DisplayProfile profile);

        void Update(DisplayProfile profile);
    }
}
