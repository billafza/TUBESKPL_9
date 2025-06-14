using API.Models;

namespace API.Repositories
{
    public class InMemoryApprovalRepository : IApprovalRepository
    {
        private static readonly List<Approval> _approvals = new();
        private static int _idCounter = 1;

        public List<Approval> GetAll()
        {
            return _approvals.ToList();
        }

        public List<Approval> GetPending()
        {
            return _approvals.Where(a => a.Status == "Pending").ToList();
        }

        public Approval? GetById(string id)
        {
            return _approvals.FirstOrDefault(a => a.IdApproval == id);
        }

        public List<Approval> GetByUser(string userName)
        {
            return _approvals.Where(a => a.NamaPeminjam.Equals(userName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public void Add(Approval approval)
        {
            _approvals.Add(approval);
        }

        public void Update(Approval approval)
        {
            var existing = GetById(approval.IdApproval);
            if (existing != null)
            {
                var index = _approvals.IndexOf(existing);
                _approvals[index] = approval;
            }
        }

        public bool Delete(string id)
        {
            var approval = GetById(id);
            if (approval != null)
            {
                return _approvals.Remove(approval);
            }
            return false;
        }

        public string GenerateNewId()
        {
            return $"APV{_idCounter++:D3}";
        }
    }
}