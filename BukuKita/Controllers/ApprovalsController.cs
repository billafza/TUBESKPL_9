using Microsoft.AspNetCore.Mvc;
using BukuKita.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace BukuKita.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Approvals")]
    public class ApprovalsController : ControllerBase
    {
        private readonly MainMenu _mainMenu;

        public ApprovalsController(MainMenu mainMenu)
        {
            _mainMenu = mainMenu;
        }

        /// <summary>
        /// Get all approvals
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all approvals", Description = "Retrieves all approval records in the system")]
        [SwaggerResponse(200, "Successfully retrieved all approvals")]
        [SwaggerResponse(500, "Internal server error")]
        public ActionResult<IEnumerable<Approval>> GetAllApprovals()
        {
            try
            {
                var approvals = _mainMenu.GetAllApprovals();
                return Ok(new { success = true, data = approvals, count = approvals.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get pending approvals
        /// </summary>
        [HttpGet("pending")]
        [SwaggerOperation(Summary = "Get pending approvals", Description = "Retrieves all approval records with pending status")]
        [SwaggerResponse(200, "Successfully retrieved pending approvals")]
        [SwaggerResponse(500, "Internal server error")]
        public ActionResult<IEnumerable<Approval>> GetPendingApprovals()
        {
            try
            {
                var approvals = _mainMenu.GetPendingApprovals();
                return Ok(new { success = true, data = approvals, count = approvals.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get approval by ID
        /// </summary>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get approval by ID", Description = "Retrieves a specific approval by its ID")]
        [SwaggerResponse(200, "Approval found")]
        [SwaggerResponse(404, "Approval not found")]
        [SwaggerResponse(500, "Internal server error")]
        public ActionResult<Approval> GetApprovalById(string id)
        {
            try
            {
                var approval = _mainMenu.GetApprovalById(id);
                if (approval == null)
                    return NotFound(new { success = false, message = "Approval not found" });

                return Ok(new { success = true, data = approval });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Create new approval
        /// </summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create new approval", Description = "Creates a new approval request for book borrowing")]
        [SwaggerResponse(201, "Approval created successfully")]
        [SwaggerResponse(400, "Invalid request data")]
        [SwaggerResponse(404, "Book not found")]
        [SwaggerResponse(500, "Internal server error")]
        public ActionResult<Approval> CreateApproval([FromBody] CreateApprovalRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.IdBuku) || string.IsNullOrEmpty(request.NamaPeminjam))
                    return BadRequest(new { success = false, message = "IdBuku and NamaPeminjam are required" });

                var book = _mainMenu.GetBookById(request.IdBuku);
                if (book == null)
                    return NotFound(new { success = false, message = "Book not found" });

                var result = _mainMenu.CreateApprovalWithValidation(request.IdBuku, book.judul, request.NamaPeminjam);

                if (!result.IsSuccess)
                    return BadRequest(new { success = false, message = result.ErrorMessage });

                return CreatedAtAction(nameof(GetApprovalById), new { id = result.Data.idApproval },
                    new { success = true, data = result.Data, message = "Approval created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Process approval (approve/reject)
        /// </summary>
        [HttpPut("{id}/process")]
        [SwaggerOperation(Summary = "Process approval", Description = "Approve or reject an approval request")]
        [SwaggerResponse(200, "Approval processed successfully")]
        [SwaggerResponse(400, "Invalid request data")]
        [SwaggerResponse(404, "Approval not found")]
        [SwaggerResponse(500, "Internal server error")]
        public ActionResult<Approval> ProcessApproval(string id, [FromBody] ProcessApprovalRequest request)
        {
            try
            {
                if (request.Status != "Approved" && request.Status != "Rejected")
                    return BadRequest(new { success = false, message = "Status must be 'Approved' or 'Rejected'" });

                var result = _mainMenu.ProcessApprovalWithValidation(id, request.Status, request.Keterangan ?? "");

                if (!result.IsSuccess)
                    return BadRequest(new { success = false, message = result.ErrorMessage });

                return Ok(new { success = true, data = result.Data, message = "Approval processed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Delete approval
        /// </summary>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete approval", Description = "Delete a pending approval request")]
        [SwaggerResponse(200, "Approval deleted successfully")]
        [SwaggerResponse(400, "Cannot delete non-pending approval")]
        [SwaggerResponse(404, "Approval not found")]
        [SwaggerResponse(500, "Internal server error")]
        public ActionResult DeleteApproval(string id)
        {
            try
            {
                var result = _mainMenu.DeleteApproval(id);

                if (!result.IsSuccess)
                    return BadRequest(new { success = false, message = result.ErrorMessage });

                return Ok(new { success = true, message = "Approval deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }

    // Request DTOs
    public class CreateApprovalRequest
    {
        /// <summary>
        /// ID buku yang akan dipinjam
        /// </summary>
        /// <example>B01</example>
        public string IdBuku { get; set; } = "";

        /// <summary>
        /// Nama peminjam
        /// </summary>
        /// <example>John Doe</example>
        public string NamaPeminjam { get; set; } = "";
    }

    public class ProcessApprovalRequest
    {
        /// <summary>
        /// Status: "Approved" atau "Rejected"
        /// </summary>
        /// <example>Approved</example>
        public string Status { get; set; } = "";

        /// <summary>
        /// Keterangan atau catatan (opsional)
        /// </summary>
        /// <example>Peminjaman disetujui</example>
        public string Keterangan { get; set; } = "";
    }
}