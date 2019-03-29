using System.Collections.Generic;
using System.Text;
using learning_gui.Types;

namespace learning_gui.Helpers
{
    public class MorphHelp
    {
        private static readonly Map<string, (int index, char value)> _morphCodePartMap = new Map<string, (int index, char value)>
        {
            {"fir", (1, '1')},
            {"sec", (1, '2')},
            {"thir", (1, '3')},

            {"sing", (2, 's')},
            {"plur", (2, 'p')},

            {"imp", (3, 'i')},
            {"fut", (3, 'f')},
            {"aor", (3, 'a')},
            {"pres", (3, 'p')},
            {"perf", (3, 'r')},
            {"plup", (3, 'l')},
            {"futper", (3, 't')},


            {"ind", (4, 'i')},
            {"subj", (4, 's')},
            {"inf", (4, 'n')},
            {"part", (4, 'p')},
            {"imper", (4, 'm')},
            {"supine", (4, 'u')},
            {"gerund", (4, 'd')},
            {"gerundive", (4, 'g')},

            {"act", (5, 'a')},
            {"dep", (5, 'd')},
            {"pass", (5, 'p')},
            {"medpas", (5, 'e')},

            {"masc", (6, 'm')},
            {"fem", (6, 'f')},
            {"neut", (6, 'n')},

            {"nom", (7, 'n')},
            {"gen", (7, 'g')},
            {"dat", (7, 'd')},
            {"acc", (7, 'a')},
            {"abl", (7, 'b')},
            {"voc", (7, 'v')},
            {"loc", (7, 'l')},
            {"ins", (7, 'i')},

            {"pos", (8, 'p')},
            {"comp", (8, 'c')},
            {"sup", (8, 's')}
        };

        private static readonly Dictionary<int, string> _morphCodeCharDescriptions = new Dictionary<int, string>
        {
            {0, "Part"},
            {1, "Person"},
            {2, "Number"},
            {3, "Tense"},
            {4, "Mood"},
            {5, "Voice"},
            {6, "Gender"},
            {7, "Case"},
            {8, "Degree"}
        };

        public static string GenerateMorphCodeFromDesc(string userInput)
        {
            var parts = userInput.Split(",");
            var morphCode = new StringBuilder("---------");


            foreach (var part in parts)
            {
                var wasData = _morphCodePartMap.TryGetValue(part.Trim(), out var indexAndChar);
                if (wasData) morphCode[indexAndChar.Item1] = indexAndChar.Item2;
            }

            return morphCode.ToString();
        }

        public static IEnumerable<string> GenerateDescFromMorphCode(string form, bool treatedParticipleAsAdj = false, bool includeGender = true)
        {
            var descriptions = new List<string>();
            if (treatedParticipleAsAdj)
            {
                var ivals = new[] {2, 6, 7};
                foreach (var i in ivals)
                {
                    var c = form[i];
                    if (c == '-' || i == 6 && !includeGender) continue;
                    if (_morphCodePartMap.TryGetValue((i, c), out var newDesc)) descriptions.Add(newDesc);
                }
            }
            else
            {
                for (var i = 0; i < form.Length; i++)
                {
                    var c = form[i];
                    if (c == '-' || i == 6 && !includeGender) continue;
                    if (_morphCodePartMap.TryGetValue((i, c), out var newDesc)) descriptions.Add(newDesc);
                }
            }

            return descriptions;
        }

        public static IEnumerable<string> DescribeMorphCode(string morphCode, bool includeGender = true)
        {
            var descriptions = new List<string>();
            for (var i = 1; i < morphCode.Length; i++)
            {
                if (morphCode[i] == '-' || i == 6 && !includeGender) continue;
                descriptions.Add(_morphCodeCharDescriptions[i]);
            }

            return descriptions;
        }
    }
}