using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserTaskManagerAPI.Models.Entities
{
    /// <summary>
    /// Entidad que representa un usuario en el sistema
    /// Tabla: Users
    /// </summary>
    [Table("Users")]
    public class UserEntity
    {
        /// <summary>
        /// Identificador único del usuario (clave primaria autoincrementable)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        /// <summary>
        /// Nombre completo del usuario
        /// Campo requerido con longitud máxima de 200 caracteres
        /// </summary>
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 200 caracteres")]
        [Column(TypeName = "nvarchar(200)")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de correo electrónico del usuario
        /// Debe ser única en el sistema y cumplir formato de email válido
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        [StringLength(150, ErrorMessage = "El correo no puede exceder 150 caracteres")]
        [Column(TypeName = "nvarchar(150)")]
        public string EmailAddress { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora de registro del usuario en el sistema
        /// Se establece automáticamente al crear el registro
        /// </summary>
        [Required]
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Fecha y hora de la última actualización del usuario
        /// Se actualiza automáticamente en cada modificación
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Colección de tareas asociadas al usuario
        /// Relación uno a muchos (un usuario puede tener múltiples tareas)
        /// </summary>
        public virtual ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
    }
}