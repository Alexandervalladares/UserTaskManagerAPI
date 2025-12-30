using System.ComponentModel.DataAnnotations;

namespace UserTaskManagerAPI.Models.Pagination
{
    /// <summary>
    /// Parámetros de filtro para listados paginados
    /// </summary>
    public class FilterQuery
    {
        /// <summary>
        /// Texto de búsqueda parcial (nombre, email o descripción)
        /// Máximo 100 caracteres para evitar consultas pesadas
        /// </summary>
        [StringLength(100, ErrorMessage = "La búsqueda no puede exceder 100 caracteres")]
        public string? Search { get; set; }

        /// <summary>
        /// Filtro por estado de tarea: "completed", "pending" o "all" (por defecto)
        /// Solo aplica en listado de tareas
        /// </summary>
        public string Status { get; set; } = "all";
    }
}