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
    public partial class AlgoTapeInspectionEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion

        #region Properties
        #endregion

        public AlgoTapeInspectionEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += AlgoInspectAIEdit_SubjectChanged;
        }
        private void AlgoInspectAIEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as AlgoTapeInspection;
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AlgoTapeInspection Subject
        {
            get { return base.GetSubject() as AlgoTapeInspection; }
            set { base.SetSubject(value); }
        }

        private void InitBinding()
        {
            source = new BindingSource(typeof(AlgoTapeInspection), null);
        }
    }
}
