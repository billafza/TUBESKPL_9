using Microsoft.AspNetCore.Mvc;
using API.Models;
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

        [HttpGet]
        [SwaggerOperation(Summary = "Get all approvals")]
        public ActionResult<IEnumerable<Approval>> GetAllApprovals()
        {
            try
            {
                var approvals = _approvalService.GetAllApprovals();
                return Ok(new
                {
                    success = true,
                    data = approvals,
                    count = approvals.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("pending")]
        [SwaggerOperation(Summary = "Get pending approvals")]
        public ActionResult<IEnumerable<Approval>> GetPendingApprovals()
        {
            try
            {
                var approvals = _approvalService.GetPendingApprovals();
                return Ok(new
                {
                    success = true,
                    data = approvals,
                    count = approvals.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get approval by ID")]
        public ActionResult<Approval> GetApprovalById(string id)
        {
            try
            {
                var approval = _approvalService.GetApprovalById(id);
                if (approval == null)
                    return NotFound(new { success = false, message = "Approval not found" });

                return Ok(new { success = true, data = approval });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("user/{userName}")]
        [SwaggerOperation(Summary = "Get approvals by user")]
        public ActionResult<IEnumerable<Approval>> GetApprovalsByUser(string userName)
        {
            try
            {
                var approvals = _approvalService.GetApprovalsByUser(userName);
                return Ok(new
                {
                    success = true,
                    data = approvals,
                    count = approvals.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create new approval")]
        public ActionResult<Approval> CreateApproval([FromBody] CreateApprovalRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.IdBuku) || string.IsNullOrEmpty(request.NamaPeminjam))
                    return BadRequest(new { success = false, message = "IdBuku and NamaPeminjam are required" });

                var result = _approvalService.CreateApproval(request.IdBuku, request.NamaPeminjam);

                if (!result.IsSuccess)
                    return BadRequest(new { success = false, message = result.ErrorMessage });

                return CreatedAtAction(nameof(GetApprovalById), new { id = result.Data.IdApproval },
                    new { success = true, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPut("{id}/process")]
        [SwaggerOperation(Summary = "Process approval")]
        public ActionResult<Approval> ProcessApproval(string id, [FromBody] ProcessApprovalRequest request)
        {
            try
            {
                if (request.Status != "Approved" && request.Status != "Rejected")
                    return BadRequest(new { success = false, message = "Status must be 'Approved' or 'Rejected'" });

                var result = _approvalService.ProcessApproval(id, request.Status, request.Keterangan ?? "");

                if (!result.IsSuccess)
                    return BadRequest(new { success = false, message = result.ErrorMessage });

                return Ok(new { success = true, data = result.Data, message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete approval")]
        public ActionResult DeleteApproval(string id)
        {
            try
            {
                var result = _approvalService.DeleteApproval(id);

                if (!result.IsSuccess)
                    return BadRequest(new { success = false, message = result.ErrorMessage });

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}