using AutoMapper;
using Fora.Business.Models.SEC.CompanyInfo;
using Fora.Infrastructure.Api.SEC.Models.CompanyInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fora.Infrastructure.Api.SEC.Mappings.CompanyInfo
{
    public class CompanyInfoMapping : Profile
    {

        public CompanyInfoMapping()
        {
            this.CreateMap<SECCompanyInfoModel, CompanyInfoModel>()
                .ForMember(p => p.CompanyInfoId, p => p.MapFrom(c => c.Cik))
                .ForMember(p => p.Name, p => p.MapFrom(c => c.EntityName) )
                .ForMember(p => p.NetIncomeLossInUSD, p =>
                {
                    p.MapFrom(e => this.MapNetIncomeInUSDToBusinessModel(e));
                });
        }

        private CompanyInfoModel.InfoFactUsGaapIncomeLossUnitsUsd[] MapNetIncomeInUSDToBusinessModel(SECCompanyInfoModel e)
        {
            if (e.Facts?.UsGaap?.NetIncomeLoss?.Units?.Usd == null)
                return new CompanyInfoModel.InfoFactUsGaapIncomeLossUnitsUsd[0];

            var res = e.Facts.UsGaap.NetIncomeLoss.Units.Usd.Select((c) =>
            {
                if (c.Frame != null)
                {
                    int year = 0;
                    var yearRegex = Regex.Match(c.Frame, "CY(?<year>[0-9]{4}).*");
                    if (yearRegex.Groups.ContainsKey("year"))
                    {
                        var yearString = yearRegex.Groups["year"].Value;
                        bool converted = int.TryParse(yearString, out year);
                    }
                    return (CompanyInfoModel.InfoFactUsGaapIncomeLossUnitsUsd?)new CompanyInfoModel.InfoFactUsGaapIncomeLossUnitsUsd()
                    {
                        Form = c.Form,
                        Value = c.Val,
                        Year = year
                    };
                }
                return null;
            })
                .Where(c => c != null)
                .GroupBy(p => new { p.Year, p.Form })
                .Select(p => new CompanyInfoModel.InfoFactUsGaapIncomeLossUnitsUsd()
                {
                    Form = p.Key.Form,
                    Value = p.Sum(c => c.Value),
                    Year = p.Key.Year
                })
                .ToArray();

            return res;
        }
    }
}
