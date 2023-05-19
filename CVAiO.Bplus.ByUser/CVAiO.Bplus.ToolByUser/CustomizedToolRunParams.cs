using CVAiO.Bplus.Core;
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
    public class CustomizedToolRunParams : RunParams, ISerializable
    {
        #region Fields
        private int paramInt = 1;
        private double paramDouble = 1.0;
        private bool paramBool = true;
        private float paramFloat = 2.0f;
        #endregion

        #region Properties

        [Description("Please define all the parameter needed for tool processing"), Category("Tool By User")]
        public int ParamInt { get => paramInt; set => paramInt = value; }
        [Description("Please define all the parameter needed for tool processing"), Category("Tool By User")]
        public double ParamDouble { get => paramDouble; set => paramDouble = value; }
        [Description("Please define all the parameter needed for tool processing"), Category("Tool By User")]
        public bool ParamBool { get => paramBool; set => paramBool = value; }
        [Description("Please define all the parameter needed for tool processing"), Category("Tool By User")]
        public float ParamFloat { get => paramFloat; set => paramFloat = value; }

        #endregion
        public CustomizedToolRunParams()
        {
        }

        #region Do not change
        public CustomizedToolRunParams(SerializationInfo info, StreamingContext context)
        {
            Serializer.Deserializing(this, info, context);
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Serializer.Serializing(this, info, context);
        }
        public CustomizedToolRunParams Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                object runParams = formatter.Deserialize(stream);
                stream.Dispose();
                return (CustomizedToolRunParams)runParams;
            }
        }
        #endregion
    }
}
