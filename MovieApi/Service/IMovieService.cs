using MovieApi.Models;

namespace MovieApi.Service;

public interface IMovieService
{
    MovieListResponseModel SearchMovies(GetMoviesModel model);
    List<string> GetUniqueLanguages();
    List<string> GetUniqueGenres();
}