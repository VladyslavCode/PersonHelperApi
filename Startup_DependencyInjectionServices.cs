using Microsoft.Extensions.DependencyInjection;
using PersonHelperApi.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonHelperApi
{
    public partial class Startup
    {
        void SetupServices(IServiceCollection services)
        {
            services.AddSingleton<InternalDbHandler>();
        }
    }
}
