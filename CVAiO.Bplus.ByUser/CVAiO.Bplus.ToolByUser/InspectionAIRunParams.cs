using CVAiO.Bplus.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CVAiO.Bplus.ToolByUser
{
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class InspectionAIRunParams : RunParams, ISerializable
    {
        #region Fields
        private float threshold = 0.5f;
        private string onnxFile;
        private string classFile;
        private bool usingGPU;
        #endregion

        #region Properties

        [Description("Threshold for AI to confirm OK/NG"), Category("Inspection AI")]
        public float Threshold
        { 
            get => threshold;
            set
            {
                if (threshold == value || value < 0 || value > 1) return;
                threshold = value;
                NotifyPropertyChanged(nameof(Threshold));
            }
        }

        [Description("Model File - Onnx format"), Category("Inspection AI")]
        [Editor(typeof(OnnxFileSelectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OnnxFile 
        { 
            get => onnxFile;
            set
            {
                if (onnxFile == value || value == "") return;
                if (!File.Exists(value)) return;
                onnxFile = value;
                NotifyPropertyChanged(nameof(OnnxFile));
            }
        }

        [Description("Class File - txt format"), Category("Inspection AI")]
        [Editor(typeof(ClassFileSelectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ClassFile 
        { 
            get => classFile;
            set
            {
                if (classFile == value || value == "" ) return;
                if (!File.Exists(value)) return;
                classFile = value;
                NotifyPropertyChanged(nameof(ClassFile));
            }
        }

        [Description("Option to use/not use GPU"), Category("Inspection AI")]
        public bool UsingGPU 
        { 
            get => usingGPU;
            set
            {
                if (usingGPU == value) return;
                usingGPU = value;
                NotifyPropertyChanged(nameof(UsingGPU));
            }
        }

        #endregion
        public InspectionAIRunParams()
        {
        }

        #region Do not change
        public InspectionAIRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public InspectionAIRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (InspectionAIRunParams)runParams;
            }
        }
        #endregion
    }

    public class OnnxFileSelectionEditor : UITypeEditor
    {
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = null;
            if (provider != null)
            {
                editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }
            if (editorService != null)
            {
                OpenFileDialog fd = new OpenFileDialog();
                fd.InitialDirectory = @"D:\";
                fd.Filter = "Onnx file (*.onnx)|*.onnx";
                fd.FilterIndex = 1;
                fd.Multiselect = true;
                if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return "";
                value = fd.FileName;
            }
            return value;
        }
    }

    public class ClassFileSelectionEditor : UITypeEditor
    {
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = null;
            if (provider != null)
            {
                editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }
            if (editorService != null)
            {
                OpenFileDialog fd = new OpenFileDialog();
                fd.InitialDirectory = @"D:\";
                fd.Filter = "Class file (*.txt)|*.txt";
                fd.FilterIndex = 1;
                fd.Multiselect = true;
                if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return "";
                value = fd.FileName;
            }
            return value;
        }
    }
}
