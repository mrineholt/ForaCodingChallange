using AutoMapper;
using Fora.Business.Models.SEC.CompanyInfo;
using Fora.Business.SEC.CompanyInfo;
using Fora.Infrastructure.Api.SEC;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fora.Business.Models.SEC.CompanyInfo.CompanyInfoModel;

namespace Fora.UnitTest.Business.SEC.CompanyInfo
{
    [TestClass]
    public class CompanyInfo
    {
        Mock<IHttpClientFactory> _httpClientFactory = new Mock<IHttpClientFactory>();

        Mock<ILogger<SECHttpService>> _logger = new Mock<ILogger<SECHttpService>>();

        Infrastructure.Api.SEC.Models.SECConfig _config = new Infrastructure.Api.SEC.Models.SECConfig() { Url = "https://data.sec.gov/api/xbrl/companyfacts/CIK##########.json" };
        Infrastructure.Api.SEC.CompanyInfo.CompanyInfoLoader _secCompanyLoader;
        Mapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            var httpClient = new Mock<HttpClient>();

            //httpClient.Setup(x => x.Dispose());


            _httpClientFactory.Setup(e => e.CreateClient(It.IsAny<string>())).Returns(httpClient.Object);
            _mapper = new AutoMapper.Mapper(
                new AutoMapper.MapperConfiguration(e => e.AddMaps(typeof(SECHttpService).Assembly)));


            _secCompanyLoader = new Infrastructure.Api.SEC.CompanyInfo.CompanyInfoLoader(
                new Infrastructure.Api.SEC.SECHttpService(_httpClientFactory.Object, _logger.Object),
                new Infrastructure.Api.SEC.CompanyInfo.CIKFileLoader(),
                _config,
                _mapper
                );
        }

        //Test harness until API is written
        //[TestMethod]
        public async Task CanLoadData()
        {

            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var models = await cachedLoader.Load();

            Assert.IsTrue(models.Count > 0);
        }


        [TestMethod]
        public async Task YearRequirementMet()
        {

            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var model = new Fora.Business.Models.SEC.CompanyInfo.CompanyInfoModel()
            {
                NetIncomeLossInUSD = new[] {
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2018 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2019 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2020 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2021 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2022 },  }
            };

            var res = cachedLoader.HasAllYearsRequirement(model);

