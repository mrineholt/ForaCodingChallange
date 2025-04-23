using AutoMapper;
using Castle.Core.Logging;
using Fora.Infrastructure.Api.SEC;
using Fora.Infrastructure.Api.SEC.CompanyInfo;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fora.UnitTest.Api.SEC.CompanyInfo
{
    [TestClass]
    public class LoadCompanyInfoFromSEC
    {

        Mock<IHttpClientFactory> _httpClientFactory = new Mock<IHttpClientFactory>();

        Mock<ILogger<SECHttpService>> _logger = new Mock<ILogger<SECHttpService>>();

        Infrastructure.Api.SEC.Models.SECConfig _config = new Infrastructure.Api.SEC.Models.SECConfig() { Url = "https://data.sec.gov/api/xbrl/companyfacts/CIK##########.json" };
        Mapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            var httpClient = new Mock<HttpClient>();
            
            //httpClient.Setup(x => x.Dispose());


            _httpClientFactory.Setup(e => e.CreateClient(It.IsAny<string>())).Returns(httpClient.Object);
            _mapper  = new AutoMapper.Mapper(
                new AutoMapper.MapperConfiguration(e => e.AddMaps(typeof(SECHttpService).Assembly)));
        }

        [TestMethod]
        public async Task CanLoadData()
        {
            
            

            var secData = new Infrastructure.Api.SEC.CompanyInfo.CompanyInfoLoader(
                new Infrastructure.Api.SEC.SECHttpService(_httpClientFactory.Object, _logger.Object),
                new Infrastructure.Api.SEC.CompanyInfo.CIKFileLoader(),
                _config,
                _mapper
                );

            var models = await secData.Load();
            Assert.IsTrue(models.Count > 0);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpClientFactory = null;
        }
    }
}
