using CVAiO.Bplus.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVAiO.Bplus.Algorithm
{
    public interface IAlgorithm
    {
        // Đây là 2 thuộc tính bắt buộc phải implement của công cụ thuật toán
        // Calc : Nhân tín hiệu yêu cầu tính toán từ công cụ Scheduler
        // AlgoJudgement: Kết quả phán định của quá trình kiểm tra; trường hợp không có tính năng kiểm tra trong công cụ Algo thì mặc định là True
        #region Properties
        Execution Calc { get; set; }
        bool AlgoJudgement { get; set; }
        #endregion
    }
}
