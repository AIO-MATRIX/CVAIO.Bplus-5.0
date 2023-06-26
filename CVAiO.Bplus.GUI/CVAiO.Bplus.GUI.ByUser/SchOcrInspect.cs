using System;
using System.Collections.Generic;
using CVAiO.Bplus.Core;
using CVAiO.Bplus.GUI.Core;
using CVAiO.Bplus.Algorithm;
using CVAiO.Bplus.ToolByUser;
using System.ComponentModel;

namespace CVAiO.Bplus.GUI.ByUser
{
    [Serializable]
    public class SchOcrInspect : SchedulerBase
    {
        #region Fields
        private int ocr1OffsetAddr;
        private int ocr2OffsetAddr;
        private int ocr3OffsetAddr;

        #endregion

        [PropertyOrder(70), Description("Offset of OCR 1 Writting address from Data start address"), Editor(typeof(UpDownValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int Ocr1OffsetAddr { get => ocr1OffsetAddr; set => ocr1OffsetAddr = value; }

        [PropertyOrder(71), Description("Offset of OCR 2 Writting address from Data start address"), Editor(typeof(UpDownValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int Ocr2OffsetAddr { get => ocr2OffsetAddr; set => ocr2OffsetAddr = value; }

        [PropertyOrder(72), Description("Offset of OCR 3 Writting address from Data start address"), Editor(typeof(UpDownValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int Ocr3OffsetAddr { get => ocr3OffsetAddr; set => ocr3OffsetAddr = value; }
        #region Properties


        #endregion
        public SchOcrInspect()
        {
            DicDisplay.List.Add("Ocr1", "");
            DicDisplay.List.Add("Orc2", "");
            DicDisplay.List.Add("Ocr3", "");
            ocr1OffsetAddr = 20;
            ocr2OffsetAddr = 40;
            ocr3OffsetAddr = 60;
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
                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + ocr1OffsetAddr, (string)((AlgorithmTool as AlgoOcrInspect).InOcr1));
                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + ocr2OffsetAddr, (string)((AlgorithmTool as AlgoOcrInspect).InOcr2));
                IFCommunication.ComData.WriteValue((int)IFCommunication.DataAddr + ocr3OffsetAddr, (string)((AlgorithmTool as AlgoOcrInspect).InOcr3));
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
                
                List<string> listOcr = new List<string>();
                listOcr.Add((string)((AlgorithmTool as AlgoOcrInspect).InOcr1));
                listOcr.Add((string)((AlgorithmTool as AlgoOcrInspect).InOcr2));
                listOcr.Add((string)((AlgorithmTool as AlgoOcrInspect).InOcr3));
                DicDisplay.AddList("Offset", string.Format("{0},{1:f3},{2:f3},{3:f3}", DateTime.Now.ToString("HH:mm:ss:fff"), listOcr[0], listOcr[1], listOcr[2]));

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
