using JobLess.Oglasi.Application.Interfaces;
using JobLess.Oglasi.Application.Models;
using JobLess.Oglasi.Application.Queries.VratiSveOglase;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Oglasi.Application.Queries.PretraziOglase;


public class PretraziOglaseHandler : IRequestHandler<PretraziOglaseQuery, PretraziOglaseResult>
{
    private readonly IApplicationDbContext _context;

    public PretraziOglaseHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PretraziOglaseResult> Handle(
    PretraziOglaseQuery query,
    CancellationToken cancellationToken)
    {
        var oglasiUpit = _context.Oglasi
            .Where(x => x.Aktivan == true)
            .AsQueryable();

        // === FILTERI ===

        if (query.KompanijaId.HasValue)
            oglasiUpit = oglasiUpit.Where(x => x.KompanijaId == query.KompanijaId.Value);

        if (!string.IsNullOrWhiteSpace(query.Naslov))
            oglasiUpit = oglasiUpit.Where(x => x.Naslov.Contains(query.Naslov));

        if (!string.IsNullOrWhiteSpace(query.Pozicija))
            oglasiUpit = oglasiUpit.Where(x => x.Pozicija.Contains(query.Pozicija));

        if (query.DatumIstekaOd.HasValue)
            oglasiUpit = oglasiUpit.Where(x => x.DatumIsteka >= query.DatumIstekaOd.Value);

        if (query.DatumIstekaDo.HasValue)
            oglasiUpit = oglasiUpit.Where(x => x.DatumIsteka <= query.DatumIstekaDo.Value);

        if (query.TipZaposlenja.HasValue)
            oglasiUpit = oglasiUpit.Where(x => x.TipZaposlenja == query.TipZaposlenja.Value);

        if (query.RadnoVreme.HasValue)
            oglasiUpit = oglasiUpit.Where(x => x.RadnoVreme == query.RadnoVreme.Value);

        if (query.Senioritet.HasValue)
            oglasiUpit = oglasiUpit.Where(x => x.Senioritet == query.Senioritet.Value);

        if (query.IskustvoMin.HasValue)
            oglasiUpit = oglasiUpit.Where(x => x.IskustvoMin >= query.IskustvoMin.Value);

        if (query.IskustvoMax.HasValue)
            oglasiUpit = oglasiUpit.Where(x => x.IskustvoMax <= query.IskustvoMax.Value);

        if (!string.IsNullOrWhiteSpace(query.Grad))
            oglasiUpit = oglasiUpit.Where(x => x.Grad.Contains(query.Grad));

        if (!string.IsNullOrWhiteSpace(query.Drzava))
            oglasiUpit = oglasiUpit.Where(x => x.Drzava.Contains(query.Drzava));

        if (query.TipRada.HasValue)
            oglasiUpit = oglasiUpit.Where(x => x.TipRada == query.TipRada.Value);

        if (query.PlataOd.HasValue)
            oglasiUpit = oglasiUpit.Where(x => x.PlataOd >= query.PlataOd.Value);

        if (query.PlataDo.HasValue)
            oglasiUpit = oglasiUpit.Where(x => x.PlataDo <= query.PlataDo.Value);

        if (!string.IsNullOrWhiteSpace(query.Valuta))
            oglasiUpit = oglasiUpit.Where(x => x.Valuta == query.Valuta);

        if (query.PlataVidljiva.HasValue)
            oglasiUpit = oglasiUpit.Where(x => x.PlataVidljiva == query.PlataVidljiva.Value);


        var ukupanBroj = await oglasiUpit.CountAsync(cancellationToken);

        var oglasi = await oglasiUpit
            .Select(OglasModel.Projekcija)
            .Skip((query.BrojStranice - 1) * query.VelicinaStranice)
            .Take(query.VelicinaStranice)
            .ToListAsync(cancellationToken);

        var ukupnoStranica = (int)Math.Ceiling(ukupanBroj / (double)query.VelicinaStranice);

        return new PretraziOglaseResult
        {
            Oglasi = oglasi,
            BrojStranice = query.BrojStranice,
            VelicinaStranice = query.VelicinaStranice,
            UkupanBroj = ukupanBroj,
            UkupnoStranica = ukupnoStranica
        };
    }

}