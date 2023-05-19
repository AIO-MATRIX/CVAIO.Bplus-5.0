using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVAiO.Bplus.OpenCV;
using CVAiO.Bplus.Core;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;

namespace CVAiO.Bplus.Algorithm
{
    [Serializable]
    public class AlgoInspectAI : ToolBase, IAlgorithm
    {
        #region Fields
        [NonSerialized]
        private Image inImage;
        
        private ImageInfo inputImageInfo;
        //private ImageInfo outputImageInfo;

        private AlgoInspectAIRunParams runParams;

        [NonSerialized]
        private Execution calc;
        [NonSerialized]
        private bool algoJudgement;

        [NonSerialized]
        private bool inspResult;

        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InspResult { get => inspResult; set => inspResult = value; }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params")]
        public AlgoInspectAIRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new AlgoInspectAIRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [InputParam, Browsable(false), Description("Vision Algorithm Execute"), Category("4. Input"), PropertyOrder(11)]
        public Execution Calc { get => calc; set => calc = value; }

        [Browsable(false)]
        public bool AlgoJudgement { get => algoJudgement; set => algoJudgement = value; }

        #endregion

        public AlgoInspectAI()
        {
            toolName = "AlgoInspectAI";
            toolGroup = "Vision Algorithm"; // Don't change tool Group
            name = "AlgoInspectAI";
            ToolColor = System.Drawing.Color.Orange;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        public AlgoInspectAI(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        #region Init
        public override void InitInParams()
        {
            inParams.Add("InImage", null);
            inParams.Add("InspResult", null);
            inParams.Add("Calc", null);
        }
        public override void InitOutParams()
        {
        }
        public override void InitImageList()
        {
            inputImageInfo = new ImageInfo(string.Format("[{0}] InputImage", this.ToString()));
            //outputImageInfo = new ImageInfo(string.Format("[{0}] OutputImage", this.ToString()));
            imageList.Add(inputImageInfo);
            //imageList.Add(outputImageInfo);
        }
        public void DrawResult(DisplayView display)
        {
            if (display.Image == null) return;
        }
        public override void InitOutProperty()
        {
            RunStatus = new RunStatus(EToolResult.Waiting, "Tool has not run yet. Please Run Tool first. ");
            GetOutParams();
        }
        #endregion

        public override void Run()
        {
            inputImageInfo.Image = InImage;
            if (inParams.Keys.FirstOrDefault(x => inParams[x] == null) != null) return;
            if (inParams.Values.FirstOrDefault(x => x.Value == null) != null) return;
            DateTime lastProcessTimeStart = DateTime.Now;
            if (!AiO.IsPossibleImage(inImage)) throw new Exception("InputImage = Null");
            try
            {
                algoJudgement = inspResult; // kết quả phán định quá trình kiểm tra
                RunStatus = new RunStatus(EToolResult.Accept, "Succcess", DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, DateTime.Now.Subtract(lastProcessTimeStart).TotalMilliseconds, null);
                OnRan();
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

    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class AlgoInspectAIRunParams : RunParams, ISerializable
    {
        #region Fields
        #endregion

        #region Properties
        #endregion
        public AlgoInspectAIRunParams()
        {
        }

        #region Do not change
        public AlgoInspectAIRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public AlgoInspectAIRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (AlgoInspectAIRunParams)runParams;
            }
        }
        #endregion
    }
   
}
