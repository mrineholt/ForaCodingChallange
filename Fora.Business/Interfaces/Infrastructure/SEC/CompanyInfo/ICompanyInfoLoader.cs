using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fora.Business.Models.SEC.CompanyInfo;

namespace Fora.Business.Interfaces.Infrastructure.SEC.CompanyInfo
{
    public interface ICompanyInfoLoader
    {
        Task<List<CompanyInfoModel>> Load();
    }
}
