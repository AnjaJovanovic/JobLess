using JobLess.Advertisement.Application.Commands.Create;
using JobLess.Advertisement.Application.Commands.Update;
using JobLess.Advertisement.Application.Commands.Delete;
using JobLess.Advertisement.Application.Commands.Activate;
using JobLess.Advertisement.Application.Queries.Search;
using JobLess.Advertisement.Application.Queries.GetOne;
using JobLess.Advertisement.Application.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Jobless.Advertisement.Application.Queries.GetAllForCompany;

namespace JobLess.Advertisement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdvertisementsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdvertisementsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Create([FromBody] CreateAdvertisementCommand command)
        {
            if (command == null)
                return BadRequest("Nevalidni podaci.");

            var companyEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (companyEmail is null)
                return Unauthorized();

            command.CompanyEmail = companyEmail;

            var oglasId = await _mediator.Send(command);

            return CreatedAtAction(nameof(Create), new { id = oglasId }, oglasId);
        }

        [HttpDelete]
        [Authorize(Roles = "Company")]
        public async Task<bool> Delete([FromBody] DeleteAdvertismentCommand command)
        {
            var companyEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (companyEmail is null)
                return false;

            command.CompanyEmail = companyEmail;

            var success = await _mediator.Send(command);

            return success;
        }
        [HttpGet("All")]
        [Authorize(Roles = "Candidate")]
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
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<GetOneAdvertisementResult>> GetOne([FromQuery] int id)
        {
            var query = new GetOneAdvertisementQuery
            {
                Id = id
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }
        [HttpGet("Search")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<SearchAdvertisementResult>> Search([FromQuery] SearchAdvertisementQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("GetAdvertisementsForCompany")]
        [Authorize(Roles = "Company")]
        public async Task<ActionResult<GetAllForCompanyResult>> GetAdvertisementsForComapny([FromQuery] GetAllForCompanyQuery query)
        {
            var companyEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (companyEmail is null)
                return Unauthorized();

            query.CompanyEmail = companyEmail;

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("Activate")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Activate([FromQuery] ActivateAdvertisementCommand command)
        {
            var companyEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (companyEmail is null)
                return Unauthorized();

            command.CompanyEmail = companyEmail;

            var result = await _mediator.Send(command);

            if (!result)
                return BadRequest(false);

            return Ok(true);
        }

        [HttpPut("Update")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Update([FromBody] UpdateAdvertisementCommand command)
        {
            var companyEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (companyEmail is null)
                return Unauthorized();

            command.CompanyEmail = companyEmail;

            await _mediator.Send(command);
            return Ok();
        }

        //TODO: Izbaci polje plataVidljiva

    }
}