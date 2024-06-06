namespace Lab03.Models
{
    public class GioHangViewModel
    {
        //Lưu trữ thông tin các sản phẩm trong giỏ hàng
        public IEnumerable<GioHang> DsGioHang { get; set; }
        //Lưu trữ tổng số tiền của giỏ hàng
       /* public double TotalPrice { get; set; }*/

        public HoaDon HoaDon { get; set; }
        // Không có sử dụng nhưng đang phân vân không biết nên xóa hay 0
    }
}
