using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Domain.Enums
{
    public enum Industry
    {
        [Display(Name = "Informacione tehnologije")]
        InformationTechnology = 1,

        [Display(Name = "Finansije i bankarstvo")]
        FinanceAndBanking = 2,

        [Display(Name = "Maloprodaja i usluge")]
        RetailAndServices = 3,

        [Display(Name = "Industrija i proizvodnja")]
        Manufacturing = 4,

        [Display(Name = "Zdravstvo")]
        Healthcare = 5,

        [Display(Name = "Građevinarstvo")]
        Construction = 6,

        [Display(Name = "Mediji i marketing")]
        MediaAndMarketing = 7,

        [Display(Name = "Ostalo")]
        Other = 8
    }
}
