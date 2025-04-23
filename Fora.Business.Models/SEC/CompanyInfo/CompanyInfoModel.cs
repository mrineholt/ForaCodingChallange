using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fora.Business.Models.SEC.CompanyInfo
{
    public class CompanyInfoModel
    {
        public int CompanyInfoId { get; set; }
        public string Name { get; set; }

        public InfoFactUsGaapIncomeLossUnitsUsd[] NetIncomeLossInUSD { get; set; }

        public class InfoFactUsGaapIncomeLossUnitsUsd
        {
            /// <summary>
            /// Possibilities include 10-Q, 10-K,8-K, 20-F, 40-F, 6-K, and  their variants.YOU ARE INTERESTED ONLY IN 10-K DATA!
            /// </summary>
            public string Form { get; set; }

            /// <summary>
            /// For yearly information, the format is CY followed by the year                 number.For example: CY2021.YOU ARE INTERESTED ONLY IN YEARLY INFORMATION                WHICH FOLLOWS THIS FORMAT!
            /// </summary>
            public int Year { get; set; }

            /// <summary>
            /// The income/loss amount.
            /// </summary>
            public decimal Value { get; set; }
        }
    }
}
