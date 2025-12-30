using Microsoft.EntityFrameworkCore;
using UserTaskManagerAPI.Data;
using UserTaskManagerAPI.Models.DTOs;
using UserTaskManagerAPI.Models.Entities;
using UserTaskManagerAPI.Models.Pagination;
using UserTaskManagerAPI.Repositories;

namespace UserTaskManagerAPI.Services
{
    /// <summary>
    /// Servicio que implementa la lógica de negocio para la gestión de tareas
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository, ApplicationDbContext context)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                return null;

            return MapToDto(task);
        }

        public async Task<PagedResult<TaskDto>> GetTasksByUserIdPagedAsync(int userId, PaginationQuery pagination)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"No existe un usuario con ID {userId}");
            }

            var query = _context.Tasks
                .Include(t => t.User)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreationDate);

            var totalCount = await query.CountAsync();

            var tasks = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var taskDtos = tasks.Select(MapToDto).ToList();

            return new PagedResult<TaskDto>
            {
                Items = taskDtos,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<TaskDto>> GetTasksByUserIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"No existe un usuario con ID {userId}");
            }

            var tasks = await _taskRepository.GetByUserIdAsync(userId);
            return tasks.Select(MapToDto).ToList();
        }

        public async Task<TaskDto> CreateTaskAsync(int userId, CreateTaskDto createTaskDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"No existe un usuario con ID {userId}");
            }

            var taskEntity = new TaskEntity
            {
                TaskDescription = createTaskDto.TaskDescription.Trim(),
                IsCompleted = false,
                CreationDate = DateTime.Now,
                CompletionDate = null,
                UserId = userId
            };

            var createdTask = await _taskRepository.CreateAsync(taskEntity);
            return MapToDto(createdTask);
        }

        public async Task<TaskDto?> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                return null;

            if (!string.IsNullOrWhiteSpace(updateTaskDto.TaskDescription))
            {
                task.TaskDescription = updateTaskDto.TaskDescription.Trim();
            }

            if (updateTaskDto.IsCompleted.HasValue)
            {
                task.IsCompleted = updateTaskDto.IsCompleted.Value;

                if (task.IsCompleted)
                {
                    task.CompletionDate = DateTime.Now;
                }
                else
                {
                    task.CompletionDate = null;
                }
            }

            var updatedTask = await _taskRepository.UpdateAsync(task);
            return MapToDto(updatedTask);
        }

        public async Task<TaskDto?> ToggleTaskCompletionAsync(int taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                return null;

            task.IsCompleted = !task.IsCompleted;

            if (task.IsCompleted)
            {
                task.CompletionDate = DateTime.Now;
            }
            else
            {
                task.CompletionDate = null;
            }

            var updatedTask = await _taskRepository.UpdateAsync(task);
            return MapToDto(updatedTask);
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            return await _taskRepository.DeleteAsync(taskId);
        }

        private TaskDto MapToDto(TaskEntity task)
        {
            return new TaskDto
            {
                TaskId = task.TaskId,
                TaskDescription = task.TaskDescription,
                IsCompleted = task.IsCompleted,
                CreationDate = task.CreationDate,
                CompletionDate = task.CompletionDate,
                UserId = task.UserId,
                UserFullName = task.User?.FullName ?? string.Empty
            };
        }
    }
}