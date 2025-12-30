namespace UserTaskManagerAPI.Models
{
    /// <summary>
    /// Respuesta estándar para errores de la API
    /// </summary>
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public object? Details { get; set; }
    }
}