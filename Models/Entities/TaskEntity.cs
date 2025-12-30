using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserTaskManagerAPI.Models.Entities
{
    /// <summary>
    /// Entidad que representa una tarea asignada a un usuario
    /// Tabla: Tasks
    /// </summary>
    [Table("Tasks")]
    public class TaskEntity
    {
        /// <summary>
        /// Identificador único de la tarea (clave primaria autoincrementable)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }

        /// <summary>
        /// Descripción detallada de la tarea a realizar
        /// Campo requerido con longitud máxima de 500 caracteres
        /// </summary>
        [Required(ErrorMessage = "La descripción de la tarea es obligatoria")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 500 caracteres")]
        [Column(TypeName = "nvarchar(500)")]
        public string TaskDescription { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la tarea ha sido completada
        /// Valor por defecto: false (no completada)
        /// </summary>
        [Required]
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// Fecha y hora de creación de la tarea
        /// Se establece automáticamente al crear el registro
        /// </summary>
        [Required]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Fecha y hora de la última actualización de la tarea
        /// Se actualiza automáticamente en cada modificación
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Fecha y hora en que se completó la tarea
        /// Nullable - solo tiene valor cuando IsCompleted = true
        /// </summary>
        public DateTime? CompletionDate { get; set; }

        /// <summary>
        /// Identificador del usuario propietario de la tarea (clave foránea)
        /// Relación con la tabla Users
        /// </summary>
        [Required(ErrorMessage = "La tarea debe estar asignada a un usuario")]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        /// <summary>
        /// Propiedad de navegación hacia el usuario propietario de la tarea
        /// Permite acceder a los datos del usuario desde la tarea
        /// </summary>
        public virtual UserEntity User { get; set; } = null!;
    }
}