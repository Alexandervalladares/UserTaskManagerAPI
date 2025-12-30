using System.ComponentModel.DataAnnotations;

namespace UserTaskManagerAPI.Models.DTOs
{
    /// <summary>
    /// DTO para la respuesta de datos de tarea
    /// Incluye información completa de la tarea y su usuario asignado
    /// </summary>
    public class TaskDto
    {
        public int TaskId { get; set; }
        public string TaskDescription { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para crear una nueva tarea
    /// Solo requiere la descripción, el resto se establece automáticamente
    /// </summary>
    public class CreateTaskDto
    {
        /// <summary>
        /// Descripción de la tarea a crear
        /// Debe tener entre 5 y 500 caracteres
        /// </summary>
        [Required(ErrorMessage = "La descripción de la tarea es obligatoria")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 500 caracteres")]
        public string TaskDescription { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para actualizar una tarea existente
    /// Permite modificar la descripción y/o el estado de completitud
    /// </summary>
    public class UpdateTaskDto
    {
        /// <summary>
        /// Nueva descripción de la tarea
        /// Si se proporciona, debe cumplir las validaciones
        /// </summary>
        [StringLength(500, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 500 caracteres")]
        public string? TaskDescription { get; set; }

        /// <summary>
        /// Nuevo estado de completitud de la tarea
        /// Si cambia a true, se registra automáticamente la fecha de completitud
        /// </summary>
        public bool? IsCompleted { get; set; }
    }
}