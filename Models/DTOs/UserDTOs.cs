using System.ComponentModel.DataAnnotations;

namespace UserTaskManagerAPI.Models.DTOs
{
    /// <summary>
    /// DTO para la respuesta de datos de usuario
    /// Incluye información completa del usuario sin exponer datos sensibles
    /// </summary>
    public class UserDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
    }

    /// <summary>
    /// DTO para crear un nuevo usuario
    /// Contiene solo los campos necesarios para el registro
    /// </summary>
    public class CreateUserDto
    {
        /// <summary>
        /// Nombre completo del usuario a registrar
        /// Debe tener entre 2 y 200 caracteres
        /// </summary>
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 200 caracteres")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del usuario
        /// Debe ser único en el sistema y tener formato válido
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        [StringLength(150, ErrorMessage = "El correo no puede exceder 150 caracteres")]
        public string EmailAddress { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para actualizar datos de un usuario existente
    /// Permite modificar nombre y/o email
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// Nuevo nombre completo del usuario
        /// Si se proporciona, debe cumplir las validaciones
        /// </summary>
        [StringLength(200, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 200 caracteres")]
        public string? FullName { get; set; }

        /// <summary>
        /// Nuevo correo electrónico del usuario
        /// Si se proporciona, debe ser válido y único
        /// </summary>
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        [StringLength(150, ErrorMessage = "El correo no puede exceder 150 caracteres")]
        public string? EmailAddress { get; set; }
    }
}