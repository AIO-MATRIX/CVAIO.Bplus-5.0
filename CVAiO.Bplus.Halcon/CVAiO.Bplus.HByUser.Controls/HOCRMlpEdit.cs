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
        }
        private void HOCRMlpEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as HOCRMlp;
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
