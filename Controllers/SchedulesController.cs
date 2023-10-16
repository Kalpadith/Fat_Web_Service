/*
 * This controller is used to manage the schedules
 */
using Service.Models;
using Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Service.Controllers
{
    // Defining the Schedule API controller
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;

        public SchedulesController(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

    
     // This method handles the retrieval of all Schedules.
     
        [HttpGet]
        [Authorize(Roles = "agent,travelller,office")]
        public async Task<ActionResult<IEnumerable<Schedule>>> Get()
        {
            var schedules = await _scheduleService.GetAsync();
            return Ok(schedules);
        }

    
     // This method handles the retrieval of a schedule by its ID.
     
        [HttpGet("{ScheduleId}")]
        [Authorize(Roles = "agent,travelller,office")]
        public async Task<ActionResult<Schedule>> Get(string ScheduleId)
        {
            var schedule = await _scheduleService.GetAsync(ScheduleId);

            if (schedule is null)
            {
                return NotFound();
            }

            return Ok(schedule);
        }

    
     // This method handles the creation of a new Schedule.
    
        [HttpPost]
        [Authorize(Roles = "office")]
        public async Task<IActionResult> Post(Schedule newSchedule)
        {
            await _scheduleService.CreateAsync(newSchedule);
            return CreatedAtAction(nameof(Get), new { ScheduleId = newSchedule.ScheduleId }, newSchedule);
        }

   
    // This method handles the update of an existing Schedule.
    
        [HttpPut("{ScheduleId}")]
        [Authorize(Roles = "office")]
        public async Task<IActionResult> Put(string ScheduleId, Schedule updatedSchedule)
        {
            var schedule = await _scheduleService.GetAsync(ScheduleId);

            if (schedule is null)
            {
                return NotFound();
            }

            await _scheduleService.UpdateAsync(ScheduleId, updatedSchedule);
            return NoContent();
        }

   
     // This method handles the deletion of an existing schedule.
        [HttpDelete("{ScheduleId}")]
        [Authorize(Roles = "office")]
        public async Task<IActionResult> Delete(string ScheduleId)
        {
            var schedule = await _scheduleService.GetAsync(ScheduleId);

            if (schedule is null)
            {
                return NotFound();
            }

            await _scheduleService.RemoveAsync(ScheduleId);
            return NoContent();
        }
    }
}
