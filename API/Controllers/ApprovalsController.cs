using Microsoft.AspNetCore.Mvc;
using BukuKita.Model;
using API.Models.DTOs;
using API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Approvals")]
    public class ApprovalsController : ControllerBase
    {
        private readonly ApprovalService _approvalService;

        public ApprovalsController(ApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        /// <summary>
        /// Get all approvals
        /// </summary>
        /// <returns>List of all approvals</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all approvals", Description = "Retrieves all approval records in the system")]
        [SwaggerResponse(200, "Successfully retrieved all approvals")]
        [SwaggerResponse(500, "Internal server error")]
        public ActionResult<IEnumerable<Approval>> GetAllApprovals()
        {
            try
            {
                var approvals = _approvalService.GetAllApprovals();
                return Ok(new
                {
                    success = true,
                    data = approvals,
                    count = approvals.Count,
                    message = "Successfully retrieved all approvals"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get pending approvals
        /// </summary>
        /// <returns>List of pending approvals</returns>
        [HttpGet("pending")]
        [SwaggerOperation(Summary = "Get pending approvals", Description = "Retrieves all approval records with pending status")]
        [SwaggerResponse(200, "Successfully retrieved pending approvals")]
        [SwaggerResponse(500, "Internal server error")]
        public ActionResult<IEnumerable<Approval>> GetPendingApprovals()
        {
            try
            {
                var approvals = _approvalService.GetPendingApprovals();
                return Ok(new
                {
                    success = true,
                    data = approvals,
                    count = approvals.Count,
                    message = "Successfully retrieved pending approvals"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get approval by ID
        /// </summary>
        /// <param name="id">Approval ID</param>
        /// <returns>Approval details</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get approval by ID", Description = "Retrieves a specific approval by its ID")]
        [SwaggerResponse(200, "Approval found")]
        [SwaggerResponse(404, "Approval not found")]
        [SwaggerResponse(500, "Internal server error")]
        public ActionResult<Approval> GetApprovalById(string id)
        {
            try
            {
                var approval = _approvalService.GetApprovalById(id);
                if (approval == null)
                    return NotFound(new { success = false, message = "Approval not found" });

                return Ok(new
                {
                    success = true,
                    data = approval,
                    message = "Approval found successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get approvals by user
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns>List of user's approvals</returns>
        [HttpGet("user/{userName}")]
        [SwaggerOperation(Summary = "Get approvals by user", Description = "Retrieves all approvals for a specific user")]
        [SwaggerResponse(200, "Successfully retrieved user approvals")]
        [SwaggerResponse(500, "Internal server error")]
        public ActionResult<IEnumerable<Approval>> GetApprovalsByUser(string userName)
        {
            try
            {
                var approvals = _approvalService.GetApprovalsByUser(userName);
                return Ok(new
                {
                    success = true,
                    data = approvals,
                    count = approvals.Count,
                    message = $"Successfully retrieved approvals for user: {userName}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Create new approval
        /// </summary>
        /// <param name="request">Approval creation request</param>
        /// <returns>Created approval</returns>
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

                var result = _approvalService.CreateApproval(request.IdBuku, request.NamaPeminjam);

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
        /// <param name="id">Approval ID</param>
        /// <param name="request">Processing request</param>
        /// <returns>Processed approval</returns>
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

                var result = _approvalService.ProcessApproval(id, request.Status, request.Keterangan ?? "");

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
        /// <param name="id">Approval ID</param>
        /// <returns>Deletion result</returns>
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
                var result = _approvalService.DeleteApproval(id);

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
}