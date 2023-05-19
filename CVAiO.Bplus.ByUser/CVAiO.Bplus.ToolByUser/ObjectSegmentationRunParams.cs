using CVAiO.Bplus.Core;
using CVAiO.Bplus.OpenCV;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CVAiO.Bplus.ToolByUser
{
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class ObjectSegmentationRunParams : RunParams, ISerializable
    {
        #region Fields
        private float threshold;
        #endregion

        #region Properties
        [Description("Iteration"), PropertyOrder(32)] // Description: giải thích về thuộc tính sẽ hiển thị ở của số Properties, PropertyOrder: Vị trí của thuộc tính trong danh sách các thuộc tính
        public float Threshold { get => threshold; set { if (threshold == value || value < 0 || value > 1) return; threshold = value; NotifyPropertyChanged(nameof(Threshold)); } }

        #endregion
        public ObjectSegmentationRunParams()
        {
            // khởi tạo giá trị ban đầu cho các trường
            threshold = 0.4f;
        }

        #region default
        // các funtion sử dụng trong quá trình save và load thông tin của các params
        // chú ý sửa kiểu dữ liệu trả ra của hàm Clone()
        public ObjectSegmentationRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public ObjectSegmentationRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (ObjectSegmentationRunParams)runParams;
            }
        }
        #endregion
    }
}
