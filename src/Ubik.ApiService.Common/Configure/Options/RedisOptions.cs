using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.ApiService.Common.Configure.Options
{
    public class RedisOptions : IOptionsPosition
    {
        public const string Position = "RedisCache";
        public string ConnectionString { get; set; } = string.Empty;
    }
}
