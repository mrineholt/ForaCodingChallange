using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fora.Infrastructure.Api.SEC.Models.CompanyInfo;

namespace Fora.Infrastructure.Api.SEC.CompanyInfo
{
    public class CIKFileLoader
    {
        public CIKFileLoader()
        {

        }

        public async Task<List<int>> Load()
        {

            var fullPath = Assembly.GetExecutingAssembly().Location;
            FileInfo fileInfo = new FileInfo(fullPath);
            
            
            string contents = await File.ReadAllTextAsync($"{fileInfo.Directory}/cik.json");

            CIKFileModel model = System.Text.Json.JsonSerializer.Deserialize<CIKFileModel>(contents);

            if (model == null)
                throw new Exception($"{typeof(CIKFileLoader)};  Failed to deserialize cik file");

            return model.CIKIds;
        }

    }
}
