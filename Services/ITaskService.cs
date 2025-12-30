using UserTaskManagerAPI.Models.DTOs;
using UserTaskManagerAPI.Models.Pagination;

namespace UserTaskManagerAPI.Services
{
    /// <summary>
    /// Interfaz que define las operaciones de lógica de negocio para tareas
    /// </summary>
    public interface ITaskService
    {
        Task<TaskDto?> GetTaskByIdAsync(int taskId);
        Task<PagedResult<TaskDto>> GetTasksByUserIdPagedAsync(int userId, PaginationQuery pagination);
        Task<IEnumerable<TaskDto>> GetTasksByUserIdAsync(int userId);
        Task<TaskDto> CreateTaskAsync(int userId, CreateTaskDto createTaskDto);
        Task<TaskDto?> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto);
        Task<TaskDto?> ToggleTaskCompletionAsync(int taskId);
        Task<bool> DeleteTaskAsync(int taskId);
    }
}