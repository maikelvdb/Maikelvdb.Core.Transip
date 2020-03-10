using Maikelvdb.Core.Transip.Api.Interfaces;
using Maikelvdb.Core.Transip.Api.Models.Options;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Maikelvdb.Core.Transip.Api.Extensions
{
    public static class TransipApiExtensions
    {
        public static IServiceCollection AddTransipApi(this IServiceCollection services, Action<TransipApiOptions> options)
        {
            var opt = new TransipApiOptions();
            options.Invoke(opt);

            services.AddSingleton<ITransipApi, TransipApi>((servicePrivider) => new TransipApi(opt));

            return services;
        }
    }
}
