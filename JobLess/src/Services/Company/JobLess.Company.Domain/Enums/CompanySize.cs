using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLess.Company.Domain.Enums
{
    public enum CompanySize
    {
        [Display(Name = "1-10")]
        OneToTen = 1,

        [Display(Name = "11-50")]
        ElevenToFifty = 2,

        [Display(Name = "51-200")]
        FiftyOneToTwoHundred = 3,

        [Display(Name = "201-500")]
        TwoHundredOneToFiveHundred = 4,

        [Display(Name = "500+")]
        MoreThanFiveHundred = 5
    }
}
