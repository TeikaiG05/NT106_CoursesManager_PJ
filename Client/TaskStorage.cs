using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NT106_BT2
{
    public static class TaskStorage
    {
        private static readonly Dictionary<DateTime, List<string>> _tasks
            = new Dictionary<DateTime, List<string>>();

        private static readonly string FilePath = "tasks.json";

        public static List<string> GetTasks(DateTime date)
        {
            date = date.Date;

            if (!_tasks.ContainsKey(date))
                _tasks[date] = new List<string>();

            return _tasks[date];
        }

        public static void AddTask(DateTime date, string task)
        {
            GetTasks(date).Add(task);
            SaveToFile();
        }

        public static void RemoveTask(DateTime date, string task)
        {
            GetTasks(date).Remove(task);
            SaveToFile();
        }
        public static void SaveToFile()
        {
            var data = new Dictionary<string, List<string>>();

            foreach (var kv in _tasks)
            {
                data[kv.Key.ToString("yyyy-MM-dd")] = kv.Value;
            }

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }
        public static void LoadFromFile()
        {
            if (!File.Exists(FilePath))
            {
                File.WriteAllText(FilePath, "{}");
                return;
            }

            string json = File.ReadAllText(FilePath);
            var data = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

            if (data == null) return;

            _tasks.Clear();
            foreach (var kv in data)
            {
                DateTime date = DateTime.Parse(kv.Key);
                _tasks[date] = kv.Value;
            }
        }
    }
}

