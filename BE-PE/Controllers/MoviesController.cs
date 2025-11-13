using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE_PE.Data;
using BE_PE.Models;

namespace BE_PE.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies(
            [FromQuery] string? search,
            [FromQuery] string? genre,
            [FromQuery] string? sortBy = "title",
            [FromQuery] string? sortOrder = "asc")
        {
            var query = _context.Movies.AsQueryable();

            // Search by title
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m => m.Title.ToLower().Contains(search.ToLower()));
            }

            // Filter by genre
            if (!string.IsNullOrWhiteSpace(genre))
            {
                query = query.Where(m => m.Genre != null && m.Genre.ToLower() == genre.ToLower());
            }

            // Sort
            query = sortBy?.ToLower() switch
            {
                "rating" => sortOrder?.ToLower() == "desc" 
                    ? query.OrderByDescending(m => m.Rating) 
                    : query.OrderBy(m => m.Rating),
                "title" => sortOrder?.ToLower() == "desc" 
                    ? query.OrderByDescending(m => m.Title) 
                    : query.OrderBy(m => m.Title),
                _ => query.OrderBy(m => m.Title)
            };

            return await query.ToListAsync();
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound(new { message = "Movie not found" });
            }

            return movie;
        }

        // GET: api/Movies/genres
        [HttpGet("genres")]
        public async Task<ActionResult<IEnumerable<string>>> GetGenres()
        {
            var genres = await _context.Movies
                .Where(m => m.Genre != null)
                .Select(m => m.Genre!)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            return genres;
        }

        // PUT: api/Movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var existingMovie = await _context.Movies.FindAsync(id);
            if (existingMovie == null)
            {
                return NotFound(new { message = "Movie not found" });
            }

            existingMovie.Title = movie.Title;
            existingMovie.Genre = movie.Genre;
            existingMovie.Rating = movie.Rating;
            existingMovie.PosterUrl = movie.PosterUrl;
            existingMovie.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound(new { message = "Movie not found" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Movies
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            movie.CreatedAt = DateTime.UtcNow;
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound(new { message = "Movie not found" });
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
