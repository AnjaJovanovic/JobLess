using JobLess.Oglasi.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Oglasi.Application.Commands.AzurirajOglas
{
    public class AzurirajOglasCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
        public string? Naslov { get; set; }
        public string? Opis { get; set; }
        public string? Pozicija { get; set; }
        public DateTime? DatumIsteka { get; set; }
        public TipZaposlenja? TipZaposlenja { get; set; }
        public RadnoVreme? RadnoVreme { get; set; }
        public Senioritet? Senioritet { get; set; }
        public StatusOglasa? Status { get; set; }
        public int? IskustvoMin { get; set; }
        public int? IskustvoMax { get; set; }
        public string? Grad { get; init; }
        public string? Drzava { get; init; }
        public TipRada? TipRada { get; init; }
        public decimal? PlataOd { get; set; }
        public decimal? PlataDo { get; set; }
        public string? Valuta { get; set; }
        public bool? PlataVidljiva { get; set; }
    }
}