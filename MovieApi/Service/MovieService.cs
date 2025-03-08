using CsvHelper;
using System.Globalization;
using MovieApi.Models;
using CsvHelper.Configuration;
using MovieApi.Converters;

namespace MovieApi.Service;

public class MovieService : IMovieService
{
    private readonly ILogger<MovieService> _logger;
    private readonly List<MovieModel> _movies;

    public MovieService(string csvFilePath, ILogger<MovieService> logger)
    {
        _logger = logger;
        _movies = LoadMoviesFromCsv(csvFilePath);
    }

    private List<MovieModel> LoadMoviesFromCsv(string filePath)
    {
        _logger.LogInformation($"Loading movies from csv: {filePath}");

        var movies = new List<MovieModel>();

        using (var reader = new StreamReader(filePath))
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                BadDataFound = null
            };

            using (var csv = new CsvReader(reader, config))
            {
                //added a couple of converters due to empty value mapping errors
                csv.Context.TypeConverterCache.AddConverter<decimal>(new CustomDecimalConverter());
                csv.Context.TypeConverterCache.AddConverter<int>(new CustomIntConverter());
                csv.Context.TypeConverterCache.AddConverter<DateTime>(new CustomDateTimeConverter());

                csv.Context.RegisterClassMap<MovieMap>();

                try
                {
                    movies = csv.GetRecords<MovieModel>().ToList();

                    _logger.LogInformation($"Successfully loaded {movies.Count} movies from csv");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error reading csv: {ex.Message}");
                }
            }
        }

        return movies;
    }

    public MovieListResponseModel SearchMovies(GetMoviesModel model)
    {
        _logger.LogInformation($"Searching movies with parameters: title={model.Title}, Genre={model.Genre}, Language={model.Language}, Page={model.Page}, PageSize={model.PageSize}, SortBy={model.SortBy}, IsAscending={model.IsAscending}");

        //ideally would've been handled front end...
        //then would throw Api exceptions on these :)
        if (model.Page < 1)
        {
            _logger.LogDebug("Adjusting page from {OriginalPage} to 1", model.Page);
            model.Page = 1;
        }
        if (model.PageSize < 1)
        {
            _logger.LogDebug("Adjusting page size from {OriginalPageSize} to 10", model.PageSize);
            model.PageSize = 10;
        }
        if (model.PageSize > 100)
        {
            _logger.LogDebug("Adjusting page size from {OriginalPageSize} to 100", model.PageSize);
            model.PageSize = 100;
        }

        var query = _movies.AsQueryable();

        if (!string.IsNullOrEmpty(model.Title))
        {
            _logger.LogDebug("Filtering by title: {Title}", model.Title);
            query = query.Where(m => m.Title.Contains(model.Title, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(model.Genre))
        {
            _logger.LogDebug("Filtering by genre: {Genre}", model.Genre);
            query = query.Where(m => m.Genre.Contains(model.Genre, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(model.Language))
        {
            _logger.LogDebug("Filtering by language: {Language}", model.Language);
            query = query.Where(m => m.OriginalLanguage.Equals(model.Language, StringComparison.OrdinalIgnoreCase));
        }

        query = model.SortBy.ToLower() switch
        {
            "releasedate" => model.IsAscending ? query.OrderBy(m => m.ReleaseDate) : query.OrderByDescending(m => m.ReleaseDate),
            _ => model.IsAscending ? query.OrderBy(m => m.Title) : query.OrderByDescending(m => m.Title)
        };

        var paginatedMovieList = query
            .Skip((model.Page - 1) * model.PageSize)
            .Take(model.PageSize)
            .ToList();

        //to help display front end how many pagination buttons to display
        int totalCount = query.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)model.PageSize);

        _logger.LogInformation($"Search completed. Found {totalCount} movies, returning page {model.Page} of {totalPages}");

        return new MovieListResponseModel
        {
            Movies = paginatedMovieList,
            Page = model.Page,
            PageSize = model.PageSize,
            TotalMovies = totalCount,
            TotalPages = totalPages
        };
    }

    public List<string> GetUniqueLanguages()
    {
        _logger.LogInformation("Getting unique languages");

        var languages = _movies
            .Select(m => m.OriginalLanguage)
            .Distinct()
            .OrderBy(l => l)
            .ToList();

        _logger.LogInformation($"Found {languages.Count} unique languages");

        return languages;
    }

    public List<string> GetUniqueGenres()
    {
        _logger.LogInformation("Getting unique genres");

        var allGenres = new HashSet<string>();

        foreach (var movie in _movies)
        {
            if (string.IsNullOrEmpty(movie.Genre)) { continue; }

            var genres = movie.Genre.Split(',')
                .Select(g => g.Trim())
                .Where(g => !string.IsNullOrEmpty(g));

            foreach (var genre in genres)
            {
                allGenres.Add(genre);
            }
        }

        var orderedGenres = allGenres.OrderBy(g => g).ToList();

        _logger.LogInformation($"Found {orderedGenres.Count} unique genres");

        return orderedGenres;
    }
}