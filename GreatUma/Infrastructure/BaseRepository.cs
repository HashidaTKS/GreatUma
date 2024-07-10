using System.IO;

namespace GreatUma.Infrastructures
{
    public abstract class BaseRepository<T> where T : new()
    {
        protected string FilePath { get;}
        protected XmlSerializerWrapper<T> SerializerWrapper { get; } 

        public BaseRepository(string statusFilePath)
        {
            FilePath = statusFilePath;
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            SerializerWrapper = new XmlSerializerWrapper<T>(FilePath);
        }

        public virtual void Store(T status)
        {
            lock (this)
            {
                SerializerWrapper.Serialize(status);
            }
        }

        public virtual T ReadAll(bool getDefaultInstanceIfNull = false)
        {
            lock (this)
            {
                var result = SerializerWrapper.Deserialize();
                if(result == null)
                {
                    if (getDefaultInstanceIfNull)
                    {
                        return new T();
                    }
                }
                return result;
            }
        }
    }
}
