using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace AppceleratorProxy.Infrastructure
{
    public class IsolatedStorageRepository<T>
    {
        public bool Exists(string key)
        {
            using (var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                return isoStore.FileExists(key);
            }
        }

        public T Read(string key)
        {
            using (var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                using (var isoStream = new IsolatedStorageFileStream(key, FileMode.Open, isoStore))
                {
                    using (var reader = new StreamReader(isoStream))
                    {
                        var serializer = new XmlSerializer(typeof(T));
                        return (T) serializer.Deserialize(reader);
                    }
                }
            }
        }

        public void Write(string key, T item)
        {
            using (var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                using (var isoStream = new IsolatedStorageFileStream(key, FileMode.Create, isoStore))
                {
                    using (var writer = new StreamWriter(isoStream))
                    {
                        var serializer = new XmlSerializer(typeof(T));
                        serializer.Serialize(writer, item);
                    }
                }
            }
        }
    }
}
