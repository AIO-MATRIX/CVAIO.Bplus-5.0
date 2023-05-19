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
    public partial class AlgoInspectAIEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion

        #region Properties
        #endregion

        public AlgoInspectAIEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += AlgoInspectAIEdit_SubjectChanged;
        }
        private void AlgoInspectAIEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as AlgoInspectAI;
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AlgoInspectAI Subject
        {
            get { return base.GetSubject() as AlgoInspectAI; }
            set { base.SetSubject(value); }
        }

        private void InitBinding()
        {
            source = new BindingSource(typeof(AlgoStandard), null);
        }
    }
}
