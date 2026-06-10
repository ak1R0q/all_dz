using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace second_dz
{
    public class Journal<T> where T : IJournalEntry
    {
        private readonly List<T> _entries = new List<T>();

        public void Add(T entry)
        {
            _entries.Add(entry);
        }

        public IReadOnlyList<T> GetAll()
        {
            return _entries.AsReadOnly();
        }

        public void SaveToFile(string path)
        {
            var lines = new List<string>();
            foreach (var entry in _entries)
            {
                lines.Add(entry.ToLogLine());
            }
            File.WriteAllLines(path, lines, Encoding.UTF8);
        }

        public void LoadFromFile(string path, Func<string, T> fromLogLine)
        {
            if (!File.Exists(path))
                return;

            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    T entry = fromLogLine(line);
                    Add(entry);
                }
            }
        }
    }
}