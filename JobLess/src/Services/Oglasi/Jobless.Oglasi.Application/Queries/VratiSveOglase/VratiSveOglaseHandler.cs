using JobLess.Oglasi.Application.Models;
using JobLess.Oglasi.Domain.Entities;
using JobLess.Oglasi.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Oglasi.Application.Queries.VratiSveOglase;

public class VratiSveOglaseHandler : IRequestHandler<VratiSveOglaseQuery, VratiSveOglaseResult>
{
    private readonly IApplicationDbContext _context;

    public VratiSveOglaseHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VratiSveOglaseResult> Handle(VratiSveOglaseQuery query, CancellationToken cancellationToken)
    {
        var oglasiUpit = _context.Oglasi
            .AsQueryable();

        var ukupanBroj = await oglasiUpit.CountAsync(cancellationToken);

        var oglasi = await oglasiUpit
            .Where(x => x.Aktivan == true)
            .Select(OglasModel.Projekcija)
            .Skip((query.BrojStranice - 1) * query.VelicinaStranice)
            .Take(query.VelicinaStranice)
            .ToListAsync(cancellationToken);

        var ukupnoStranica = (int)Math.Ceiling(ukupanBroj / (double)query.VelicinaStranice);

        return new VratiSveOglaseResult
        {
            Oglasi = oglasi,
            BrojStranice = query.BrojStranice,
            VelicinaStranice = query.VelicinaStranice,
            UkupanBroj = ukupanBroj,
            UkupnoStranica = ukupnoStranica
        };
    }
}