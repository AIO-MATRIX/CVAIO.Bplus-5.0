using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVAiO.Bplus.Simulator
{
    public interface IInterface : IDisposable
    {
        bool OpenDevice();
        bool CloseDevice();
        bool IsOpen { get; }
        bool SetOutValue(int _index, bool _value, bool _select = true);
        bool SetInValue(int _index, bool _value);
        bool GetInValue(int _index);
        bool GetOutValue(int _index);
        bool WriteValue(int index, int value);
        bool WriteValue(int index, short value);
        bool WriteValue(int index, float value);
        bool WriteValue(int index, string value);
        bool WriteValues(int index, List<float> values);
        bool ReadValue(int index, out int value);
        bool ReadValue(int index, out float value);
        bool ReadValue(int index, out string value);
        Dictionary<int, bool> InIO { get; }
        Dictionary<int, bool> OutIO { get; }
        bool IsConnected { get; }
    }
}
