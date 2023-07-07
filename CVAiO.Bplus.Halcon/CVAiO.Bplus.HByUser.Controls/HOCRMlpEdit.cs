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
using CVAiO.Bplus.Halcon;
using CVAiO.Bplus.OpenCV;

namespace CVAiO.Bplus.HByUser.Controls
{
    public partial class HOCRMlpEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion
        public HOCRMlpEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += HOCRMlpEdit_SubjectChanged;
            this.DisplayViewEdit.SelectedImageChanged += DisplayViewEdit_SelectedImageChanged;
            this.DisplayViewEdit.DisplayViewInteract.DrawObjectChanged += DisplayViewInteract_DrawObjectChanged;
        }
        private void HOCRMlpEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as HOCRMlp;
        }
        private void DisplayViewEdit_SelectedImageChanged(object sender, ImageInfo imageInfo)
        {
            if (imageInfo.ImageName.Contains("InputImage"))
            {
                if (Subject.InImage == null) return;
                DisplayViewEdit.DisplayViewInteract.Image = Subject.InImage;
                DisplayViewEdit.DisplayViewInteract.InteractiveGraphicsCollection.Add(Subject.RunParams.SearchRegion);
            }
        }
        private void DisplayViewInteract_DrawObjectChanged(object sender, InteractDrawObject drawObject)
        {
            ToolProperty.Refresh();
            DisplayViewEdit.DisplayViewInteract.GraphicsFuncCollection.Clear();
            Subject.DrawInputs(DisplayViewEdit.DisplayViewInteract);
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HOCRMlp Subject
        {
            get { return base.GetSubject() as HOCRMlp; }
            set { base.SetSubject(value); }
        }
        private void InitBinding()
        {
            source = new BindingSource(typeof(HOCRMlp), null);
        }
    }
}
