using EntityLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyAcademy_CargoTrack.Areas.Manager.Models
{
    public class DeliveryFailedViewModel
    {
        public Guid CargoId { get; set; }

        [Required(ErrorMessage = "Başarısız teslimat nedeni seçilmelidir.")]
        public ExceptionType ExceptionType { get; set; }

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Description { get; set; } = string.Empty;
    }
}
