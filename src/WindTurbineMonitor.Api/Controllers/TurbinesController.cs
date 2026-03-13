using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WindTurbineMonitor.Api.Data;
using WindTurbineMonitor.Api.Dtos;

namespace WindTurbineMonitor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TurbinesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TurbineDto>>> GetTurbines()
    {
        var turbines = await db.Turbines
            .OrderBy(t => t.Id)
            .Select(t => new TurbineDto(t.Id, t.Name, t.Location, t.InstalledAt))
            .ToListAsync();

        return Ok(turbines);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TurbineDto>> GetTurbine(int id)
    {
        var turbine = await db.Turbines.FindAsync(id);
        if (turbine == null)
            return NotFound();

        return Ok(new TurbineDto(turbine.Id, turbine.Name, turbine.Location, turbine.InstalledAt));
    }
}
