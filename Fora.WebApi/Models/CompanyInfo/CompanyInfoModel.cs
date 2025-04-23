using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fora.WebApi.Models.CompanyInfo
{
    public class CompanyInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal StandardFundableAmount { get; set; }
        public decimal SpecialFundableAmount { get; set; }
    }
}
