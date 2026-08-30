using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Infrastructure.Persistence.Seed;

/// <summary>Наповнення бази початковими даними</summary>
public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Services.AnyAsync())
        {
            return;
        }
        var projector = Service.Create("Проєктор", Money.Uah(500));
        var wifi = Service.Create("Wi-Fi", Money.Uah(300));
        var sound = Service.Create("Звук", Money.Uah(700));

        await context.Services.AddRangeAsync(projector, wifi, sound);

        var roomA = Room.Create("Зал А", 50, Money.Uah(2000));
        var roomB = Room.Create("Зал B", 100, Money.Uah(3500));
        var roomC = Room.Create("Зал C", 30, Money.Uah(1500));

        roomA.AddService(projector);
        roomA.AddService(wifi);

        roomB.AddService(projector);
        roomB.AddService(wifi);
        roomB.AddService(sound);

        roomC.AddService(wifi);

        await context.Rooms.AddRangeAsync(roomA, roomB, roomC);

        await context.SaveChangesAsync();
    }
}