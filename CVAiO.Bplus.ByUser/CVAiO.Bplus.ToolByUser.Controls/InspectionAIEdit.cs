using CVAiO.Bplus.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CVAiO.Bplus.ToolByUser.Controls
{
    public partial class InspectionAIEdit :ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion
        public InspectionAIEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += InspectionAIEdit_SubjectChanged;
            this.DisplayViewEdit.SelectedImageChanged += DisplayViewEdit_SelectedImageChanged;
            this.DisplayViewEdit.DisplayViewInteract.DrawObjectChanged += DisplayViewInteract_DrawObjectChanged;
        }

        private void InspectionAIEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as InspectionAI;
        }

        private void DisplayViewInteract_DrawObjectChanged(object sender, InteractDrawObject drawObject)
        {
            ToolProperty.Refresh();
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public InspectionAI Subject
        {
            get { return base.GetSubject() as InspectionAI; }
            set { base.SetSubject(value); }
        }

        private void InitBinding()
        {
            source = new BindingSource(typeof(InspectionAI), null);
        }

        private void DisplayViewEdit_SelectedImageChanged(object sender, ImageInfo imageInfo)
        {
            if (imageInfo.ImageName.Contains("InputImage"))
            {
                if (Subject.InImage == null) return;
            }
        }

        private void btnOnnxFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.InitialDirectory = @"D:\";
            fd.Filter = "Onnx file (*.onnx)|*.onnx";
            fd.FilterIndex = 1;
            fd.Multiselect = true;
            if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            Subject.RunParams.OnnxFile = fd.FileName;
            ToolProperty.Refresh();
        }

        private void btnClassFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.InitialDirectory = @"D:\";
            fd.Filter = "Class file (*.txt)|*.txt";
            fd.FilterIndex = 1;
            fd.Multiselect = true;
            if (fd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            Subject.RunParams.ClassFile = fd.FileName;
            ToolProperty.Refresh();
        }
    }
}
