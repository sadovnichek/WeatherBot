using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBot
{
    public class DataBaseRecord
    {
        public long TelegramId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public TimeOnly TimeToWakeup { get; set; }

        public override  string ToString()
        {
            return $"{TelegramId} {Latitude} {Longitude} {TimeToWakeup}";
        }
    }

    public class DataBaseStub
    {
        private Dictionary<long, DataBaseRecord> data;
        private const string pathToDatabaseFile = "./users.txt";

        public DataBaseStub()
        {
            data = new Dictionary<long, DataBaseRecord>();
            if (!File.Exists(pathToDatabaseFile))
                File.Create(pathToDatabaseFile);
            else
                ReadDataFromFile();
        }

        public bool ContainsKey(long key)
        {
            return data.ContainsKey(key);
        }

        public DataBaseRecord Read(long key)
        {
            if (!data.ContainsKey(key))
                throw new KeyNotFoundException();
            return data[key];
        }

        public DataBaseRecord Update(long key, DataBaseRecord record)
        {
            if (!data.ContainsKey(key))
                data.Add(key, record);
            return data[key] = record;
        }

        public void Delete()
        {
            File.Delete(pathToDatabaseFile);
        }

        public void CloseConnection()
        {
            using (var writer = new StreamWriter(pathToDatabaseFile))
            {
                foreach (var kv in data)
                {
                    writer.WriteLine(kv.Value.ToString());
                }
            }
        }

        private void ReadDataFromFile()
        {
            using (var reader = new StreamReader(pathToDatabaseFile))
            {
                while(!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var tokens = line.Split();
                    var record = new DataBaseRecord()
                    {
                        TelegramId = long.Parse(tokens[0]),
                        Latitude = double.Parse(tokens[1]),
                        Longitude = double.Parse(tokens[2]),
                        TimeToWakeup = TimeOnly.Parse(tokens[3])
                    };
                    data.Add(record.TelegramId, record);
                }
            }
        }
    }
}
