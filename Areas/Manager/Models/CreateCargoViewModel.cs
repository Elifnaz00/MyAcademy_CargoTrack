using EntityLayer.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyAcademy_CargoTrack.Areas.Manager.Models
{
    public class CreateCargoViewModel
    {
        public Guid SenderCustomerId { get; set; }

        public Guid ReceiverCustomerId { get; set; }

        public Guid OriginBranchId { get; set; }

        public Guid DestinationBranchId { get; set; }

        public decimal Weight { get; set; }

        public decimal Length { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public CargoType CargoType { get; set; }

        public DeliveryType DeliveryType { get; set; }

        public decimal? DeclaredValue { get; set; }

        public List<SelectListItem> Customers { get; set; } = new();

        public List<SelectListItem> Branches { get; set; } = new();
    }
}
