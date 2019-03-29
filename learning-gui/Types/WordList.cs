using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace learning_gui.Types
{
    public class WordList
    {
        private WordList(string path, string name, Dictionary<string, string> wordsWithDefinitions)
        {
            Path = path;
            Name = name;
            WordsWithDefinitions = wordsWithDefinitions;
        }

        private string Path { get; }
        private string Name { get; }

        public IEnumerable<string> Words => WordsWithDefinitions.Keys.ToList();

        public Dictionary<string, string> WordsWithDefinitions { get; }
        public List<string> Definitions => WordsWithDefinitions.Values.ToList();

        public void Save()
        {
            File.Delete($"{Path}.old");
            File.Move(Path, $"{Path}.old");
            var data = new List<string> {Name, "---"};
            data.AddRange(
                WordsWithDefinitions
                    .ToList()
                    .OrderBy(kvp => kvp.Key)
                    .Select(kvp => kvp.Value is null ? kvp.Key : $"{kvp.Key}:{kvp.Value}")
            );

            File.WriteAllLines(Path, data);
        }

        public static WordList Load(string path)
        {
            var data = File.ReadAllLines(path);
            var list = new WordList(path, data[0], data.Skip(2).Select(l =>
            {
                var pair = l.Split(":");
                return (pair[0], pair.Length == 1 ? null : pair[1]);
            }).ToDictionary(x => x.Item1, x => x.Item2));

            return list;
        }

        public void UpdateWord(string old, string newWord)
        {
            if (!WordsWithDefinitions.ContainsKey(old)) return;
            var def = WordsWithDefinitions[old];
            WordsWithDefinitions.Remove(old);
            WordsWithDefinitions.Add(newWord, def);
        }
    }
}