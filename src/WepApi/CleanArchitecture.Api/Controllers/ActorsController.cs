using System;
using System.Threading.Tasks;
using CleanArchitecture.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Web.Http;

namespace CleanArchitecture.Api.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly IActorService _actorService;

        public ActorsController (IActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpPost("sample")]
        public async Task<IActionResult> PostSampleData()
        {
            await _actorService.InsertManyAsync();

            return Ok(new { Result = "Data successfully registered with Elasticsearch" });
        }

        [HttpPost("exception")]
        public IActionResult PostException()
        {
            throw new Exception("Generate sample exception");
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _actorService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("name-match")]
        public async Task<IActionResult> GetByNameWithMatch([FromQuery] string name)
        {
            var result = await _actorService.GetByNameWithMatch(name);

            return Ok(result);
        }

        [HttpGet("name-age-match")]
        public async Task<IActionResult> GetByNameWithMatchAndAge([FromQuery] string name, [FromQuery] int age)
        {
            var result = await _actorService.GetByNameWithMatchAndAge(name, age);

            return Ok(result);
        }

        [HttpGet("name-multimatch")]
        public async Task<IActionResult> GetByNameAndDescriptionMultiMatch([FromQuery] string term)
        {
            var result = await _actorService.GetByNameAndDescriptionMultiMatch(term);

            return Ok(result);
        }

        [HttpGet("name-matchphrase")]
        public async Task<IActionResult> GetByNameWithMatchPhrase([FromQuery] string name)
        {
            var result = await _actorService.GetByNameWithMatchPhrase(name);

            return Ok(result);
        }

        [HttpGet("name-matchphraseprefix")]
        public async Task<IActionResult> GetByNameWithMatchPhrasePrefix([FromQuery] string name)
        {
            var result = await _actorService.GetByNameWithMatchPhrasePrefix(name);

            return Ok(result);
        }

        [HttpGet("name-term")]
        public async Task<IActionResult> GetByNameWithTerm([FromQuery] string name)
        {
            var result = await _actorService.GetByNameWithTerm(name);

            return Ok(result);
        }

        [HttpGet("name-wildcard")]
        public async Task<IActionResult> GetByNameWithWildcard([FromQuery] string name)
        {
            var result = await _actorService.GetByNameWithWildcard(name);

            return Ok(result);
        }

        [HttpGet("name-fuzzy")]
        public async Task<IActionResult> GetByNameWithFuzzy([FromQuery] string name)
        {
            var result = await _actorService.GetByNameWithFuzzy(name);

            return Ok(result);
        }

        [HttpGet("description-match")]
        public async Task<IActionResult> GetByDescriptionMatch([FromQuery] string description)
        {
            var result = await _actorService.GetByDescriptionMatch(description);

            return Ok(result);
        }

        [HttpGet("all-fields")]
        public async Task<IActionResult> SearchAllProperties([FromQuery] string term)
        {
            var result = await _actorService.SearchInAllFiels(term);

            return Ok(result);
        }

        [HttpGet("condiction")]
        public async Task<IActionResult> GetByCondictions([FromQuery] string name, [FromQuery] string description,
            [FromQuery] DateTime? birthdate)
        {
            var result = await _actorService.GetActorsCondition(name, description, birthdate);

            return Ok(result);
        }

        [HttpGet("term")]
        public async Task<IActionResult> GetByAllCondictions([FromQuery] string term)
        {
            var result = await _actorService.GetActorsAllCondition(term);

            return Ok(result);
        }

        [HttpGet("aggregation")]
        public async Task<IActionResult> GetActorsAggregation()
        {
            var result = await _actorService.GetActorsAggregation();

            return Ok(result);
        }
    }
}