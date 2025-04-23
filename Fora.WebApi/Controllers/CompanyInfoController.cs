using AutoMapper;
using Fora.Business.Interfaces.SEC.CompanyInfo;
using Microsoft.AspNetCore.Mvc;

namespace Fora.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyInfoController : ControllerBase
    {
        private readonly ILogger<CompanyInfoController> _logger;
        private readonly IMapper _mapper;
        private readonly ICompanyInfoCachedLoader _companyInfoCachedLoader;


        public CompanyInfoController(
            ILogger<CompanyInfoController> logger,
            ICompanyInfoCachedLoader companyInfoCachedLoader,
            IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _companyInfoCachedLoader = companyInfoCachedLoader;
        }

        [HttpGet()]
        [Route("{letter:alpha?}")]
        public async Task<IEnumerable<Models.CompanyInfo.CompanyInfoModel>> Get(string letter = "")
        {
            var allRecords = String.IsNullOrWhiteSpace(letter ) ? await _companyInfoCachedLoader.Load() : await _companyInfoCachedLoader.LoadByFirstLetter(letter.FirstOrDefault());
            var mappedRes = _mapper.Map<List<Models.CompanyInfo.CompanyInfoModel>>(allRecords);
            return mappedRes;
        }

        /// <summary>
        /// Returns direct from the business layer to debug
        /// </summary>
        /// <param name="CompanyInfoId"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("full/{CompanyInfoId:int}")]
        public async Task<JsonResult> GetById(int CompanyInfoId)
        {
            var allRecords = await _companyInfoCachedLoader.Load();
            return new JsonResult( allRecords.FirstOrDefault(p => p.CompanyInfo.CompanyInfoId == CompanyInfoId));

        }
    }
}
