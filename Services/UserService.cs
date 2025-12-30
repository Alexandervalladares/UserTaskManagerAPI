using Microsoft.EntityFrameworkCore;
using UserTaskManagerAPI.Data;
using UserTaskManagerAPI.Models.DTOs;
using UserTaskManagerAPI.Models.Entities;
using UserTaskManagerAPI.Models.Pagination;
using UserTaskManagerAPI.Repositories;

namespace UserTaskManagerAPI.Services
{
    /// <summary>
    /// Servicio que implementa la lógica de negocio para la gestión de usuarios
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ApplicationDbContext _context;

        public UserService(IUserRepository userRepository, ITaskRepository taskRepository, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _taskRepository = taskRepository;
            _context = context;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;

            return MapToDto(user);
        }

        public async Task<PagedResult<UserDto>> GetAllUsersPagedAsync(PaginationQuery pagination)
        {
            var query = _context.Users
                .Include(u => u.Tasks)
                .OrderBy(u => u.FullName);

            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var userDtos = users.Select(MapToDto).ToList();

            return new PagedResult<UserDto>
            {
                Items = userDtos,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto).ToList();
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(createUserDto.EmailAddress);
            if (existingUser != null)
            {
                throw new InvalidOperationException(
                    $"Ya existe un usuario registrado con el correo '{createUserDto.EmailAddress}'");
            }

            var userEntity = new UserEntity
            {
                FullName = createUserDto.FullName.Trim(),
                EmailAddress = createUserDto.EmailAddress.Trim().ToLower(),
                RegistrationDate = DateTime.Now
            };

            var createdUser = await _userRepository.CreateAsync(userEntity);
            return MapToDto(createdUser);
        }

        public async Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;

            if (!string.IsNullOrWhiteSpace(updateUserDto.FullName))
            {
                user.FullName = updateUserDto.FullName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.EmailAddress))
            {
                var newEmail = updateUserDto.EmailAddress.Trim().ToLower();

                if (newEmail != user.EmailAddress.ToLower())
                {
                    var emailExists = await _userRepository.ExistsByEmailAsync(newEmail);
                    if (emailExists)
                    {
                        throw new InvalidOperationException(
                            $"El correo '{newEmail}' ya está registrado por otro usuario");
                    }
                    user.EmailAddress = newEmail;
                }
            }

            var updatedUser = await _userRepository.UpdateAsync(user);
            return MapToDto(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            return await _userRepository.DeleteAsync(userId);
        }

        private UserDto MapToDto(UserEntity user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                EmailAddress = user.EmailAddress,
                RegistrationDate = user.RegistrationDate,
                TotalTasks = user.Tasks?.Count ?? 0,
                CompletedTasks = user.Tasks?.Count(t => t.IsCompleted) ?? 0
            };
        }
    }
}