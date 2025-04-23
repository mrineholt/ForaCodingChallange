using Fora.Business.Models.SEC.CompanyInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fora.Business.Interfaces.SEC.CompanyInfo
{
    public interface ICompanyInfoCachedLoader
    {
        Task<List<CompanyFundableModel>> Load();
        Task<List<CompanyFundableModel>> LoadByFirstLetter(char c);
    }
}
