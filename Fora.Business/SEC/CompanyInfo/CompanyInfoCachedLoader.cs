using Fora.Business.Interfaces.Infrastructure.SEC.CompanyInfo;
using Fora.Business.Interfaces.SEC.CompanyInfo;
using Fora.Business.Models.SEC.CompanyInfo;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fora.Business.SEC.CompanyInfo
{
    public class CompanyInfoCachedLoader : ICompanyInfoCachedLoader
    {
        #region Properties

        ICompanyInfoLoader _companyInfoLoader;
        static List<CompanyFundableModel> _cache { get; set; } = null;

        #endregion

        #region Constructors

        public CompanyInfoCachedLoader(ICompanyInfoLoader companyInfoLoader)
        {

            _companyInfoLoader = companyInfoLoader;
        }

        #endregion

        #region Methods

        public async Task<List<CompanyFundableModel>> Load()
        {
            await LoadCache();
            return _cache;
        }

        public async Task<List<CompanyFundableModel>> LoadByFirstLetter(char c)
        {
            var records = await this.Load();
            var res = records
                .Where(p =>
                   (p.CompanyInfo.Name ?? String.Empty).StartsWith($"{c}", StringComparison.OrdinalIgnoreCase))
                .ToList();
            return res;
        }

        async Task LoadCache()
        {
            if (_cache == null)
            {
                var res = await _companyInfoLoader.Load();

                var filtered = FilterByFormRequirements(res);


                _cache = await MapToFundableModel(filtered);
            }
        }

        public async Task<List<CompanyFundableModel>> MapToFundableModel(List<CompanyInfoModel> models)
        {

            List<CompanyFundableModel> fundsModels = new List<CompanyFundableModel>();

            foreach (var item in models)
            {

                var model = new CompanyFundableModel() { CompanyInfo = item };

                CalculateStandardFundableAmount(ref model);
                CalculateSpecialFundableAmount(ref model);

                fundsModels.Add(model);

            }

            return fundsModels;
        }

        public void CalculateStandardFundableAmount(ref CompanyFundableModel model)
        {
            bool hasAllYear = HasAllYearsRequirement(model.CompanyInfo);
            bool hasPositive = HasPositiveIncomeRequirement(model.CompanyInfo);
            if (hasAllYear && hasPositive)
            {
                var highestIncome = HighestIncome(model.CompanyInfo);
                if (highestIncome >= 10000000000)
                    model.StandardFundableAmount = highestIncome * .1233m;
                else
                    model.StandardFundableAmount = highestIncome * .2151m;
            }
            else
            {
                model.StandardFundableAmount = 0;
            }
        }

        public void CalculateSpecialFundableAmount(ref CompanyFundableModel model)
        {
            model.SpecialFundableAmount = model.StandardFundableAmount;

            if (HasVowelFundingRequirement(model.CompanyInfo))
                model.SpecialFundableAmount = model.SpecialFundableAmount * 1.15m;

            if (IncomeByYear(model.CompanyInfo, 2022) < IncomeByYear(model.CompanyInfo, 2021))
                model.SpecialFundableAmount = model.SpecialFundableAmount * .75m;

        }

        public List<CompanyInfoModel> FilterByFormRequirements(List<CompanyInfoModel> res)
        {
            return
                res
                .Select(p =>
                {
                    if (p.NetIncomeLossInUSD == null)
                        p.NetIncomeLossInUSD = new CompanyInfoModel.InfoFactUsGaapIncomeLossUnitsUsd[0];

                    p.NetIncomeLossInUSD = p.NetIncomeLossInUSD.Where(p => p.Form == "10-K").ToArray();
                    return p;
                })
                .ToList();
        }

        public bool HasAllYearsRequirement(CompanyInfoModel model) =>
                this.CreateRange(2018, 2022)
                .All(c => model.NetIncomeLossInUSD.Any(a => a.Year == c));

        public bool HasPositiveIncomeRequirement(CompanyInfoModel model) => this.CreateRange(2021, 2022).All(c => model.NetIncomeLossInUSD.Any(e => e.Year == c && e.Value > 0));

        public decimal HighestIncome(CompanyInfoModel model) =>
            model.NetIncomeLossInUSD
                 .Where(p => CreateRange(2018, 2022).Contains(p.Year))
                .OrderByDescending(c => c.Value)
                .First().Value;

        public bool HasVowelFundingRequirement(CompanyInfoModel model) => new char[] { 'a', 'e', 'i', 'o', 'u' }.Contains((model.Name ?? String.Empty).ToLower().FirstOrDefault());

        public decimal IncomeByYear(CompanyInfoModel model, int year) => model.NetIncomeLossInUSD.Where(p => p.Year == year).FirstOrDefault().Value;

        public IEnumerable<int> CreateRange(int start, int end) => Enumerable.Range(start, end - start + 1);

        #endregion
    }
}
