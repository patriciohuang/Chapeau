using Chapeau.Repositories;
using Chapeau.Services;

namespace Chapeau
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<IMenuRepository, DbMenuRepository>();
            builder.Services.AddSingleton<IMenuService, MenuService>();

            builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
            builder.Services.AddSingleton<IKitchenBarDisplayService, KitchenBarDisplayService>();
            builder.Services.AddSingleton<IPaymentRepository, PaymentRepository>();
            builder.Services.AddSingleton<IPaymentService, PaymentService>();

            // Register employees repository
            builder.Services.AddSingleton<IEmployeesRepository, EmployeesRepository>();

            // Register tables service/repository
            builder.Services.AddSingleton<ITableService, TableService>();
            builder.Services.AddSingleton<ITablesRepository, TablesRepository>();

            // Register password hashing service
            builder.Services.AddSingleton<IPasswordHashingService, PasswordHashingService>();

            // Register authentication service
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

            // Register employee management service
            builder.Services.AddSingleton<IEmployeeManagementService, EmployeeManagementService>();

            // Register order service
            builder.Services.AddSingleton<IOrderService, OrderService>();

            //signalR
            builder.Services.AddSignalR();

            builder.Services.AddControllersWithViews();

            //enable session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(300);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //enable sessions
            app.UseSession();

            app.UseAuthorization();
            app.MapHub<Hubs.OrderHub>("/orderHub");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}"); // Default route to the login page

            app.Run();
        }
    }
}
