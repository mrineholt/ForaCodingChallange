using Fora.Infrastructure.Api.SEC.CompanyInfo;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fora.Infrastructure.Api.SEC
{
    public class Services
    {
        public static IServiceCollection Configure(IServiceCollection services) {
            //Just incase the framework layer didn't add it
            services.AddHttpClient();

            services.AddScoped<SECHttpService>();
            services.AddScoped<CIKFileLoader>();
            services.AddScoped<Fora.Business.Interfaces.Infrastructure.SEC.CompanyInfo.ICompanyInfoLoader, CompanyInfoLoader>();


            return services;
        }
    }
}
