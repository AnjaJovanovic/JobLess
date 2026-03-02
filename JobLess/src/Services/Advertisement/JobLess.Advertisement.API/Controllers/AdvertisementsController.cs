using JobLess.Advertisement.Application.Commands.Create;
using JobLess.Advertisement.Application.Commands.Update;
using JobLess.Advertisement.Application.Commands.Delete;
using JobLess.Advertisement.Application.Commands.Activate;
using JobLess.Advertisement.Application.Queries.Search;
using JobLess.Advertisement.Application.Queries.GetOne;
using JobLess.Advertisement.Application.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobLess.Advertisement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvertisementsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdvertisementsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdvertisementCommand command)
        {
            if (command == null)
                return BadRequest("Nevalidni podaci.");

            var oglasId = await _mediator.Send(command);

            return CreatedAtAction(nameof(Create), new { id = oglasId }, oglasId);
        }

        [HttpDelete]
        public async Task<bool> Delete([FromBody] DeleteAdvertismentCommand command)
        {

            var success = await _mediator.Send(command);

            return success;
        }
        [HttpGet("All")]
        public async Task<ActionResult<GetAllAdvertisementResult>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllAdvertisementQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }
        [HttpGet("One")]
        public async Task<ActionResult<GetOneAdvertisementResult>> GetOne([FromQuery] int id)
        {
            var query = new GetOneAdvertisementQuery
            {
                Id = id
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }
        //TODO: ne radi ocekivano
        [HttpGet("Search")]
        public async Task<ActionResult<SearchAdvertisementResult>> Search([FromQuery] SearchAdvertisementQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("Activate")]
        public async Task<IActionResult> Activate([FromQuery] ActivateAdvertisementCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result)
                return BadRequest(false);

            return Ok(true);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateAdvertisementCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        //TODO: Izbaci polje plataVidljiva

    }
}
