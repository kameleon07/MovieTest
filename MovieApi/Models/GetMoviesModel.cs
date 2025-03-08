namespace MovieApi.Models
{
    public class GetMoviesModel
    {
        public string? Title { get; set; }
        public string? Genre { get; set; }
        public string? Language { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "Title";
        public bool IsAscending { get; set; } = true;
    }
}
