using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Wp8.Framework.Utils.Utils
{
    public sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }

    public static class Xml
    {
        public static string Serialize(object obj, Type[] extraTypes = null)
        {
            using (var sw = new Utf8StringWriter())
            {
                var type = obj.GetType();
                var serializer = extraTypes == null ? new XmlSerializer(type) : new XmlSerializer(type, extraTypes);
                serializer.Serialize(sw, obj);
                return sw.ToString();
            }
        }

        public static T Deserialize<T>(string xml, Type[] extraTypes = null)
        {
            using (var sw = new StringReader(xml))
            {
                var serializer = extraTypes == null
                                     ? new XmlSerializer(typeof(T))
                                     : new XmlSerializer(typeof(T), extraTypes);
                return (T)serializer.Deserialize(sw);
            }
        }

        public static Task<string> SerializeAsync(object obj, Type[] extraTypes = null)
        {
            var source = new TaskCompletionSource<string>();
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        source.SetResult(Serialize(obj, extraTypes));
                    }
                    catch (Exception ex)
                    {
                        source.SetException(ex);
                    }
                }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            return source.Task;
        }

        public static Task<T> DeserializeAsync<T>(string xml, Type[] extraTypes = null)
        {
            var source = new TaskCompletionSource<T>();
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        source.SetResult(Deserialize<T>(xml, extraTypes));
                    }
                    catch (Exception ex)
                    {
                        source.SetException(ex);
                    }
                }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            return source.Task;
        }
    }
}
