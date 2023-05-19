using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVAiO.Bplus.GUI.Core;
using CVAiO.Bplus.Core;

namespace CVAiO.Bplus.GUI.ByUser
{
    [Serializable]
    public class SchedulerByUser : SchedulerBase
    {
        public SchedulerByUser() : base()
        {
            
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
                ProcessScheduler.Run();
                return true;
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

                // End code here
                return true;
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
                List<float> list = new List<float>();
                Random rnd = new Random();
                list.Add(rnd.Next(1, 10));
                list.Add(rnd.Next(1, 10));
                list.Add(rnd.Next(1, 10));
                DicDisplay.AddGraph("Inspection", list);
                DicDisplay.AddList("Inspection", string.Format("{0},{1},{2},{3}", DateTime.Now.ToString("HH:mm:ss:fff"), list[0], list[1], list[2]));
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
