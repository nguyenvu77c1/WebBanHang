using System.ComponentModel.DataAnnotations;

namespace Lab03.Models
{
    public class Config
    {
        [Key]
        public int? Id { get; set; }

        [Required]
        public string? ManHinh { get; set; }
        public string? HeDieuHanh { get; set; }
        public string? CameraSau { get; set; }
        public string? CameraTruoc { get; set; }
        public string? CPU { get; set; }
        public string? Ram { get; set; }
        public string? BoNhoTrong { get; set; }
        public string? Sim { get; set; }
        public string? DungLuong { get; set; }
        public List<Product>? Products { get; set; }
        public string DisplayText
        {
            get
            {
                // Kết hợp các cột lại thành chuỗi biểu diễn
                return $"{ManHinh}, {HeDieuHanh}, {CameraSau}, {CameraSau}, {CameraTruoc}, {CPU}, {CameraSau}, {Ram}, {BoNhoTrong}, {Sim}, {DungLuong}"; // Thay đổi theo nhu cầu của bạn
            }
        }
    }
}
