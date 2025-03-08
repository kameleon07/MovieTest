namespace MovieApi.Models
{
    public class MovieListResponseModel
    {
        public List<MovieModel>? Movies { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalMovies { get; set; }
        public int TotalPages { get; set; }
    }
}
