using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CVAiO.Bplus.Core;
using CVAiO.Bplus.ToolByUser;

namespace CVAiO.Bplus.ToolByUser.Controls
{
    public partial class BallBondInspectionEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion

        public BallBondInspectionEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += BallBondInspectionEdit_SubjectChanged;
            this.DisplayViewEdit.SelectedImageChanged += DisplayViewEdit_SelectedImageChanged;
            this.DisplayViewEdit.DisplayViewInteract.DrawObjectChanged += DisplayViewInteract_DrawObjectChanged;
        }
        private void DisplayViewEdit_SelectedImageChanged(object sender, ImageInfo imageInfo)
        {
            if (imageInfo.ImageName.Contains("InputImage"))
            {
                if (Subject.InImage == null) return;
            }
        }
        private void DisplayViewInteract_DrawObjectChanged(object sender, InteractDrawObject drawObject)
        {
            ToolProperty.Refresh();
            DisplayViewEdit.DisplayViewInteract.GraphicsFuncCollection.Clear();
        }
        private void BallBondInspectionEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as BallBondInspection;
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BallBondInspection Subject
        {
            get { return base.GetSubject() as BallBondInspection; }
            set { base.SetSubject(value); }
        }
        private void InitBinding()
        {
            source = new BindingSource(typeof(BallBondInspection), null);
        }
    }
}
