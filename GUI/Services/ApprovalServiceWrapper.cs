using API.Services;
using BukuKita.Model;
using BukuKita;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GUI.Services
{
    // Simple display class untuk binding ke DataGridView
    public class ApprovalDisplayItem
    {
        public string IdApproval { get; set; } = "";
        public string NamaPeminjam { get; set; } = "";
        public string IdBuku { get; set; } = "";
        public string JudulBuku { get; set; } = "";
        public DateTime TanggalPengajuan { get; set; }
        public string Status { get; set; } = "";
        public string Keterangan { get; set; } = "";
    }

    /// <summary>
    /// Simple wrapper tanpa DataTable - langsung ke List binding
    /// </summary>
    public class SimpleApprovalWrapper
    {
        private readonly ApprovalService _approvalService;
        private readonly MainMenu _mainMenu;

        public SimpleApprovalWrapper()
        {
            try
            {
                _mainMenu = new MainMenu();
                _approvalService = new ApprovalService(_mainMenu);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all approvals sebagai List untuk binding
        /// </summary>
        public List<ApprovalDisplayItem> GetAllApprovalsForDisplay()
        {
            try
            {
                var approvals = _approvalService.GetAllApprovals();
                return ConvertToDisplayItems(approvals);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all approvals: {ex.Message}");
            }
        }

        /// <summary>
        /// Get pending approvals sebagai List untuk binding
        /// </summary>
        public List<ApprovalDisplayItem> GetPendingApprovalsForDisplay()
        {
            try
            {
                var approvals = _approvalService.GetPendingApprovals();
                return ConvertToDisplayItems(approvals);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting pending approvals: {ex.Message}");
            }
        }

        /// <summary>
        /// Get raw approvals data
        /// </summary>
        public List<Approval> GetAllApprovals()
        {
            return _approvalService.GetAllApprovals();
        }

        /// <summary>
        /// Get raw pending approvals
        /// </summary>
        public List<Approval> GetPendingApprovals()
        {
            return _approvalService.GetPendingApprovals();
        }

        /// <summary>
        /// Process approval
        /// </summary>
        public (bool Success, string Message) ProcessApproval(string id, string status, string keterangan = "")
        {
            try
            {
                var result = _approvalService.ProcessApproval(id, status, keterangan);
                return (result.IsSuccess, result.Message ?? "");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create new approval
        /// </summary>
        public (bool Success, string Message) CreateApproval(string idBuku, string namaPeminjam)
        {
            try
            {
                var result = _approvalService.CreateApproval(idBuku, namaPeminjam);
                return (result.IsSuccess, result.Message ?? "");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Search approvals
        /// </summary>
        public List<ApprovalDisplayItem> SearchApprovals(string keyword, List<Approval> sourceList)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return ConvertToDisplayItems(sourceList);

            var filtered = sourceList.Where(a =>
                a.namaPeminjam.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                a.judulBuku.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                a.idBuku.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                a.status.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                a.idApproval.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            return ConvertToDisplayItems(filtered);
        }

        /// <summary>
        /// Convert to display items
        /// </summary>
        private List<ApprovalDisplayItem> ConvertToDisplayItems(List<Approval> approvals)
        {
            return approvals.Select(a => new ApprovalDisplayItem
            {
                IdApproval = a.idApproval,
                NamaPeminjam = a.namaPeminjam,
                IdBuku = a.idBuku,
                JudulBuku = a.judulBuku,
                TanggalPengajuan = a.tanggalPengajuan,
                Status = a.status,
                Keterangan = a.keterangan ?? ""
            }).ToList();
        }

        /// <summary>
        /// Test connection
        /// </summary>
        public bool IsConnected()
        {
            try
            {
                _approvalService.GetAllApprovals();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}