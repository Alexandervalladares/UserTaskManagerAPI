using Microsoft.AspNetCore.Mvc;
using UserTaskManagerAPI.Models;
using UserTaskManagerAPI.Models.DTOs;
using UserTaskManagerAPI.Models.Pagination;
using UserTaskManagerAPI.Services;

namespace UserTaskManagerAPI.Controllers
{
    /// <summary>
    /// Controlador REST para la gestión de tareas
    /// Proporciona endpoints para operaciones CRUD sobre tareas
    /// </summary>
    [Route("api/task")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(ITaskService taskService, ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene los detalles de una tarea específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskDto>> GetTaskById(int id)
        {
            try
            {
                _logger.LogInformation("Consultando tarea con ID: {TaskId}", id);

                var task = await _taskService.GetTaskByIdAsync(id);

                if (task == null)
                {
                    _logger.LogWarning("Tarea con ID {TaskId} no encontrada", id);
                    return NotFound(new ErrorResponse
                    {
                        Message = $"No se encontró una tarea con ID {id}"
                    });
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tarea con ID {TaskId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// Obtiene todas las tareas asociadas a un usuario específico (paginadas)
        /// </summary>
        /// <param name="userId">ID del usuario propietario</param>
        /// <param name="page">Número de página (inicia en 1)</param>
        /// <param name="pageSize">Cantidad de tareas por página (máximo 50)</param>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(PagedResult<TaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<TaskDto>>> GetTasksByUserId(
            int userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var pagination = new PaginationQuery { Page = page, PageSize = pageSize };

            try
            {
                _logger.LogInformation("Consultando tareas paginadas del usuario {UserId} - Página: {Page}, Tamaño: {PageSize}",
                    userId, pagination.Page, pagination.PageSize);

                var result = await _taskService.GetTasksByUserIdPagedAsync(userId, pagination);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Usuario con ID {UserId} no encontrado", userId);
                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tareas paginadas del usuario {UserId}", userId);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// Crea una nueva tarea asociada a un usuario
        /// </summary>
        [HttpPost("user/{userId}")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskDto>> CreateTask(int userId, [FromBody] CreateTaskDto createTaskDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de entrada inválidos",
                        Details = ModelState
                    });
                }

                _logger.LogInformation("Creando nueva tarea para usuario con ID: {UserId}", userId);

                var createdTask = await _taskService.CreateTaskAsync(userId, createTaskDto);

                _logger.LogInformation("Tarea creada exitosamente con ID: {TaskId}", createdTask.TaskId);

                return CreatedAtAction(
                    nameof(GetTaskById),
                    new { id = createdTask.TaskId },
                    createdTask);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error al crear tarea para usuario {UserId}", userId);
                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear tarea para usuario {UserId}", userId);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// Actualiza los datos de una tarea existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskDto>> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de entrada inválidos",
                        Details = ModelState
                    });
                }

                _logger.LogInformation("Actualizando tarea con ID: {TaskId}", id);

                var updatedTask = await _taskService.UpdateTaskAsync(id, updateTaskDto);

                if (updatedTask == null)
                {
                    _logger.LogWarning("Tarea con ID {TaskId} no encontrada para actualizar", id);
                    return NotFound(new ErrorResponse
                    {
                        Message = $"No se encontró una tarea con ID {id}"
                    });
                }

                _logger.LogInformation("Tarea con ID {TaskId} actualizada exitosamente", id);
                return Ok(updatedTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar tarea con ID {TaskId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// Alterna el estado de completitud de una tarea
        /// </summary>
        [HttpPatch("{id}/complete")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskDto>> ToggleTaskCompletion(int id)
        {
            try
            {
                _logger.LogInformation("Alternando estado de completitud de tarea con ID: {TaskId}", id);

                var updatedTask = await _taskService.ToggleTaskCompletionAsync(id);

                if (updatedTask == null)
                {
                    _logger.LogWarning("Tarea con ID {TaskId} no encontrada", id);
                    return NotFound(new ErrorResponse
                    {
                        Message = $"No se encontró una tarea con ID {id}"
                    });
                }

                var status = updatedTask.IsCompleted ? "completada" : "pendiente";
                _logger.LogInformation("Tarea con ID {TaskId} marcada como {Status}", id, status);

                return Ok(updatedTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al alternar estado de tarea con ID {TaskId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// Elimina una tarea del sistema
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando tarea con ID: {TaskId}", id);

                var deleted = await _taskService.DeleteTaskAsync(id);

                if (!deleted)
                {
                    _logger.LogWarning("Tarea con ID {TaskId} no encontrada para eliminar", id);
                    return NotFound(new ErrorResponse
                    {
                        Message = $"No se encontró una tarea con ID {id}"
                    });
                }

                _logger.LogInformation("Tarea con ID {TaskId} eliminada exitosamente", id);
                return Ok(new { success = true, message = $"Tarea con ID {id} eliminada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tarea con ID {TaskId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error interno del servidor"
                });
            }
        }
    }
}