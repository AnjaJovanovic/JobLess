using System;
using System.Collections.Generic;
using MediatR;
using JobLess.Oglasi.Domain.Enums;

namespace JobLess.Oglasi.Application.Queries.PretraziOglase
{
    public class PretraziOglaseQuery : IRequest<PretraziOglaseResult>
    {
        public int? KompanijaId { get; init; }

        public string? Naslov { get; init; }
        public string? Pozicija { get; init; }

        public DateTime? DatumIstekaOd { get; init; }
        public DateTime? DatumIstekaDo { get; init; }

        public TipZaposlenja? TipZaposlenja { get; init; }
        public RadnoVreme? RadnoVreme { get; init; }
        public Senioritet? Senioritet { get; init; }

        public int? IskustvoMin { get; init; }
        public int? IskustvoMax { get; init; }

        public string? Grad { get; init; }
        public string? Drzava { get; init; }

        public TipRada? TipRada { get; init; }

        public decimal? PlataOd { get; init; }
        public decimal? PlataDo { get; init; }

        public string? Valuta { get; init; }
        public bool? PlataVidljiva { get; init; }
        public int BrojStranice { get; set; } = 1;
        public int VelicinaStranice { get; set; } = 10;

    }
}
