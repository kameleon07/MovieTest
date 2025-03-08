using Microsoft.AspNetCore.Mvc;
using MovieApi.Models;
using MovieApi.Service;

namespace MovieApi.Controllers;

//ideally we'd want to add [Authorize]
//and add some roles/permissions for each endpoint so it's secure
[ApiController]
[Route("api/[controller]")]
public class MoviesController(IMovieService movieService, ILogger<MoviesController> logger) : ControllerBase
{
    private readonly IMovieService _movieService = movieService;
    private readonly ILogger<MoviesController> _logger = logger;

    [HttpGet]
    public ActionResult<MovieListResponseModel> GetMovies([FromQuery] GetMoviesModel model)
    {
        try
        {
            _logger.LogInformation($"GetMovies begin");

            //typically this would be an async
            //but since it's just local csv, there was no need
            var response = _movieService.SearchMovies(model);

            _logger.LogInformation($"GetMovies completed successfuly");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"GetMovies error: {ex.Message}");

            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("languages")]
    public ActionResult<List<string>> GetLanguages()
    {
        try
        {
            _logger.LogInformation($"GetLanguages begin");

            return Ok(_movieService.GetUniqueLanguages());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"GetLanguages error: {ex.Message}");

            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("genres")]
    public ActionResult<List<string>> GetGenres()
    {
        try
        {
            _logger.LogInformation($"GetGenres begin");

            return Ok(_movieService.GetUniqueGenres());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"GetGenres error: {ex.Message}");

            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}