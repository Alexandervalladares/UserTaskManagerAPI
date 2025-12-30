namespace UserTaskManagerAPI.Models
{
    /// <summary>
    /// Parámetros para requests de paginación
    /// </summary>
    public class PaginationQueryDto
    {
        private int _page = 1;
        private int _pageSize = 10;

        /// <summary>
        /// Número de página (mínimo 1)
        /// </summary>
        public int Page
        {
            get => _page;
            set => _page = Math.Max(1, value);
        }

        /// <summary>
        /// Tamaño de página (entre 1 y 50)
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Clamp(value, 1, 50); // Límite razonable
        }
    }
}