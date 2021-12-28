using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Access the amazing world of Whisky.
/// </summary>
[ApiController]
[Route("whisky")]
public class WhiskyController : Controller
{
    private IWhiskyRepository _whiskyRepository;

    public WhiskyController(IWhiskyRepository whiskyRepository)
    {
        _whiskyRepository = whiskyRepository;
    }

    /// <summary>
    /// Returns all the whisky in the database.
    /// </summary>
    /// <param name="pageNumber">Which page number should the result return. Default is page 0.</param>
    /// <param name="pageSize">The number of elements returned in the page. Default is 100. </param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Whisky>))]
    public IActionResult GetAllWhisky([FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 100)
    {
        var whiskies = _whiskyRepository.GetAll(pageNumber, pageSize);
        return Ok(whiskies);
    }

    /// <summary>
    /// Get a particular whisky by its id.
    /// </summary>
    /// <param name="id">Id of the whisky (GUID)</param>
    /// <returns>The requested whisky</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Whisky))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetWhiskeyById(Guid id)
    {
        var whisky = _whiskyRepository.GetById(id);
        if (whisky == null) return NotFound();

        return Ok(whisky);
    }

    /// <summary>
    /// Adds a new whisky to the database.
    /// </summary>
    /// <param name="whisky">The whisky to add</param>
    /// <returns>The newly created whisky</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Whisky))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Add([FromBody] Whisky whisky)
    {
        if (whisky == null) return BadRequest();
        if (string.IsNullOrWhiteSpace(whisky.Name)) return BadRequest("Whisky name is required");
        if (string.IsNullOrWhiteSpace(whisky.RegionStyle)) return BadRequest("Region or style is required");

        _whiskyRepository.Add(whisky);
        return CreatedAtAction(nameof(GetWhiskeyById), new { id = whisky.Id }, whisky);
    }

    /// <summary>
    /// Get a list of regions or styles from the database.
    /// </summary>
    /// <returns></returns>
    [HttpGet("regions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
    public IActionResult Regions()
    {
        return Ok(_whiskyRepository.GetAll().Select(p => p.RegionStyle).Distinct().OrderBy(p => p));
    }
}