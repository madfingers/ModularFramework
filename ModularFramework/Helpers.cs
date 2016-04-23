using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace ModularFramework {
    public static class SerializationHelper {
        public static void SerializeTo(string manifestFilePath, object @object) {
            using(Stream fileStream = File.Create(manifestFilePath)) {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(@object.GetType());
                serializer.WriteObject(fileStream, @object);
            }
        }
        public static T DeserializeFrom<T>(string objectFile) {
            T @object = default(T);
            if(!File.Exists(objectFile)) return default(T);
            using(Stream stream = new FileStream(objectFile, FileMode.Open, FileAccess.Read)) {
                var serializer = new DataContractJsonSerializer(typeof(T));
                @object = (T)serializer.ReadObject(stream);
            }
            return @object;
        }
    }
}
