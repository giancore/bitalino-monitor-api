using BitalinoMonitor.Domain.PatientContext.Handlers;
using BitalinoMonitor.Domain.PatientContext.Repositories;
using BitalinoMonitor.Domain.PatientContext.Services;
using BitalinoMonitor.Infra.DataContexts;
using BitalinoMonitor.Infra.PatientContext.Repositories;
using BitalinoMonitor.Infra.PatientContext.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Elmah.Io.AspNetCore;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using BitalinoMonitor.Shared;

namespace BitalinoMonitor.Api
{
    public class Startup
    {
        public static IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc();

            services.AddResponseCompression();

            services.AddScoped<BitalinoMonitorDataContext, BitalinoMonitorDataContext>();
            services.AddTransient<IPatientRepository, PatientRepository>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<PatientHandler, PatientHandler>();

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Info { Title = "Bitalino Monitor", Version = "v1" });
            });

            Settings.ConnectionString = $"{Configuration["connectionString"]}";
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMvc();
            app.UseResponseCompression();
            app.UseCors(option => option.AllowAnyOrigin());

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bitalino Monitor - V1");
            });

            app.UseElmahIo("923f4c946cc1435cb0ec665d6e7370b7", new Guid("e42a9995-df89-4d91-a625-ecc57d124004"));
        }
    }
}
