
using CoffeSystem_DataAccess.DBContext;
using CoffeSystem_DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeSystem_DataAccess.Repositories
{
    public class CafeSettingsRepository
    {
        private readonly AppDbContext _context;

        public CafeSettingsRepository(AppDbContext context)
        {
            _context = context;
        }

        // Methode N°1
        public async Task<CafeSetting> GetSettingsAsync()
        {
            var settings = await _context.CafeSettings.AsNoTracking()
                .OrderBy(s => s.CafeSettingId)
                .FirstOrDefaultAsync();

            if (settings != null)
                return settings;

            // Create default settings on first use
            settings = new CafeSetting();

            _context.CafeSettings.Add(settings);
            await _context.SaveChangesAsync();

            return settings;
        }


        // Methode N°2

        public async Task UpdateSettingsAsync( int cafeSettingId,string cafeName,string address,string phoneNumb ,string receiptFooter,int lowStockThreshold)
        {
            var settings = await _context.CafeSettings
                .FirstOrDefaultAsync(
                    s => s.CafeSettingId == cafeSettingId);

            if (settings == null)
                return;

            // Update fields one by one
            settings.CafeName = cafeName;
            settings.Address = address;
            settings.PhoneNumber = phoneNumb;
            settings.ReceiptFooter = receiptFooter;
            settings.LowStockThreshold = lowStockThreshold;

            settings.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }

    }
}
