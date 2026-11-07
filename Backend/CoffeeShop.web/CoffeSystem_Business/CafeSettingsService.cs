
using CoffeSystem_DataAccess.Entities;
using CoffeSystem_DataAccess.Repositories;
using System;
using System.Threading.Tasks;

namespace CoffeSystem_Business
{
    public class CafeSettingsService
    {
        private readonly CafeSettingsRepository _repository;

        public CafeSettingsService(
            CafeSettingsRepository repository)
        {
            _repository = repository;
        }

        // Methode N°1
        public async Task<CafeSetting?> GetSettingsAsync()
        {
            return await _repository.GetSettingsAsync();
        }

        // Methode N°2

        public async Task UpdateSettingsAsync( int cafeSettingId,string cafeName,string address,string phoneNumber,string receiptFooter, int lowStockThreshold)
        {
            if (string.IsNullOrWhiteSpace(cafeName))
            {
                throw new ArgumentException(
                    "Cafe name is required.");
            }

            if (lowStockThreshold < 0)
            {
                throw new ArgumentException(
                    "Low stock threshold cannot be negative.");
            }

            // Prepare values before sending them to the Repository
            cafeName = cafeName.Trim();
            address = address?.Trim() ?? string.Empty;
            phoneNumber = phoneNumber?.Trim() ?? string.Empty;
            receiptFooter = receiptFooter?.Trim() ?? string.Empty;

            await _repository.UpdateSettingsAsync(
                cafeSettingId,
                cafeName,
                address,
                phoneNumber,
                receiptFooter,
                lowStockThreshold);
        }

    }
}
