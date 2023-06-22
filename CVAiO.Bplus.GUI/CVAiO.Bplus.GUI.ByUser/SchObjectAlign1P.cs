using System;
using System.Collections.Generic;
using CVAiO.Bplus.Core;
using CVAiO.Bplus.GUI.Core;
using CVAiO.Bplus.Algorithm;
using CVAiO.Bplus.ToolByUser;

namespace CVAiO.Bplus.GUI.ByUser
{
    [Serializable]
    public class SchObjectAlign1P : SchedulerBase
    {
        public SchObjectAlign1P()
        {
            DicDisplay.Graph.Add("ObjectPosition", new List<float>());
            DicDisplay.List.Add("ObjectPosition", "");
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
                // Send Object Position to PLC
                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + 4, (float)(AlgorithmTool as AlgoObjectAlign1P).AlgoPosition.X);
                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + 6, (float)(AlgorithmTool as AlgoObjectAlign1P).AlgoPosition.Y);
                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + 8, (float)(AlgorithmTool as AlgoObjectAlign1P).AlgoPosition.ThetaRad);
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
                list.Add((float)(AlgorithmTool as AlgoObjectAlign1P).AlgoPosition.X);
                list.Add((float)(AlgorithmTool as AlgoObjectAlign1P).AlgoPosition.Y);
                list.Add((float)((AlgorithmTool as AlgoObjectAlign1P).AlgoPosition.ThetaRad));

                DicDisplay.AddGraph("ObjectPosition", new List<float> { list[0], list[1], list[2] });
                DicDisplay.AddList("ObjectPosition", string.Format("{0},{1:f3},{2:f3},{3:f3}", DateTime.Now.ToString("HH:mm:ss:fff"), list[0], list[1], list[2]));

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
