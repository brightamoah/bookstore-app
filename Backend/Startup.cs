using System.Text;
using AspNetCoreRateLimit;
using Backend.Data;
using Backend.Helpers;
using Backend.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;



namespace Backend
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("Default");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string not found");
            }

            services.AddDbContext<UserContext>(options =>
            options.UseMySQL(connectionString));
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<JwtService>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            RateLimitConfig.ConfigureRateLimit(services, Configuration);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration"))),
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["jwtToken"];
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddCors(options =>
           {
               options.AddPolicy("AllowVueApp", builder =>
               {
                   builder.WithOrigins("http://localhost:4000")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials()
                          .WithExposedHeaders("Content-Disposition");
               });
           });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStoreApi v1"));

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowVueApp");
            app.UseIpRateLimiting();

            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.WebRootPath, "uploads")),
                RequestPath = "/uploads",
                OnPrepareResponse = ctx =>
       {
           ctx.Context.Response.Headers.Append(
               "Cache-Control",
               "public,max-age=31536000");
           ctx.Context.Response.Headers.Append(
               "Access-Control-Allow-Origin",
               "*");
       }
            });


            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}