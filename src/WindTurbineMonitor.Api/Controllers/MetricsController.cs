using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WindTurbineMonitor.Api.Data;
using WindTurbineMonitor.Api.Dtos;

namespace WindTurbineMonitor.Api.Controllers;

[ApiController]
[Route("api/turbines/{turbineId}/metrics")]
public class MetricsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MetricDto>>> GetMetrics(
        string turbineId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int limit = 100)
    {
        var query = db.TurbineMetrics.AsQueryable();

        if (from.HasValue)
            query = query.Where(m => m.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(m => m.Timestamp <= to.Value);

        var metrics = await query
            .Where(m => m.TurbineId == turbineId)
            .OrderByDescending(m => m.Timestamp)
            .Take(limit)
            .Select(m => new MetricDto(
                m.Id, m.TurbineId, m.Timestamp,
                m.RotorRpm, m.PowerOutputKw,
                m.WindSpeedMs, m.WindDirectionDeg,
                m.NacelleTemperatureCelsius, m.GearboxTemperatureCelsius,
                m.Status.ToString()))
            .ToListAsync();

        return Ok(metrics);
    }

    [HttpGet("latest")]
    public async Task<ActionResult<MetricDto>> GetLatestMetric(string turbineId)
    {
        var metric = await db.TurbineMetrics
            .Where(m => m.TurbineId == turbineId)
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefaultAsync();

        if (metric == null)
            return NotFound();

        var dto = new MetricDto(
            metric.Id, metric.TurbineId, metric.Timestamp,
            metric.RotorRpm, metric.PowerOutputKw,
            metric.WindSpeedMs, metric.WindDirectionDeg,
            metric.NacelleTemperatureCelsius, metric.GearboxTemperatureCelsius,
            metric.Status.ToString());

        return Ok(dto);
    }
}
