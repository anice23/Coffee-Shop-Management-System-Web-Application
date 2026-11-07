namespace CoffeeShop.API.DTOs
{
    public class UpdateCafeSettingsRequestDto
    {
        public int CafeSettingId { get; set; }

        public string CafeName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string ReceiptFooter { get; set; } = string.Empty;

        public int LowStockThreshold { get; set; }
    }
}