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
    public partial class AlgoPlateInspectionEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion

        #region Properties
        #endregion

        public AlgoPlateInspectionEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += AlgoPlateInspectionEdit_SubjectChanged;
        }
        private void AlgoPlateInspectionEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as AlgoPlateInspection;
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AlgoPlateInspection Subject
        {
            get { return base.GetSubject() as AlgoPlateInspection; }
            set { base.SetSubject(value); }
        }

        private void InitBinding()
        {
            source = new BindingSource(typeof(AlgoPlateInspection), null);
        }
    }
}
