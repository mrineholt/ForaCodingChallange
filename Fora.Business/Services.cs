using Fora.Business.Interfaces.SEC.CompanyInfo;
using Fora.Business.SEC.CompanyInfo;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fora.Business
{
 
        public class Services
        {
            public static IServiceCollection Configure(IServiceCollection services)
            {

                services.AddScoped<ICompanyInfoCachedLoader, CompanyInfoCachedLoader>();


                return services;
            }
        }
}
