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
    public partial class AlgoStandardEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        private int valueCount;
        #endregion

        #region Properties
        public int ValueCount 
        { 
            get => valueCount; 
            set
            { 
                if (valueCount == value) return;
                valueCount = value; ToolProperty.Refresh(); 
                ToolProperty.ExpandAllGridItems(); 
            } 
        }
        #endregion

        public AlgoStandardEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += AlgoStandardEdit_SubjectChanged;
        }
        private void AlgoStandardEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as AlgoStandard;
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AlgoStandard Subject
        {
            get { return base.GetSubject() as AlgoStandard; }
            set { base.SetSubject(value); }
        }

        private void InitBinding()
        {
            source = new BindingSource(typeof(AlgoStandard), null);
            this.DataBindings.Add("ValueCount", source, "ValueCount", true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
