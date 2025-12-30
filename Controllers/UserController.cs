using Microsoft.AspNetCore.Mvc;
using UserTaskManagerAPI.Models;
using UserTaskManagerAPI.Models.DTOs;
using UserTaskManagerAPI.Models.Pagination;
using UserTaskManagerAPI.Services;

namespace UserTaskManagerAPI.Controllers
{
    /// <summary>
    /// Controlador REST para la gestión de usuarios
    /// Proporciona endpoints para operaciones CRUD sobre usuarios
    /// </summary>
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene los detalles de un usuario específico por su ID
        /// </summary>
        /// <param name="id">ID del usuario a consultar</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            try
            {
                _logger.LogInformation("Consultando usuario con ID: {UserId}", id);

                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("Usuario con ID {UserId} no encontrado", id);
                    return NotFound(new ErrorResponse
                    {
                        Message = $"No se encontró un usuario con ID {id}"
                    });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con ID {UserId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// Obtiene la lista paginada de todos los usuarios registrados
        /// </summary>
        /// <param name="page">Número de página (inicia en 1)</param>
        /// <param name="pageSize">Cantidad de usuarios por página (máximo 50)</param>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<UserDto>>> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var pagination = new PaginationQuery { Page = page, PageSize = pageSize };

            try
            {
                _logger.LogInformation("Consultando usuarios paginados - Página: {Page}, Tamaño: {PageSize}",
                    pagination.Page, pagination.PageSize);

                var result = await _userService.GetAllUsersPagedAsync(pagination);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lista paginada de usuarios");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
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

                _logger.LogInformation("Creando nuevo usuario con email: {Email}", createUserDto.EmailAddress);

                var createdUser = await _userService.CreateUserAsync(createUserDto);

                _logger.LogInformation("Usuario creado exitosamente con ID: {UserId}", createdUser.UserId);

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = createdUser.UserId },
                    createdUser);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al crear usuario");
                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
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

                _logger.LogInformation("Actualizando usuario con ID: {UserId}", id);

                var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto);

                if (updatedUser == null)
                {
                    _logger.LogWarning("Usuario con ID {UserId} no encontrado para actualizar", id);
                    return NotFound(new ErrorResponse
                    {
                        Message = $"No se encontró un usuario con ID {id}"
                    });
                }

                _logger.LogInformation("Usuario con ID {UserId} actualizado exitosamente", id);
                return Ok(updatedUser);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar usuario {UserId}", id);
                return BadRequest(new ErrorResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario con ID {UserId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// Elimina un usuario y todas sus tareas asociadas del sistema
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando usuario con ID: {UserId}", id);

                var deleted = await _userService.DeleteUserAsync(id);

                if (!deleted)
                {
                    _logger.LogWarning("Usuario con ID {UserId} no encontrado para eliminar", id);
                    return NotFound(new ErrorResponse
                    {
                        Message = $"No se encontró un usuario con ID {id}"
                    });
                }

                _logger.LogInformation("Usuario con ID {UserId} eliminado exitosamente", id);
                return Ok(new { success = true, message = $"Usuario con ID {id} eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario con ID {UserId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error interno del servidor"
                });
            }
        }
    }
}