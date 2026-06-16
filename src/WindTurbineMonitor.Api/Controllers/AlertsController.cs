using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WindTurbineMonitor.Api.Data;
using WindTurbineMonitor.Api.Dtos;

namespace WindTurbineMonitor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlertDto>>> GetAlerts(
        [FromQuery] string? turbineId,
        [FromQuery] bool unacknowledgedOnly = false,
        [FromQuery] int limit = 50)
    {
        var query = db.Alerts.AsQueryable();

        if (!string.IsNullOrEmpty(turbineId))
            query = query.Where(a => a.TurbineId == turbineId);

        if (unacknowledgedOnly)
            query = query.Where(a => !a.IsAcknowledged);

        var alerts = await query
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .Select(a => new AlertDto(
                a.Id, a.TurbineId, a.Timestamp,
                a.Severity.ToString(), a.Title, a.Description,
                a.IsAcknowledged, a.AcknowledgedAt))
            .ToListAsync();

        return Ok(alerts);
    }

    [HttpPatch("{id}/acknowledge")]
    public async Task<IActionResult> AcknowledgeAlert(int id)
    {
        var alert = await db.Alerts.FindAsync(id);
        if (alert == null)
            return NotFound();

        alert.IsAcknowledged = true;
        alert.AcknowledgedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return NoContent();
    }
}
