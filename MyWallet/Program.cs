using Microsoft.EntityFrameworkCore;
using MyWallet.Data;

namespace MyWallet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configure DbContext with SQL Server provider. Connection string name: "MyWalletContext"
            builder.Services.AddDbContext<MyWalletContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyWalletContext")));

            var app = builder.Build();

            // アプリ起動時に画像登録
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MyWalletContext>();
                db.Database.Migrate();        // マイグレーション適用
                db.SeedImagesFromWwwroot();    // 画像登録
                db.SeedData(db);           // PaymentType データ登録
            }


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Payments}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
