using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PokerstarsAutoNotes.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonSerializer
    {
        /// <summary>
        /// Serializes a given object into a json string
        /// </summary>
        /// <typeparam name="T">Type to serialize</typeparam>
        /// <param name="objectToSerialize">Object to serialize</param>
        /// <returns>Serialized Json string of the given object</returns>
        public static string SerializeToJson<T>(T objectToSerialize) where T : class
        {
            if (objectToSerialize == null)
            {
                throw new ArgumentException("objectToSerialize must not be null");
            }
            MemoryStream ms = null;
            try
            {
                var serializer = new DataContractJsonSerializer(objectToSerialize.GetType());
                ms = new MemoryStream();
                serializer.WriteObject(ms, objectToSerialize);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                }
            }
        }

        /// <summary>
        /// Deserialize a given json string
        /// </summary>
        /// <typeparam name="T">Type to deserialize</typeparam>
        /// <param name="jsonStringToDeserialize">Json String representation</param>
        /// <returns>Deserialized object</returns>
        public static T DeserializeFromJson<T>(string jsonStringToDeserialize) where T : class
        {
            if (string.IsNullOrEmpty(jsonStringToDeserialize))
            {
                throw new ArgumentException("jsonStringToDeserialize must not be null");
            }
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonStringToDeserialize));
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
            finally
            {
                if (ms != null)
                    ms.Close();
            }
        }
    }
}
