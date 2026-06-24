using JobLess.Company.Domain.Enums;

namespace JobLess.Company.Application.Common.Helpers;

public static class IndustryHelper
{
    private static readonly Dictionary<string, Industry> Industries =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "Informacione tehnologije", Industry.InformationTechnology },
            { "Finansije i bankarstvo", Industry.FinanceAndBanking },
            { "Maloprodaja i usluge", Industry.RetailAndServices },
            { "Industrija i proizvodnja", Industry.Manufacturing },
            { "Zdravstvo", Industry.Healthcare },
            { "Građevinarstvo", Industry.Construction },
            { "Mediji i marketing", Industry.MediaAndMarketing },
            { "Ostalo", Industry.Other }
        };

    public static bool IsValid(string value)
        => Industries.ContainsKey(value);

    public static Industry GetIndustry(string value)
    {
        if (!Industries.TryGetValue(value, out var industry))
            throw new ArgumentException("Nepostojeća industrija.");

        return industry;
    }
}