            Assert.IsTrue(res);
        }

        [TestMethod]
        public async Task YearRequirementNotMet()
        {

            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var model = new Fora.Business.Models.SEC.CompanyInfo.CompanyInfoModel()
            {
                NetIncomeLossInUSD = new[] {
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2018 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2022 },  }
            };

            var res = cachedLoader.HasAllYearsRequirement(model);

            Assert.IsFalse(res);
        }

        [TestMethod]
        public async Task PositiveIncomeRequirementMet()
        {

            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var model = new Fora.Business.Models.SEC.CompanyInfo.CompanyInfoModel()
            {
                NetIncomeLossInUSD = new[] {
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2021, Value = 12 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2022, Value = 12 },  }
            };

            var res = cachedLoader.HasPositiveIncomeRequirement(model);

            Assert.IsTrue(res);
        }


        [TestMethod]
        public async Task PositiveIncomeRequirementNotMet()
        {

            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var model = new Fora.Business.Models.SEC.CompanyInfo.CompanyInfoModel()
            {
                NetIncomeLossInUSD = new[] {
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2021, Value = -12 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2022, Value = 12 },  }
            };

            var res = cachedLoader.HasPositiveIncomeRequirement(model);

            Assert.IsFalse(res);
        }


        [TestMethod]
        public async Task GetHighestIncome()
        {
            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var model = new Fora.Business.Models.SEC.CompanyInfo.CompanyInfoModel()
            {
                NetIncomeLossInUSD = new[] {
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2021, Value = 11 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2022, Value = 12 },  }
            };

            var res = cachedLoader.HighestIncome(model);

            Assert.IsTrue(res == 12);
        }

        [TestMethod]
        public async Task VowelFundingRequirementMet()
        {

            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var model = new Fora.Business.Models.SEC.CompanyInfo.CompanyInfoModel()
            {
                Name = "ABC Inc.",
                NetIncomeLossInUSD = new[] {
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2021, Value = 11 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2022, Value = 12 },  }
            };

            var res = cachedLoader.HasVowelFundingRequirement(model);

            Assert.IsTrue(res);
        }

        [TestMethod]
        public async Task VowelFundingRequirementNotMet()
        {
            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var model = new Fora.Business.Models.SEC.CompanyInfo.CompanyInfoModel()
            {
                Name = "DEF Inc.",
                NetIncomeLossInUSD = new[] {
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2021, Value = 11 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2022, Value = 12 },  }
            };

            var res = cachedLoader.HasVowelFundingRequirement(model);

            Assert.IsFalse(res);
        }

        [TestMethod]
        public async Task IncomeByYear()
        {

            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var model = new Fora.Business.Models.SEC.CompanyInfo.CompanyInfoModel()
            {
                NetIncomeLossInUSD = new[] {
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2021, Value = 11 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2022, Value = 12 },  }
            };

            var res2021 = cachedLoader.IncomeByYear(model, 2021);
            var res2022 = cachedLoader.IncomeByYear(model, 2022);
            Assert.IsTrue(res2021 == 11);
            Assert.IsTrue(res2022== 12);
        }


        [TestMethod]
        public async Task CalculateStandardFundableAmount10B()
        {

            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var model = new Fora.Business.Models.SEC.CompanyInfo.CompanyInfoModel()
            {
                NetIncomeLossInUSD = new[] {
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2018, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2019, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2020, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2021 , Value = 100},
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2022 , Value = 10000000000},   }
            };

            var res = await cachedLoader.MapToFundableModel(new List<CompanyInfoModel>() { model });
            
            Assert.IsTrue(res.FirstOrDefault().StandardFundableAmount == 1233000000);

        }

        [TestMethod]
        public async Task CalculateStandardFundableAmountUnder10B()
        {

            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var model = new Fora.Business.Models.SEC.CompanyInfo.CompanyInfoModel()
            {
                NetIncomeLossInUSD = new[] {
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2018, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2019, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2020, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2021 , Value = 100},
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2022 , Value = 10000},   }
            };

            var res = await cachedLoader.MapToFundableModel(new List<CompanyInfoModel>() { model });

            Assert.IsTrue(res.FirstOrDefault().StandardFundableAmount == 2151.00m);

        }


        [TestMethod]
        public async Task CalculateSpecialFundableAmount10BWithVowel()
        {

            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var model = new Fora.Business.Models.SEC.CompanyInfo.CompanyInfoModel()
            {
                Name = "Alpha inc",
                NetIncomeLossInUSD = new[] {
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2018, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2019, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2020, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2021 , Value = 100},
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2022 , Value = 10000000000},   }
            };

            var res = await cachedLoader.MapToFundableModel(new List<CompanyInfoModel>() { model });

            Assert.IsTrue(res.FirstOrDefault().SpecialFundableAmount == 1233000000 * 1.15m);

        }

        [TestMethod]
        public async Task CalculateSpecialFundableAmountUnder10BWithoutVowel()
        {

            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var model = new Fora.Business.Models.SEC.CompanyInfo.CompanyInfoModel()
            {
                Name = "Delta",
                NetIncomeLossInUSD = new[] {
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2018, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2019, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2020, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2021 , Value = 100},
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2022 , Value = 10000},   }
            };

            var res = await cachedLoader.MapToFundableModel(new List<CompanyInfoModel>() { model });

            Assert.IsTrue(res.FirstOrDefault().SpecialFundableAmount == 2151.00m);

        }

        [TestMethod]
        public async Task CalculateSpecialFundableAmountUnder10BWithoutVowelAnd2022wasLessThan2021()
        {

            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var model = new Fora.Business.Models.SEC.CompanyInfo.CompanyInfoModel()
            {
                Name = "Delta",
                NetIncomeLossInUSD = new[] {
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2018, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2019, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2020, Value = 100 },
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2021 , Value = 10000},
                    new InfoFactUsGaapIncomeLossUnitsUsd() { Year = 2022 , Value = 100},   }
            };

            var res = await cachedLoader.MapToFundableModel(new List<CompanyInfoModel>() { model });

            Assert.IsTrue(res.FirstOrDefault().SpecialFundableAmount == 2151.00m * .75m);

        }


        [TestMethod]
        public void CorrectRangeGenerated()
        {

            var cachedLoader = new CompanyInfoCachedLoader(_secCompanyLoader);


            var range1 = cachedLoader.CreateRange(2018, 2022);

            var expectedRange1 = new int[5] { 2018, 2019, 2020, 2021, 2022 };
            Assert.IsTrue(range1.All(c=> expectedRange1.Any(p=>p == c)));


            var range2= cachedLoader.CreateRange(2018, 2018);

            var expectedRange2 = new int[1] { 2018 };
            Assert.IsTrue(range2.All(c => expectedRange2.Any(p => p == c)));

        }
    }
}
