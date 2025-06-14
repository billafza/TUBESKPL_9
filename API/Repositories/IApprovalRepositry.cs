using API.Models;

namespace API.Repositories
{
    public interface IApprovalRepository
    {
        List<Approval> GetAll();
        List<Approval> GetPending();
        Approval? GetById(string id);
        List<Approval> GetByUser(string userName);
        void Add(Approval approval);
        void Update(Approval approval);
        bool Delete(string id);
        string GenerateNewId();
    }
}