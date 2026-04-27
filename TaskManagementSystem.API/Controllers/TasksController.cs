using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces.Services;

namespace TaskManagementSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // السطر دا بيحمي كل الـ Endpoints، ومحدش يقدر يدخلها من غير Token
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    // --- Helper Methods ---
    // دالة لاستخراج ID المستخدم من التوكن
    private int GetCurrentUserId()
    {
        var uid = User.FindFirst("uid")?.Value;
        return int.Parse(uid ?? "0");
    }

    // دالة لاستخراج Role المستخدم من التوكن
    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
    // ----------------------

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.CreateTaskAsync(model, userId);

            // 201 Created
            return CreatedAtAction(nameof(GetTaskById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
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

        // Best Practice: بنضيف عدد العناصر الكلي في الـ Header عشان الـ Frontend يعرف يعمل Pagination
        Response.Headers.Append("X-Total-Count", totalCount.ToString());

        return Ok(tasks);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            await _taskService.UpdateTaskAsync(id, model, userId, role);
            return NoContent(); // 204 No Content (Standard for successful PUT)
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid(); // 403 Forbidden (ليس لديك صلاحية)
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // حماية إضافية: الـ Endpoint دي للإدمن فقط
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            await _taskService.DeleteTaskAsync(id, userId, role);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}