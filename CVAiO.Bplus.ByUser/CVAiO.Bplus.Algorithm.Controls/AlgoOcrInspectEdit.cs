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

namespace CVAiO.Bplus.Algorithm.Controls
{
    public partial class AlgoOcrInspectEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion

        #region Properties
        #endregion

        public AlgoOcrInspectEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += AlgoOcrInspectEdit_SubjectChanged;
        }
        private void AlgoOcrInspectEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as AlgoOcrInspect;
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AlgoOcrInspect Subject
        {
            get { return base.GetSubject() as AlgoOcrInspect; }
            set { base.SetSubject(value); }
        }

        private void InitBinding()
        {
            source = new BindingSource(typeof(AlgoOcrInspect), null);
        }
    }
}
