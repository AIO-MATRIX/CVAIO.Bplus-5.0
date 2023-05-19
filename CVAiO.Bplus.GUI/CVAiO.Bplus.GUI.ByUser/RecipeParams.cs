using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CVAiO.Bplus.Core;

namespace CVAiO.Bplus.GUI.ByUser
{
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public class RecipeParams : INotifyPropertyChanged
    {
        #region Field
        private string name; // Don't delete this field
        private float xlimit;
        private float ylimit;
        private float tlimit;
        private float length;
        private float tolorance;
        #endregion

        #region Property
        [Category("1. Recipe"), PropertyOrder(11), Description("Recipe Name")] // Property Order named from 20-49
        public string Name { get { return name; } set { if (name == value) return; name = value; NotifyPropertyChanged(nameof(Name)); } }

        [Category("1. Recipe"), PropertyOrder(12), Description("Xlimit")] // Property Order named from 20-49
        public float Xlimit { get => xlimit; set => xlimit = value; }
        [Category("1. Recipe"), PropertyOrder(13), Description("Ylimit")] 
        public float Ylimit { get => ylimit; set => ylimit = value; }
        [Category("1. Recipe"), PropertyOrder(14), Description("Tlimit")] 
        public float Tlimit { get => tlimit; set => tlimit = value; }
        [Category("1. Recipe"), PropertyOrder(15), Description("Length")] 
        public float Length { get => length; set => length = value; }
        [Category("1. Recipe"), PropertyOrder(16), Description("Tolorance")]
        public float Tolorance { get => tolorance; set => tolorance = value; }

        #endregion

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            LogWriter.Instance.LogSystem(string.Format("{0} - Parameter changed: {1} {2}", this.GetType().ToString(), propertyName, this.GetType().GetProperty(propertyName).GetValue(this).ToString()));
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public RecipeParams()
        {
        }
        public override string ToString()
        {
            return "";
        }
    }
}
