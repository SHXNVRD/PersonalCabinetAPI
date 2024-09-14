using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace API.Extensions
{
    public static class SerilogExtensions
    {
        public static IHostBuilder ConfigureSerilog(this IHostBuilder host)
        {
            host.UseSerilog((context, config) =>
                config.ReadFrom.Configuration(context.Configuration));

            return host;
        }
    }
}