using Application.Services;
using Domain.Aggregates.CardAggregate;
using Domain.Aggregates.ProductAggregate;
using Domain.Aggregates.UserAggregate;
using Domain.Shared.ValueObjects;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task<IApplicationBuilder> InitializeDatabaseAsync(this IApplicationBuilder builder, bool seedData = true)
    {
        if (seedData)
            await builder.SeedDataAsync();
        else
            builder.ApplyMigrations();
        
        return builder;
    }
    
    private static IApplicationBuilder ApplyMigrations(this IApplicationBuilder builder)
    {
        using IServiceScope scope = builder.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
       
        var pendingMigrations = context.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
            context.Database.Migrate();

        return builder;
    }

    private static async Task SeedDataAsync(this IApplicationBuilder builder)
    {
        using IServiceScope scope = builder.ApplicationServices.CreateScope();
        await using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        builder.ApplyMigrations();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var roles = Role.All();
        
        foreach (var role in roles)
        {
            if (!context.Roles.Any(r => r.Name == role))
                await roleManager.CreateAsync(new Role { Name = role, NormalizedName = role.ToUpperInvariant() });
        }

        if (!context.Users.Any())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<AppUserManager>();

            var testUser = User.Create("ivan@gmail.com", "9000000000", "ivan@gmail.com", Name.Create("Иван", "Иванов", "Иванович").Value).Value;

            var testUserResult = await userManager.CreateAsync(testUser, "P@ssw0rd");
            if (!testUserResult.Succeeded)
                throw new InvalidOperationException("Cannot create user");

            var testUserRoleResult = await userManager.AddToRoleAsync(testUser, Role.User);
            if (!testUserRoleResult.Succeeded)
                throw new InvalidOperationException("Failed to user add to role");

            await context.SaveChangesAsync();
            
            await context.Users
                .Where(u => u.Id == testUser.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(
                    u => u.EmailConfirmed, u => true));

            await context.SaveChangesAsync();

            var adminUser = User.Create("shxnvrd@gmail.com", "9000000001", "shxnvrd", Name.Create("Admin", "Admin", "Admin").Value).Value;

            var adminUserResult = await userManager.CreateAsync(adminUser, "P@ssw0rd");
            if (!adminUserResult.Succeeded)
                throw new InvalidOperationException("Cannot create user");

            var adminUserRoleResult = await userManager.AddToRolesAsync(adminUser, Role.All());
            if (!adminUserRoleResult.Succeeded)
                throw new InvalidOperationException("Failed to add user to role");
            
            await context.SaveChangesAsync();

            await context.Users
                .Where(u => u.Id == adminUser.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(
                    u => u.EmailConfirmed, u => true));

            await context.SaveChangesAsync();
        }

        foreach (var category in Category.All())
        {
            if (!context.Categories.Any(c => c.Id == category.Id))
                await context.Categories.AddAsync(category);
            
            await context.SaveChangesAsync();
        }
        
        if (!context.Products.Any())
        {
            await context.Database.ExecuteSqlAsync($"""
                                                    insert into products (id, title, quantity, category_id, description)
                                                    values(101, 'Дизель', 100000, {Category.Fuel.Id}, 'Дизель')
                                                    """);

            await context.Database.ExecuteSqlAsync($"""
                                                    insert into product_price_histories (id, price, product_id)
                                                    values ('a579f30b-2816-4958-aa9b-59f3f9a5457c', 1.7,  101)
                                                    """);
        }

        foreach (var status in Status.All())
        {
            if (!context.Statuses.Any(s => s.Id == status.Id))
                await context.Statuses.AddAsync(status);

            await context.SaveChangesAsync();
        }

        if (!context.Cards.Any())
        {
            List<Card> cards = [];
            
            var number1 = CardNumber.Create("009900014094").Value;
            var pin1 = CardPinHash.Create("3028").Value;
            cards.Add(Card.Create(number1, pin1,  Quantity.Create(1000m).Value).Value);
            
            var number2 = CardNumber.Create("009900014095").Value;
            var pin2 = CardPinHash.Create("3028").Value;
            cards.Add(Card.Create(number2, pin2,  Quantity.Create(99999m).Value).Value);
            
            var number3 = CardNumber.Create("009900014096").Value;
            var pin3 = CardPinHash.Create("3028").Value;
            cards.Add(Card.Create(number3, pin3, Quantity.Create(10m).Value).Value);

            context.Attach(Status.Unused);
            await context.Cards.AddRangeAsync(cards);
            await context.SaveChangesAsync();
        }
    }
}