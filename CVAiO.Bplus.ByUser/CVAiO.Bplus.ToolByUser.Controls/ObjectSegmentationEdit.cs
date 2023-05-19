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
    // Kế thừa của ToolEditBase
    public partial class ObjectSegmentationEdit : ToolEditBase
    {
        #region Fields
        private BindingSource source;
        #endregion

        #region Properties

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObjectSegmentation Subject
        {
            get { return base.GetSubject() as ObjectSegmentation; }
            set { base.SetSubject(value); }
        }
        #endregion

        public ObjectSegmentationEdit()
        {
            InitializeComponent();
            InitBinding();
            this.SubjectChanged += ObjectSegmentationEdit_SubjectChanged;
            this.DisplayViewEdit.SelectedImageChanged += DisplayViewEdit_SelectedImageChanged;
            this.DisplayViewEdit.DisplayViewInteract.DrawObjectChanged += DisplayViewInteract_DrawObjectChanged;
        }
        
        private void ObjectSegmentationEdit_SubjectChanged(object sender, ToolBase tool)
        {
            source.DataSource = Subject;
            source.ResetBindings(true);
            ToolProperty.SelectedObject = Subject as ObjectSegmentation;
        }

        private void InitBinding()
        {
            source = new BindingSource(typeof(ObjectSegmentation), null);
            // thêm vào các Binding cần thiết ở đây
        }

        protected override void btnRun_Click(object sender, EventArgs e)
        {
            // Tác vụ cần thực hiện trước khi Run
            base.btnRun_Click(sender, e);
            // Tác vụ cần thực hiện sau khi Run
        }

        private void DisplayViewInteract_DrawObjectChanged(object sender, InteractDrawObject drawObject)
        {
            // Tác vụ nếu có sự thay đổi của các đối tượng tương tác trong hình
            ToolProperty.Refresh();
        }
        private void DisplayViewEdit_SelectedImageChanged(object sender, ImageInfo imageInfo)
        {
            // Tác vụ nếu có sự thay đổi hình ảnh hiển thị
        }
    }
}
