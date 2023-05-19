using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CVAiO.Bplus.Simulator
{
    public class Serializer
    {
        void CreateFilePath(string FilePath)
        {
            if (!File.Exists(FilePath))
            {
                string dc = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(dc))
                    Directory.CreateDirectory(dc);

                using (File.Create(FilePath)) { };
            }
        }
        public bool Serializing(string Path, Object Obj)
        {
            Stream memoryStream = new MemoryStream();
            try
            {
                CreateFilePath(Path);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, Obj);
                using (Stream fileStream = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    memoryStream.Position = 0;
                    memoryStream.CopyTo(fileStream);
                    fileStream.Dispose();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (memoryStream != null) memoryStream.Dispose();
            }
        }
        public Object Deserializing(string Path)
        {
            if (!File.Exists(Path)) return null;
            Object Obj = new Object();
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (Stream stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Obj = (Object)formatter.Deserialize(stream);
                    stream.Dispose();
                }
                return Obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
