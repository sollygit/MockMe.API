using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MockMe.API.Services;
using MockMe.Repository;

namespace MockMe.API
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
            services.AddMemoryCache();
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins(Configuration["CorsUrl"]?.Split(';') ?? [])
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            AutoMapperProfile.Initialize();

            services.AddScoped<ITradeService, TradeService>();
            services.AddScoped<ICountryService, TradeService>();
            services.AddScoped<IAssetService, TradeService>();
            services.AddSingleton<ITradeRepository, TradeRepository>();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("trade", new OpenApiInfo { Title = "Trade API" });
                c.SwaggerDoc("file", new OpenApiInfo { Title = "File Upload API" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "MockMe API";
                c.RoutePrefix = string.Empty;
                c.SwaggerEndpoint("/swagger/trade/swagger.json", "Trade API");
                c.SwaggerEndpoint("/swagger/file/swagger.json", "File Upload API");
            });

            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
