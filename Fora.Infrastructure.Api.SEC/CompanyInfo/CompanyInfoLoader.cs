using AutoMapper;
using Fora.Business.Interfaces.Infrastructure.SEC.CompanyInfo;
using Fora.Business.Models.SEC.CompanyInfo;
using Fora.Infrastructure.Api.SEC.Models;
using Fora.Infrastructure.Api.SEC.Models.CompanyInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fora.Infrastructure.Api.SEC.CompanyInfo
{
    public class CompanyInfoLoader : ICompanyInfoLoader
    {
        #region Properties

        SECHttpService _secHttpService;
        CIKFileLoader _cikFileLoader;
        SECConfig _secConfig;
        IMapper _mapper;

        #endregion

        #region Constructors

        public CompanyInfoLoader(
            SECHttpService secHttpService,
            CIKFileLoader cikFileLoader,
            SECConfig secConfig,
            IMapper mapper)
        {
            _secHttpService = secHttpService;
            _cikFileLoader = cikFileLoader;
            _secConfig = secConfig;
            _mapper = mapper;
        }

        #endregion

        #region Methods

        public async Task<List<CompanyInfoModel>> Load()
        {
            //Load the cik ids from the file
            var companyIds = await _cikFileLoader.Load();

            //Download file from SEC
            var secRes = await DownloadFilesFromSECd(companyIds);

            //Map files to business model
            var businessModel = _mapper.Map<List<CompanyInfoModel>>(secRes);

            return businessModel;
        }

        private async Task<List<SECCompanyInfoModel>> DownloadFilesFromSECd(List<int> companyIds)
        {
            List<SECCompanyInfoModel> apiModels = new List<SECCompanyInfoModel>();

            foreach (var cik in companyIds)
            {
                string fullCIK = $"{cik}".PadLeft(10, '0');
                var url = _secConfig.Url.Replace("##########.json", $"{fullCIK}.json");
                var model = await this._secHttpService.Get<SECCompanyInfoModel>(url);
                apiModels.Add(model);
            }

            return apiModels.Where(p=>p != null).ToList();
        }

        #endregion
    }
}
