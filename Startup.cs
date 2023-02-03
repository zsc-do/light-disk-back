using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using lightDiskBack.Hubs;
using lightDiskBack.Models;
using lightDiskBack.redis;
using lightDiskBack.utils;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.Extensions.Logging;

namespace lightDiskBack
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
            services.AddControllersWithViews();

            services.AddEntityFrameworkMySql()
       .AddDbContext<IdDBContext>(options => options
       .UseMySql("server=127.0.0.1;port=3306;database=zsc_blog;uid=root;pwd=root;character set=utf8", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.35-mysql")));

            services.AddDataProtection();
            services.AddIdentityCore<SysUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            });

            var idBuilder = new IdentityBuilder(typeof(SysUser), typeof(SysRole), services);
            idBuilder.AddEntityFrameworkStores<IdDBContext>()
                .AddDefaultTokenProviders()
                .AddRoleManager<RoleManager<SysRole>>()
                .AddUserManager<UserManager<SysUser>>();


            services.Configure<JWTOptions>(Configuration.GetSection("JWT"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                // var jwtOpt = builder.Configuration.GetSection("JWT").Get<JWTOptions>();
                byte[] keyBytes = Encoding.UTF8.GetBytes("fasdfad&9045dafz222#fadpio@0232");
                var secKey = new SymmetricSecurityKey(keyBytes);
                x.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = secKey
                };
            });


            services.AddControllersWithViews().AddJsonOptions(cfg =>
            {
                cfg.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            }).AddNewtonsoftJson(option =>
            option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );


            //signlRÅäÖÃ
            services.AddSignalR();

            string[] urls = new[] { "http://localhost:8080" };
            services.AddCors(options =>
                options.AddDefaultPolicy(builder => builder.WithOrigins(urls)
                    .AllowAnyMethod().AllowAnyHeader().AllowCredentials())
            );


            services.AddSession();


            //redisÅäÖÃ
            //redis»º´æ
            var section = Configuration.GetSection("Redis:Default");
            //Á¬½Ó×Ö·û´®
            string _connectionString = section.GetSection("Connection").Value;
            //ÊµÀýÃû³Æ
            string _instanceName = section.GetSection("InstanceName").Value;
            //Ä¬ÈÏÊý¾Ý¿â 
            int _defaultDB = int.Parse(section.GetSection("DefaultDB").Value ?? "0");
            services.AddSingleton(new RedisHelper(_connectionString, _instanceName, _defaultDB));



            services.AddHttpClient();

            services.AddScoped<TokenUtils>();

           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();


            app.UseSession();
            app.UseRouting();


            app.UseCors();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<ChatRoomHub>("/chatHub");
            });
        }
    }
}
