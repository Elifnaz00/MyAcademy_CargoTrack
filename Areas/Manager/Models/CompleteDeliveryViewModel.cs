using System.ComponentModel.DataAnnotations;

namespace MyAcademy_CargoTrack.Areas.Manager.Models
{
    public class CompleteDeliveryViewModel
    {
        public Guid CargoId { get; set; }

        [Required(ErrorMessage = "Teslimat kodu zorunludur.")]
        [StringLength(6, MinimumLength = 6,
            ErrorMessage = "Teslimat kodu 6 haneli olmalıdır.")]
        [RegularExpression(@"^\d{6}$",
            ErrorMessage = "Teslimat kodu yalnızca 6 rakamdan oluşmalıdır.")]
        public string DeliveryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Teslim alan kişinin adı zorunludur.")]
        [StringLength(100,
            ErrorMessage = "İsim en fazla 100 karakter olabilir.")]
        public string RecipientName { get; set; } = string.Empty;
    }
}
