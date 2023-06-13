using System;
using System.Collections.Generic;
using CVAiO.Bplus.Core;
using CVAiO.Bplus.GUI.Core;
using CVAiO.Bplus.Algorithm;
using CVAiO.Bplus.ToolByUser;

namespace CVAiO.Bplus.GUI.ByUser
{
    [Serializable]
    public class SchTapeAlign2P_Object2Target : SchedulerBase
    {
        public SchTapeAlign2P_Object2Target()
        {
            DicDisplay.Graph.Add("Target", new List<float>());
            DicDisplay.Graph.Add("Object", new List<float>());
            DicDisplay.Graph.Add("Offset", new List<float>());
            DicDisplay.List.Add("Target", "");
            DicDisplay.List.Add("Object", "");
            DicDisplay.List.Add("Offset", "");
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
                return ProcessScheduler.Run();
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
                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + 20, (int)((AlgorithmTool as AlgoTapeAlign2P_Object2Target).AlgoObject2Target.X * 1000));
                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + 22, (int)((AlgorithmTool as AlgoTapeAlign2P_Object2Target).AlgoObject2Target.Y * 1000));
                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + 24, (int)(((AlgorithmTool as AlgoTapeAlign2P_Object2Target).AlgoObject2Target.ThetaRad * 180 /Math.PI ) * 1000));
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
                List<float> listTarget = new List<float>();
                listTarget.Add((float)(AlgorithmTool as AlgoTapeAlign2P_Object2Target).TarLine.CP.X);
                listTarget.Add((float)(AlgorithmTool as AlgoTapeAlign2P_Object2Target).TarLine.CP.Y);
                listTarget.Add((float)((AlgorithmTool as AlgoTapeAlign2P_Object2Target).TarLine.ThetaRad));
                DicDisplay.AddGraph("Target", new List<float> { listTarget[0], listTarget[1], listTarget[2] });
                DicDisplay.AddList("Target", string.Format("{0},{1:f3},{2:f3},{3:f3}", DateTime.Now.ToString("HH:mm:ss:fff"), listTarget[0], listTarget[1], listTarget[2]));

                List<float> listObject = new List<float>();
                listObject.Add((float)(AlgorithmTool as AlgoTapeAlign2P_Object2Target).ObjLine.CP.X);
                listObject.Add((float)(AlgorithmTool as AlgoTapeAlign2P_Object2Target).ObjLine.CP.Y);
                listObject.Add((float)((AlgorithmTool as AlgoTapeAlign2P_Object2Target).ObjLine.ThetaRad));
                DicDisplay.AddGraph("Object", new List<float> { listObject[0], listObject[1], listObject[2] });
                DicDisplay.AddList("Object", string.Format("{0},{1:f3},{2:f3},{3:f3}", DateTime.Now.ToString("HH:mm:ss:fff"), listObject[0], listObject[1], listObject[2]));

                List<float> listObj2Tar = new List<float>();
                listObj2Tar.Add((float)(AlgorithmTool as AlgoTapeAlign2P_Object2Target).AlgoObject2Target.X);
                listObj2Tar.Add((float)(AlgorithmTool as AlgoTapeAlign2P_Object2Target).AlgoObject2Target.Y);
                listObj2Tar.Add((float)((AlgorithmTool as AlgoTapeAlign2P_Object2Target).AlgoObject2Target.ThetaRad));
                DicDisplay.AddGraph("Offset", new List<float> { listObj2Tar[0], listObj2Tar[1], listObj2Tar[2] });
                DicDisplay.AddList("Offset", string.Format("{0},{1:f3},{2:f3},{3:f3}", DateTime.Now.ToString("HH:mm:ss:fff"), listObj2Tar[0], listObj2Tar[1], listObj2Tar[2]));

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
