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
    public class AlgoStandard : ToolBase, IAlgorithm
    {
        #region Fields
        [NonSerialized]
        private Image inImage;

        [NonSerialized]
        private Image outImage;
        private ImageInfo inputImageInfo;
        private ImageInfo outputImageInfo;

        private AlgoStandardRunParams runParams;

        [NonSerialized]
        private Execution calc;
        [NonSerialized]
        private bool algoJudgement;

        private int valueCount;
        [NonSerialized]
        private float value0;
        [NonSerialized]
        private float value1;
        [NonSerialized]
        private float value2;
        [NonSerialized]
        private float value3;
        [NonSerialized]
        private float value4;
        [NonSerialized]
        private float value5;
        [NonSerialized]
        private float value6;
        [NonSerialized]
        private float value7;
        [NonSerialized]
        private float value8;
        [NonSerialized]
        private float value9;

        [NonSerialized]
        private bool inspectionResult;

        #endregion

        #region Properties
        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image InImage { get => inImage; set => inImage = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Value0 { get => value0; set => value0 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Value1 { get => value1; set => value1 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Value2 { get => value2; set => value2 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Value3 { get => value3; set => value3 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Value4 { get => value4; set => value4 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Value5 { get => value5; set => value5 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Value6 { get => value6; set => value6 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Value7 { get => value7; set => value7 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Value8 { get => value8; set => value8 = value; }

        [InputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Value9 { get => value9; set => value9 = value; }

        [Description("Number of Input Values (0 ~ 10)"), Category("3. Tool Params"), PropertyOrder(20), ReadOnly(true)]
        public int ValueCount
        {
            get => valueCount;
            set
            {
                if (valueCount == value || value < 0 || value > 10) return;
                valueCount = value;
                if (2 * valueCount > RunParams.CustomProperties.Count)
                {
                    for (int i = RunParams.CustomProperties.Count / 2; i < valueCount; i++)
                    {
                        RunParams.CustomProperties.Add(new PropertyEx("Upper Value " + i.ToString(), 100, false, true));
                        RunParams.CustomProperties.Add(new PropertyEx("Lower Value " + i.ToString(), -100, false, true));
                    }
                }
                else
                {
                    for (int i = RunParams.CustomProperties.Count; i > 2 * valueCount; i--)
                    {
                        RunParams.CustomProperties.RemoveAt(i - 1);
                    }
                }
                NotifyPropertyChanged(nameof(ValueCount));
            }
        }
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Run Parameters"), Category("3. Tool Params"), PropertyOrder(21)]
        public AlgoStandardRunParams RunParams
        {
            get
            {
                if (runParams == null) runParams = new AlgoStandardRunParams();
                return runParams;
            }
            set => runParams = value;
        }

        [InputParam, Browsable(false)]
        public bool InspectionResult { get => inspectionResult; set => inspectionResult = value; }

        [InputParam, Browsable(false)]
        public Execution Calc { get => calc; set => calc = value; }

        [OutputParam, Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image OutImage { get => outImage; set => outImage = value; }

        [Browsable(false)]
        public bool AlgoJudgement { get => algoJudgement; set => algoJudgement = value; }

        #endregion

        public AlgoStandard()
        {
            toolName = "AlgoStandard";
            toolGroup = "Vision Algorithm"; // Don't change tool Group
            name = "AlgoStandard";
            ToolColor = System.Drawing.Color.Orange;
            RunParams.PropertyChanged += RunParams_PropertyChanged;
            ValueCount = 0;
        }

        public AlgoStandard(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RunParams.PropertyChanged += RunParams_PropertyChanged;
        }

        #region Init
        public override void InitInParams()
        {
            inParams.Add("InImage", null);
            inParams.Add("Value0", null);
            inParams.Add("Value1", null);
            inParams.Add("InspectionResult", null);
            inParams.Add("Calc", null);
        }
        public override void InitOutParams()
        {
        }
        public override void InitImageList()
        {
            inputImageInfo = new ImageInfo(string.Format("[{0}] InputImage", this.ToString()));
            outputImageInfo = new ImageInfo(string.Format("[{0}] OutputImage", this.ToString()));
            outputImageInfo.drawingFunc += DrawOutputs;
            imageList.Add(inputImageInfo);
            imageList.Add(outputImageInfo);
        }
        public void DrawOutputs(DisplayView display)
        {
            if (display.Image == null) return;
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
                if (inImage == null || inImage.Width == 0 || inImage.Height == 0)
                    throw new Exception("InputImage = Null");
                if (inParams.Keys.Count(x => x.Contains("Value")) != valueCount)
                {
                    ValueCount = inParams.Keys.Count(x => x.Contains("Value"));
                    throw new Exception("Input Value != Value Count");
                }

                if (OutImage != null) OutImage.Dispose();
                OutImage = InImage.Clone(true);
                outputImageInfo.Image = OutImage;
                algoJudgement = true;
                // Kiểm tra tất cả các giá trị đưa vào có nằm trong Spec cho phép hay không
                for (int i = 0; i < 10; i++)
                    if (inParams.ContainsKey(string.Format("Value{0}", i)))
                    {
                        PropertyEx upper = RunParams.CustomProperties.FirstOrDefault(x => x.Name.Contains(string.Format("Upper Value {0}", i)));
                        PropertyEx lower = RunParams.CustomProperties.FirstOrDefault(x => x.Name.Contains(string.Format("Lower Value {0}", i)));
                        float value = (float)this.GetType().GetProperty(string.Format("Value{0}", i)).GetValue(this);
                        if (upper != null && lower != null)
                        {
                            if (value < lower.Value || value > upper.Value) algoJudgement = false;
                        }
                        else
                            throw new Exception(string.Format("Value0: Could not find Upper/Lower Value {0}", i));
                    }
                if (inParams.ContainsKey("InspectionResult"))
                    algoJudgement = inspectionResult;
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
    public class AlgoStandardRunParams : RunParams, ISerializable, ICustomTypeDescriptor
    {
        #region Fields
        private List<PropertyEx> customProperties;
        #endregion

        #region Properties
        [Browsable(false)]
        public List<PropertyEx> CustomProperties { get { if (customProperties == null) customProperties = new List<PropertyEx>(); return customProperties; } set => customProperties = value; }
        #endregion
        public AlgoStandardRunParams()
        {
        }

        #region Do not change
        public AlgoStandardRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public AlgoStandardRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (AlgoStandardRunParams)runParams;
            }
        }
        #endregion


        /// <summary>
		/// Add CustomProperty to Collectionbase List
		/// </summary>
		/// <param name="Value"></param>
		public void Add(PropertyEx Value)
        {
            customProperties.Add(Value);
        }

        /// <summary>
        /// Remove item from List
        /// </summary>
        /// <param name="Name"></param>
        public void Remove(string Name)
        {
            foreach (PropertyEx prop in customProperties)
            {
                if (prop.Name == Name)
                {
                    customProperties.Remove(prop);
                    return;
                }
            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        public PropertyEx this[int index]
        {
            get
            {
                return (PropertyEx)customProperties[index];
            }
            set
            {
                customProperties[index] = (PropertyEx)value;
            }
        }

        #region "TypeDescriptor Implementation" https://www.codeproject.com/Articles/9280/Add-Remove-Items-to-from-PropertyGrid-at-Runtime
        /// <summary>
        /// Get Class Name
        /// </summary>
        /// <returns>String</returns>
        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// GetAttributes
        /// </summary>
        /// <returns>AttributeCollection</returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        /// GetComponentName
        /// </summary>
        /// <returns>String</returns>
        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        /// GetConverter
        /// </summary>
        /// <returns>TypeConverter</returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        /// GetDefaultEvent
        /// </summary>
        /// <returns>EventDescriptor</returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        /// GetDefaultProperty
        /// </summary>
        /// <returns>PropertyDescriptor</returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        /// GetEditor
        /// </summary>
        /// <param name="editorBaseType">editorBaseType</param>
        /// <returns>object</returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (customProperties == null || customProperties.Count == 0) return null;
            PropertyDescriptor[] newProps = new PropertyDescriptor[customProperties.Count];

            for (int i = 0; i < customProperties.Count; i++)
            {
                PropertyEx prop = (PropertyEx)this[i];
                Attribute[] attributesEx = new Attribute[attributes.Length + 2];
                for (int j = 0; j < attributes.Length; j++)
                    attributesEx[j] = attributes[j];
                attributesEx[attributesEx.Length - 2] = new PropertyOrderAttribute(i);
                attributesEx[attributesEx.Length - 1] = new DescriptionAttribute(string.Format("Value {0} Upper/Lower Limit", i));
                newProps[i] = new PropertyDescriptorEx(ref prop, attributesEx);
            }

            return new PropertyDescriptorCollection(newProps);
        }

        public PropertyDescriptorCollection GetProperties()
        {

            return TypeDescriptor.GetProperties(this, true);

        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }

}
