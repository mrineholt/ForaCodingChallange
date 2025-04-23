using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fora.UnitTest.Api.SEC.CompanyInfo
{

    [TestClass]
    public class LoadCIKFile
    {

        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public async Task CanLoadFile()
        {
            var fileLoader = new Infrastructure.Api.SEC.CompanyInfo.CIKFileLoader();
            var models = await fileLoader.Load();
            Assert.IsTrue(models.Count > 0);
        }


    }
}
