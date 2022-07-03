using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Album.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using App.Service;
using App.Security.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace App
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

            services.AddOptions();
            var mailsetting = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailsetting);
            services.AddSingleton<IEmailSender,SendMailService>();

            services.AddRazorPages();

            services.AddDbContext<AppDbContext>(options =>
            {
                string connectstring = Configuration.GetConnectionString("MyBlogContext");
                options.UseSqlServer(connectstring);
            });

            // tiến hành đăng ký IdentityUser

            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            // services.AddDefaultIdentity<AppUser>()
            //         .AddEntityFrameworkStores<MyBlogContext>();
                    // .AddDefaultTokenProviders(); // chổ này *** bug

            // Truy cập IdentityOptions
            services.Configure<IdentityOptions>(options =>
            {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 5 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
                options.SignIn.RequireConfirmedAccount = true;
            });


            services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/login/";
                options.LogoutPath ="/logout/";
                options.AccessDeniedPath = "/khongduoctruycap.html";
            });
            // đăng ký dịch vụ đăng nhập bằng google

            services.AddAuthentication()
                    .AddGoogle(options => {
                        var gconfig = Configuration.GetSection("Authentication:Google");
                        options.ClientId =gconfig["ClientId"];
                        options.ClientSecret=gconfig["ClientSecret"];
                        options.CallbackPath ="/dang-nhap-tu-google";
                    })
                    .AddFacebook(options =>{
                        var fconfig = Configuration.GetSection("Authentication:Facebook");
                        options.AppId = fconfig["AppId"];
                        options.AppSecret=fconfig["AppSecret"];
                        options.CallbackPath ="/dang-nhap-tu-facebook";
                    });
                    // .AddTwitter()
                    // .AddMicrosoftAccount();

            services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

            services.AddAuthorization(options =>{
                options.AddPolicy("AllowEditRole",policyBuilder =>{
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireRole("Editor");
                });
                 options.AddPolicy("InGenZ",policyBuilder =>{
                    policyBuilder.RequireAuthenticatedUser();
                    //policyBuilder.RequireRole("Editor");
                    policyBuilder.Requirements.Add(new GenZRequirement());
                });

                options.AddPolicy("ShowAdminMenu",pb =>{
                    pb.RequireRole("Admin");
                });

                options.AddPolicy("CanUpdateArticle",builder =>{
                    builder.Requirements.Add(new ArticleUpdateRequirement());
                });
                
            });

            services.AddTransient<IAuthorizationHandler,AppAuthorizationHandler>();
                    
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // phải có 2 middelwere này

            app.UseAuthorization(); // phải có 2 middelwere này

            IdentityUser user;
            IdentityDbContext context;

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            // SignInManager<AppUser> s;
            // UserManager<AppUser> u;
        }

    }
}

/*
    CREATE, READ, UPDATE, DELETE (CRUD)

    dotnet aspnet-codegenerator razorpage -m  ASP_Razor_EF.models.Article -dc  ASP_Razor_EF.models.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries


    B1:
    Identity
    dotnet add package System.Data.SqlClient
    dotnet add package Microsoft.EntityFrameworkCore
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    dotnet add package Microsoft.EntityFrameworkCore.Design
    dotnet add package Microsoft.Extensions.DependencyInjection
    dotnet add package Microsoft.Extensions.Logging.Console

    dotnet add package Microsoft.AspNetCore.Identity
    dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
    dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
    dotnet add package Microsoft.AspNetCore.Identity.UI
    dotnet add package Microsoft.AspNetCore.Authentication
    dotnet add package Microsoft.AspNetCore.Http.Abstractions
    dotnet add package Microsoft.AspNetCore.Authentication.Cookies
    dotnet add package Microsoft.AspNetCore.Authentication.Facebook
    dotnet add package Microsoft.AspNetCore.Authentication.Google
    dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
    dotnet add package Microsoft.AspNetCore.Authentication.MicrosoftAccount
    dotnet add package Microsoft.AspNetCore.Authentication.oAuth
    dotnet add package Microsoft.AspNetCore.Authentication.OpenIDConnect
    dotnet add package Microsoft.AspNetCore.Authentication.Twitter



    - chức năng:
        + Athentication: Xác định danh tính -> Login, Logout....
        + Authorization: xác thực quyền truy cập
        + Quản lý user: Sign Up, User, Role...

    - đường dẫn login 
        + /Identity/Account/Login
        + /Identity/Account/Manage

    -phát sinh code tùy biến các trang HTML dotnet aspnet-codegenerator identity -dc ASP_Razor_EF.models.MyBlogContext
    - dotnet new page -n EditUserRoleClaim -o Areas/Admin/Pages/User -na App.Admin.User
    -CallbackPath:
   https://localhost:7191/dang-nhap-tu-google


   - Authorization: xác định danh tính  -> Login, logout...
    + Role-based authorization - xác thực quyền theo vai trò
        + Role(vai trò): (Admin, Editor, Manager, Member..)
        + thử mục Role được tao theo đường dẫn /Areas/Admin/Pages/Role
        + tạo page: dotnet new page -n Index -o Areas/Admin/Pages/Role -na App.Admin.Role

    *policy-based authorization
    *claim-based authorization
*/