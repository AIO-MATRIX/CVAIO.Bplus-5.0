using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVAiO.Bplus.ToolByUser
{
    public class YoloLabel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public YoloLabelKind Kind { get; set; }

        public Color Color { get; set; }

        public YoloLabel() => Color = Color.Yellow;
    }
}
