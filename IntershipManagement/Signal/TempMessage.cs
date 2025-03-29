using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntershipManagement.Signal
{
    public class TempMessage
    {
        public static string url_warranti = "/admin/warranti/detail/{0}";
        public static string url_export = "/admin/aexport";
        public static string url_user = "/admin/user";
        public static string url_confirm_user = "/admin/user?textsearch={0}";
        public static string url_pricemodel = "/admin/pricemodel";
        public static string url_submitorder = "/admin/order";
        /*Message*/
        public static string register_user = "Tài khoản đại lý {0} vừa được tạo";
        public static string delete_pricemodel = "{0} đã xoá Model cấu hình thưởng {1}";
        public static string edit_pricemodel = "{0} đã sửa Model cấu hình thưởng {1}";
        public static string edit_warranti = "{0} chỉnh sửa thông tin phiếu {1}";
        public static string create_warranti = "{0} đã tạo phiếu {1}";
        public static string approval_warranti = "{0} đã nhận ca bảo hành {1}";
        public static string success_warranti = "{0} đã xác nhận hoàn thành phiếu {1}";
        public static string success_accessary = "{0} đã nhận được linh kiện phiếu {1}";
        public static string create_user = "{0} đã yêu cầu tạo tài khoản {1}";
        public static string confirm_user = "{0} đã phê duyệt tài khoản {1}";
        public static string send_station_warranti = "{0} đã chuyển tới trạm {1} phiếu {2}";
        public static string send_station_export = "{0} đã xuất linh kiện cho bạn";
        public static string send_KTV_warranti = "{0} đã chuyển tới kỹ thuật viên {1} phiếu {2}";
        public static string send_KTV_success = "{0} đã xử lý hoàn thành phiếu {1}";
        public static string submitorder = "Bạn nhận được đơn đặt hàng mới từ đại lý {0} với mã đơn hàng: {1}";
    }
}