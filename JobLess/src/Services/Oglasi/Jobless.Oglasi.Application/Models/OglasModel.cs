using System;
using System.Linq.Expressions;
using MATFInfostud.Oglasi.Domain.Entities;
using MATFInfostud.Oglasi.Domain.Enums;


namespace MATFInfostud.Oglasi.Application.Models
{
    public class OglasModel
    {
        public int Id { get; set; }
        public int KompanijaId { get; set; }

        public string Naslov { get; set; } = string.Empty;
        public string? Opis { get; set; }
        public string Pozicija { get; set; } = string.Empty;

        public StatusOglasa Status { get; set; }

        public DateTime DatumPostavljanja { get; set; }
        public DateTime? DatumIsteka { get; set; }
        public bool Aktivan { get; set; }

        public TipZaposlenja TipZaposlenja { get; set; }
        public RadnoVreme RadnoVreme { get; set; }
        public Senioritet Senioritet { get; set; }

        public int? IskustvoMin { get; set; }
        public int? IskustvoMax { get; set; }

        public string Grad { get; set; } = string.Empty;
        public string? Drzava { get; set; }
        public TipRada TipRada { get; set; }

        public decimal? PlataOd { get; set; }
        public decimal? PlataDo { get; set; }
        public string? Valuta { get; set; }
        public bool? PlataVidljiva { get; set; }

        public static Expression<Func<Oglas, OglasModel>> Projekcija
        {
            get
            {
                return entity => new OglasModel
                {
                    Id = entity.Id,
                    KompanijaId = entity.KompanijaId,

                    Naslov = entity.Naslov,
                    Opis = entity.Opis,
                    Pozicija = entity.Pozicija,

                    Status = entity.Status,
                    DatumPostavljanja = entity.DatumPostavljanja,
                    DatumIsteka = entity.DatumIsteka,
                    Aktivan = entity.Aktivan,

                    TipZaposlenja = entity.TipZaposlenja,
                    RadnoVreme = entity.RadnoVreme,
                    Senioritet = entity.Senioritet,

                    IskustvoMin = entity.IskustvoMin,
                    IskustvoMax = entity.IskustvoMax,

                    Grad = entity.Grad,
                    Drzava = entity.Drzava,
                    TipRada = entity.TipRada,

                    PlataOd = entity.PlataOd,
                    PlataDo = entity.PlataDo,
                    Valuta = entity.Valuta,
                    PlataVidljiva = entity.PlataVidljiva
                };
            }
        }
    }
}

