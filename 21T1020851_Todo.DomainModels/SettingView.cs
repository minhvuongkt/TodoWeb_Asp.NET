using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21T1020851_Todo.DomainModels
{
    public class SettingView
    {
        /*
                    ViewPage có 4 thiết lập: 1 -> xếp theo từ a -> z (mặc định)
											 2 -> xếp theo từ z -> a
											 3 -> xếp theo tăng dần (id task)
											 4 -> xếp theo giảm dần (id task)
					StyleView có 2 thiết lập: 1 -> dạng bảng (mặc định)
											  2 -> dạng thẻ (card)
					TypeView có 3 thiết lập: 1 -> chỉ hiển thị những task đã hoàn thành
											 2 -> chỉ hiển thị những task chưa hoàn thành
											 3 -> mặc định (hiển thị cả 2)
        */
        public int UserID { get; set; }
        public int ViewPage { get; set; } = 1;
        public int StyleView { get; set; } = 1;
        public int TypeView { get; set; } = 3;
    }
}
