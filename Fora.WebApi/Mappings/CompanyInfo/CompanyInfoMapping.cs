using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fora.WebApi.Mappings.CompanyInfo
{
    public class CompanyInfoMapping : Profile
    {
        public CompanyInfoMapping()
        {

            this.CreateMap<Business.Models.SEC.CompanyInfo.CompanyFundableModel, Models.CompanyInfo.CompanyInfoModel>()
                .ForMember(c => c.Id, c => c.MapFrom(e => e.CompanyInfo.CompanyInfoId))
                .ForMember(c => c.Name, c => c.MapFrom(e => e.CompanyInfo.Name));
        }
    }
}
