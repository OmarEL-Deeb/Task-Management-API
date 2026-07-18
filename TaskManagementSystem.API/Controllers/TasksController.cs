using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces.Services;

namespace TaskManagementSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    
    private int GetCurrentUserId()
    {
        var uid = User.FindFirst("uid")?.Value;
        return int.Parse(uid ?? "0");
    }

   
    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        
            var userId = GetCurrentUserId();
            var result = await _taskService.CreateTaskAsync(model, userId);

            return CreatedAtAction(nameof(GetTaskById), new { id = result.Id }, result);
        
        
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null) return NotFound();

        return Ok(task);
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] TaskParameters parameters)
    {
        var (tasks, totalCount) = await _taskService.GetTasksAsync(parameters);

        Response.Headers.Append("X-Total-Count", totalCount.ToString());

        return Ok(tasks);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

       
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            await _taskService.UpdateTaskAsync(id, model, userId, role);
            return NoContent(); 
        
      
        
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> DeleteTask(int id)
    {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            await _taskService.DeleteTaskAsync(id, userId, role);
            return NoContent();
        
        
    }
}