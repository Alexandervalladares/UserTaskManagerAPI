using UserTaskManagerAPI.Models.DTOs;
using UserTaskManagerAPI.Models.Pagination;

namespace UserTaskManagerAPI.Services
{
    /// <summary>
    /// Interfaz que define las operaciones de lógica de negocio para usuarios
    /// </summary>
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<PagedResult<UserDto>> GetAllUsersPagedAsync(PaginationQuery pagination);
        Task<IEnumerable<UserDto>> GetAllUsersAsync(); // Mantenido por compatibilidad
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int userId);
    }
}