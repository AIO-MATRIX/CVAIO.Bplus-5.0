using CVAiO.Bplus.Core;
using CVAiO.Bplus.GUI.Core;
using System;

namespace CVAiO.Bplus.GUI.ByUser
{
    [Serializable]
    public class SchTapeInspection : SchedulerBase
    {
        public SchTapeInspection() : base()
        {

        }
        public override bool Do_Init()
        {
            // Do 1 time when scheduler Start
            try
            {
                // Start code here
                sequenceDelay = 10;
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
                schedulerTool.Trigger1 = new Trigger(true);
                //schedulerTool.Trigger2 = new Trigger(true);
                //schedulerTool.Trigger3 = new Trigger(true);
                //schedulerTool.Trigger4 = new Trigger(true);
                schedulerTool.Calc = new Execution(true);
                schedulerTool.CalcControl = true;
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
