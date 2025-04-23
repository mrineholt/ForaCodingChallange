using Fora.Infrastructure.Api.SEC.Converters;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Fora.Infrastructure.Api.SEC.Models.CompanyInfo
{
    public class SECCompanyInfoModel
    {
        //standardize the garbage data
        [JsonPropertyName("cik"), JsonConverter(typeof(IntegerConverter))]
        public int Cik { get; set; }

        [JsonPropertyName("entityName")]
        public string EntityName { get; set; }
        [JsonPropertyName("facts")]
        public InfoFact Facts { get; set; }
        public class InfoFact
        {
            [JsonPropertyName("us-gaap")]
            public InfoFactUsGaap UsGaap { get; set; }
        }
        public class InfoFactUsGaap
        {
            [JsonPropertyName("NetIncomeLoss")]
            public InfoFactUsGaapNetIncomeLoss NetIncomeLoss { get; set; }
        }
        public class InfoFactUsGaapNetIncomeLoss
        {
            [JsonPropertyName("units")]
            public InfoFactUsGaapIncomeLossUnits Units { get; set; }
        }
        public class InfoFactUsGaapIncomeLossUnits
        {
            [JsonPropertyName("USD")]
            public InfoFactUsGaapIncomeLossUnitsUsd[] Usd { get; set; }
        }
        public class InfoFactUsGaapIncomeLossUnitsUsd
        {
            /// <summary>
            /// Possibilities include 10-Q, 10-K,8-K, 20-F, 40-F, 6-K, and  their variants.YOU ARE INTERESTED ONLY IN 10-K DATA!
            /// </summary>
            [JsonPropertyName("form")]
            public string Form { get; set; }
            /// <summary>
            /// For yearly information, the format is CY followed by the year                 number.For example: CY2021.YOU ARE INTERESTED ONLY IN YEARLY INFORMATION                WHICH FOLLOWS THIS FORMAT!
            /// </summary>
            [JsonPropertyName("frame")]
            public string Frame { get; set; }
            /// <summary>
            /// The income/loss amount.
            /// </summary>
            [JsonPropertyName("val")]
            public decimal Val { get; set; }

        }

    }
}
