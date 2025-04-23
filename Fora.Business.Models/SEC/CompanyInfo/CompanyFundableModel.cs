using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fora.Business.Models.SEC.CompanyInfo
{
    public class CompanyFundableModel 
    {
        public CompanyInfoModel CompanyInfo { get; set; }
        public decimal StandardFundableAmount { get; set; }
        public decimal SpecialFundableAmount { get; set; }
    }
}
