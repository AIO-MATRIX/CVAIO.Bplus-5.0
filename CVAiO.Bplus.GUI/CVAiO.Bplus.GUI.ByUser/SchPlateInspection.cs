using CVAiO.Bplus.Core;
using CVAiO.Bplus.GUI.Core;
using System;
using CVAiO.Bplus.Algorithm;
using System.Collections.Generic;

namespace CVAiO.Bplus.GUI.ByUser
{
    [Serializable]
    public class SchPlateInspection : SchedulerBase
    {
        public SchPlateInspection() : base()
        {
            DicDisplay.Graph.Add("Measurement", new List<float>());
            DicDisplay.List.Add("Measurement", "");
        }
        public override bool Do_Init()
        {
            // Do 1 time when scheduler Start
            try
            {
                // Start code here
                // End code here
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.Instance.LogException(string.Format("Do_Init Error: {0}", ex.ToString()));
                return false;
            }
        }
        public override bool Do_BeforeStart()
        {
            // Do everytime before run Vision Process
            try
            {
                // Start code here
                // End code here
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.Instance.LogException(string.Format("Do_BeforeStart Error: {0}", ex.ToString()));
                return false;
            }
        }
        public override bool Do_VisionProcess()
        {
            // Run Process scheduler (Image Processing)
            try
            {
                return ProcessScheduler.Run(); ;
            }
            catch (Exception ex)
            {
                LogWriter.Instance.LogException(string.Format("Do_VisionProcess Error: {0}", ex.ToString()));
                return false;
            }
        }
        public override bool Do_Judgement()
        {
            // Do extend after Image processing
            try
            {
                // Start code here
                if ((bool)AlgorithmTool.GetType().GetProperty("AlgoJudgement").GetValue(AlgorithmTool))
                    judgeOK = true;
                else
                    judgeOK = false;
                // End code here
                return judgeOK;
            }
            catch (Exception ex)
            {
                LogWriter.Instance.LogException(string.Format("Do_Judgement Error: {0}", ex.ToString()));
                return false;
            }
        }
        public override bool Do_WriteResult()
        {
            // Send data to PLC or other device
            try
            {
                // Start code here
                // Gửi data sang PLC, Offset từ 20 thanh ghi từ vị trí thanh ghi dữ liệu DataAddr
                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + 20, (int)(AlgorithmTool as AlgoPlateInspection).Length1 * 1000);
                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + 22, (int)(AlgorithmTool as AlgoPlateInspection).Radius1 * 1000);
                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + 24, (int)(AlgorithmTool as AlgoPlateInspection).Radius2 * 1000);

                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + 26, (int)(AlgorithmTool as AlgoPlateInspection).Length2 * 1000);
                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + 28, (int)(AlgorithmTool as AlgoPlateInspection).Radius3 * 1000);
                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + 30, (int)(AlgorithmTool as AlgoPlateInspection).Radius4 * 1000);
                // End code here
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.Instance.LogException(string.Format("Do_WriteResult Error: {0}", ex.ToString()));
                return false;
            }
        }
        public override bool Do_Finalization()
        {
            // Do after reciving Complete signal
            try
            {
                // Start code here
                List<float> list = new List<float>();
                list.Add((float)(AlgorithmTool as AlgoPlateInspection).Length1);
                list.Add((float)(AlgorithmTool as AlgoPlateInspection).Radius1);
                list.Add((float)(AlgorithmTool as AlgoPlateInspection).Radius2);
                list.Add((float)(AlgorithmTool as AlgoPlateInspection).Length2);
                list.Add((float)(AlgorithmTool as AlgoPlateInspection).Radius3);
                list.Add((float)(AlgorithmTool as AlgoPlateInspection).Radius4);

                DicDisplay.AddGraph("Measurement", new List<float> { list[0], list[1], list[2], list[3], list[4], list[5] });
                DicDisplay.AddList("Measurement", string.Format("{0},{1:f3},{2:f3},{3:f3}", DateTime.Now.ToString("HH:mm:ss:fff"), list[0], list[1], list[2], list[3], list[4], list[5]));
                // End code here
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.Instance.LogException(string.Format("Do_Finalization Error: {0}", ex.ToString()));
                return false;
            }
        }
    }
}
