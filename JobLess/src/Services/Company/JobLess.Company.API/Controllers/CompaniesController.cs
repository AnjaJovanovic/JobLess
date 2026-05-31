using JobLess.Company.Application.Commands.Create;
using JobLess.Company.Application.Commands.Delete;
using JobLess.Company.Application.Commands.Update;
using JobLess.Company.Application.Queries.GetAll;
using JobLess.Company.Application.Queries.GetByName;
using JobLess.Company.Application.Queries.GetOne;
using JobLess.Company.Application.Queries.GetByName;
using JobLess.Company.Application.Queries.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobLess.Company.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CompaniesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCompanyCommand command)
        {
            if (command == null)
                return BadRequest("Podaci nisu validni.");

            var companyId = await _mediator.Send(command);
            return CreatedAtAction(nameof(Create), new { id = companyId }, companyId);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteCompanyCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound("Tražena kompanija ne postoji.");

            return Ok();
        }

        [HttpGet("All")]
        public async Task<ActionResult<GetAllCompaniesResult>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetAllCompaniesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("One")]
        public async Task<IActionResult> GetOne([FromQuery] int id)
        {
            var result = await _mediator.Send(new GetOneCompanyQuery { Id = id });

            if (result == null)
                return NotFound("Tražena kompanija ne postoji");

            return Ok(result);
        }

        [HttpGet("ByName")]
        public async Task<IActionResult> GetByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Naziv kompanije je obavezan.");

            var result = await _mediator.Send(new GetByNameCompanyQuery { Name = name });
            return Ok(result);
        }

        [HttpGet("Search")]
        public async Task<ActionResult<SearchCompanyResult>> Search([FromQuery] SearchCompanyQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateCompanyCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound("Tražena kompanija ne postoji");
            return Ok();
        }
    }
}
