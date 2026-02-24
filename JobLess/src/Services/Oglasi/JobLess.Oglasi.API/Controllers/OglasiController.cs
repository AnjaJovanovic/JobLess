using JobLess.Oglasi.Application.Commands.AktivirajOglas;
using JobLess.Oglasi.Application.Commands.AzurirajOglas;
using JobLess.Oglasi.Application.Commands.IzbrisiOglas;
using JobLess.Oglasi.Application.Commands.KreirajOglas;
using JobLess.Oglasi.Application.Queries.PretraziOglase;
using JobLess.Oglasi.Application.Queries.VratiOglas;
using JobLess.Oglasi.Application.Queries.VratiSveOglase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MATFInfostud.Oglasi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OglasiController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OglasiController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> KreirajOglas([FromBody] KreirajOglasCommand command)
        {
            if (command == null)
                return BadRequest("Nevalidni podaci.");

            var oglasId = await _mediator.Send(command);

            return CreatedAtAction(nameof(KreirajOglas), new { id = oglasId }, oglasId);
        }

        [HttpDelete]
        public async Task<bool> IzbiriOglas([FromBody] IzbrisiOglasCommand command)
        {

            var uspesno = await _mediator.Send(command);

            return uspesno;
        }
        [HttpGet("SviOglasi")]
        public async Task<ActionResult<VratiSveOglaseResult>> VratiSveOglase([FromQuery] int brojStranice = 1, [FromQuery] int velicinaStranice = 10)
        {
            var query = new VratiSveOglaseQuery
            {
                BrojStranice = brojStranice,
                VelicinaStranice = velicinaStranice
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }
        [HttpGet("JedanOglas")]
        public async Task<ActionResult<VratiOglasResult>> VratiOglas([FromQuery] int id)
        {
            var query = new VratiOglasQuery
            {
                Id = id
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }
        [HttpGet("PretraziOglase")]
        public async Task<ActionResult<PretraziOglaseResult>> PretraziOglase([FromQuery] PretraziOglaseQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("AktivirajOglas")]
        public async Task<IActionResult> AktivirajOglas([FromQuery] AktivirajOglasCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result)
                return BadRequest(false);

            return Ok(true);
        }

        [HttpPut("AzurirajOglas")]
        public async Task<IActionResult> AzurirajOglas([FromBody] AzurirajOglasCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        //TODO: Izbaci polje plataVidljiva

    }
}
