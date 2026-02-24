using JobLess.Oglasi.Domain.Enums;
using JobLess.Shared.Domain.Common;
namespace JobLess.Shared.Domain;
public class Oglas : BaseEntity
{
    
    public int KompanijaId { get; set; }

    public required string Naslov { get; set; }
    public string? Opis { get; set; }
    public required string Pozicija { get; set; }

    public required StatusOglasa Status { get; set; }

    public required DateTime DatumPostavljanja { get; set; }
    public DateTime? DatumIsteka { get; set; }
    public required bool Aktivan { get; set; }

    public required TipZaposlenja TipZaposlenja { get; set; }
    public required RadnoVreme RadnoVreme { get; set; }
    public required Senioritet Senioritet { get; set; }

    public int? IskustvoMin { get; set; }
    public int? IskustvoMax { get; set; }

    public required string Grad { get; set; }
    public string? Drzava { get; set; }
    public required TipRada TipRada { get; set; }

    public decimal? PlataOd { get; set; }
    public decimal? PlataDo { get; set; }
    public string? Valuta { get; set; }
    public bool? PlataVidljiva { get; set; }

}
