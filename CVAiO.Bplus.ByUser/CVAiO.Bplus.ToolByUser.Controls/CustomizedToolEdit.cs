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
    public partial class CustomizedToolEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion

        public CustomizedToolEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += ImageBlurEdit_SubjectChanged;
        }
        private void ImageBlurEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as CustomizedTool;
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CustomizedTool Subject
        {
            get { return base.GetSubject() as CustomizedTool; }
            set { base.SetSubject(value); }
        }
        private void InitBinding()
        {
            source = new BindingSource(typeof(CustomizedTool), null);
        }
    }
}
