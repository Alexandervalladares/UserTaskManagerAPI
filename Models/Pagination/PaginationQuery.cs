namespace UserTaskManagerAPI.Models.Pagination
{
    /// <summary>
    /// Parámetros de paginación desde query string
    /// </summary>
    public class PaginationQuery
    {
        private int _page = 1;
        private int _pageSize = 10;
        private const int MaxPageSize = 50;

        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 10 : value);
        }
    }
}