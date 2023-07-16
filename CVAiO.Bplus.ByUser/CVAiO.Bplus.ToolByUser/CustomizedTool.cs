using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVAiO.Bplus.OpenCV;
using CVAiO.Bplus.Core;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace CVAiO.Bplus.ToolByUser
{
    [Serializable]
    public class CustomizedTool : ToolBase
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        [NonSerialized]
        private Image outImage;
        private ImageInfo inputImageInfo;
        private ImageInfo outputImageInfo;

        private CustomizedToolRunParams runParams;
        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("Tool Params")]
        public CustomizedToolRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new CustomizedToolRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }

        #endregion

        public CustomizedTool()
        {
            toolName = "Customized Tool";
            toolGroup = "Tool By User"; // Don't change tool Group
            name = "Customized Tool";
            ToolColor = System.Drawing.Color.Khaki;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public CustomizedTool(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        #region Init
        public override void InitInParams()
        {
            inParams.Add("InImage", null);
        }
        public override void InitOutParams()
        {
            outParams.Add("OutImage", null);
        }
        public override void InitImageList()
        {
            inputImageInfo = new ImageInfo(string.Format("[{0}] InputImage", this.ToString()));
            outputImageInfo = new ImageInfo(string.Format("[{0}] OutputImage", this.ToString()));
            imageList.Add(inputImageInfo);
            imageList.Add(outputImageInfo);
        }
        public override void InitOutProperty()
        {
            RunStatus = new RunStatus(EToolResult.Waiting, "Tool has not run yet. Please Run Tool first. ");
            if (OutImage != null) OutImage.Dispose();
            OutImage = null;
            GetOutParams();
        }
        #endregion

        public override void Run()
        {
            inputImageInfo.Image = InImage;
            if (inParams.Keys.FirstOrDefault(x => inParams[x] == null) != null) return;
            if (inParams.Values.FirstOrDefault(x => x.Value == null) != null) return;
            DateTime lastProcessTimeStart = DateTime.Now;
            try
            {
                if (!AiO.IsPossibleImage(InImage)) throw new Exception("InputImage = Null");
                if (OutImage != null) OutImage.Dispose();
                OutImage = InImage.Clone(true);
                Mat findRegion = OutImage.Mat.SubMat((int)RunParams.SearchRegion.Y, (int)RunParams.SearchRegion.Y + (int)RunParams.SearchRegion.Height,
                                                     (int)RunParams.SearchRegion.X, (int)RunParams.SearchRegion.X + (int)RunParams.SearchRegion.Width);
                //AiO.ShowImage(findRegion, 1);
                double min, max;
                CVAiO2.MinMaxLoc(findRegion, out min, out max);
                Scalar mean, dev;
                CVAiO2.MeanStdDev(findRegion, out mean, out dev);
                CVAiO2.Threshold(OutImage.Mat, OutImage.Mat, (int)(mean.ToDouble()- 3 * dev.ToDouble()), (int)(mean.ToDouble() + 3 * dev.ToDouble()), ThresholdTypes.Tozero);

                outputImageInfo.Image = OutImage;
                RunStatus = new RunStatus(EToolResult.Accept, "Succcess", DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, null);
            }
            catch (Exception ex)
            {
                RunStatus = new RunStatus(EToolResult.Error, ex.ToString());
            }
        }
        public override void Reset()
        {
            RunStatus = new RunStatus(EToolResult.Waiting, "Tool has not run yet. Please Run Tool first. ");
        }

        public override void Dispose()
        {
            InitOutProperty();
            if (InImage != null)
            {
                InImage.Dispose();
            }
        }
    }
}
