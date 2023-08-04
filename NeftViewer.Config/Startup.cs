using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NeftViewer.Config.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Config
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var settings = Configuration.GetSection("Settings").Get<Settings>();

            if (settings == null)
            {
                // Логирование ошибки или выброс исключения, если settings равно null.
                throw new InvalidOperationException("Settings section is missing or invalid.");
            }

            services.AddSingleton(settings);
        }

    }



}
