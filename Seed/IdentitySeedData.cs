using EntityLayer.Constants;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;

namespace MyAcademy_CargoTrack.Seed
{
    public class IdentitySeedData
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<AppRole>>();

            string[] roles =
            {
                RoleNames.Admin,
                RoleNames.Manager,
                RoleNames.User
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(
                        new AppRole { Name = roleName }
                    );

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ",
                            result.Errors.Select(x => x.Description));

                        throw new Exception(
                            $"{roleName} rolü oluşturulamadı: {errors}");
                    }
                }
            }
        }

        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AppUser>>();

            var adminEmail = "admin@cargotrack.com";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    FullName = "CargoTrack Admin",
                  
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    adminUser, "Admin123!");

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ",
                        result.Errors.Select(x => x.Description));

                    throw new Exception(
                        $"Admin hesabı oluşturulamadı: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, RoleNames.Admin))
            {
                var roleResult = await userManager.AddToRoleAsync(
                    adminUser, RoleNames.Admin);

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ",
                        roleResult.Errors.Select(x => x.Description));

                    throw new Exception(
                        $"Admin rolü atanamadı: {errors}");
                }
            }
        }
    }
}
