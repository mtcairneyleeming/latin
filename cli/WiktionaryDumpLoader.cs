using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using database;
using database.Database;
using database.Helpers;
using Category = database.Category;
using Gender = database.Gender;

namespace cli
{
    public class XmlExtendableReader : XmlWrappingReader
    {
        public XmlExtendableReader(Stream input, XmlReaderSettings settings, bool ignoreNamespace = false)
            : base(Create(input, settings))
        {
            IgnoreNamespace = ignoreNamespace;
        }

        private bool IgnoreNamespace { get; set; }

        public override string NamespaceURI => IgnoreNamespace ? string.Empty : base.NamespaceURI;
    }

    public static class WiktionaryDumpLoader
    {
        public static void Load()
        {
            var settings = new XmlReaderSettings {Async = true, IgnoreComments = true, IgnoreWhitespace = true};
            Console.WriteLine("Started");
            var wordsToProcess = new List<(string, string)>();
            var dataToAdd = new List<(string, (Part?, Category?, Gender?))>();
            using (var reader = new XmlExtendableReader(File.Open("./wikidata.xml", FileMode.Open), settings, true))
            {
                Console.WriteLine("Created reader");
                reader.ReadToFollowing("page");
                Console.WriteLine("Read to first page");
                Console.WriteLine($"{reader.Name}:{reader.Value}");
                var i = 0;
                while (!reader.EOF)
                {
                    if (reader.NodeType != XmlNodeType.Element && reader.Name != "page")
                    {
                        //Console.WriteLine($"Skipping to page:{reader.Name}.");
                        reader.ReadToFollowing("page");
                    }

                    if (reader.EOF)
                    {
                        Console.WriteLine("Finished loading file");
                        continue;
                    }

                    var page = (XElement) XNode.ReadFrom(reader);

                    var rev = page.Element("revision");
                    if (rev is null)
                    {
                        Console.WriteLine("<revision> was null");
                        Console.WriteLine(string.Join(", ", page.Elements().Select(e => e.Name)));
                        continue;
                    }

                    var text = rev.Element("text")?.Value ?? "";
                    if (text.Contains("==Latin=="))
                    {
                        //var tags = Regex.Matches(text, @"{{la-.+}}");
                        //Console.WriteLine($"{page.Element("title").Value}: {string.Join(", ", tags.Select(m => m.Value))}");
                        var title = page.Element("title");
                        if (!(title is null) && !title.Value.StartsWith("User:") && !title.Value.StartsWith("Wiktionary:") &&
                            !title.Value.StartsWith("Reconstruction:"))
                            wordsToProcess.Add((title.Value, text));
                    }

                    if (wordsToProcess.Count >= 1000)
                    {
                        Console.WriteLine($"Processed {i * 1000} Latin words");
                        i++;
                        var data = wordsToProcess.SelectMany(w => ProcessWord(w.Item1, w.Item2)).ToList();
                        dataToAdd.AddRange(data);
                        wordsToProcess.Clear();
                    }
                }
            }

            using (var context = new LatinContext())
            {
                AddDataToDb(dataToAdd, context);
            }
        }

        private static List<(string s, (Part? part, Category? cat, Gender? gend) data)> ProcessWord(string word, string pageText)
        {
            var tags = Regex.Matches(pageText, @"({{la-.+?}})|({{head\|la\|.+}})");
            var wordsToAdd = new List<(string lemma, (Part? part, Category? cat, Gender? gend) data)>();
            foreach (var tag in tags.Select(m => m.Value))
            {
                var data = ProcessTag(word, tag);
                if (data != (null, null, null))
                {
                    wordsToAdd.Add((word, data));
                }
            }

            return wordsToAdd;
        }

        private static (Part?, Category?, Gender?) ProcessTag(string word, string tag)
        {
            var processed = tag.Replace("{", "").Replace("}", "");
            var parts = processed.Split("|");
            if (parts[0].Contains("IPA") || parts[0].Contains("form")) return (null, null, null);


            switch (parts[0])
            {
                case "la-verb":
                    var verbDict = new Dictionary<string, Category>
                    {
                        {"1", Category.OneC},
                        {"2", Category.TwoC},
                        {"3", Category.ThreeC},
                        {"io", Category.ThreeC},
                        {"4", Category.FourC},
                        {"first", Category.OneC},
                        {"second", Category.TwoC},
                        {"third", Category.ThreeC},
                        {"fourth", Category.FourC},
                        {"irreg", Category.IrrC}
                    };
                    Category conj;

                    var tagData = parts.FirstOrDefault(q => q.StartsWith("conj=") || q.StartsWith("c="));
                    if (tagData is null)
                    {
                        conj = Category.IrrC;
                        Console.WriteLine($"{word} was given IrrC as no conjugation could be found!");
                    }
                    else
                        conj = verbDict[tagData.Split("=")[1].Split(" ")[0]];

                    return (Part.Verb, conj, null);
                case "la-noun":
                case "la-proper noun":
                case "la-location":
                    var nounDict = new Dictionary<string, Category>
                    {
                        {"first", Category.OneD},
                        {"second", Category.TwoD},
                        {"third", Category.ThreeD},
                        {"fourth", Category.FourD},
                        {"fifth", Category.FiveD},
                        {"irregular", Category.IrrD}
                    };
                    var nounGenderDict = new Dictionary<string, Gender>
                    {
                        {"m", Gender.Masculine},
                        {"f", Gender.Feminine},
                        {"n", Gender.Neuter},
                        {"c", Gender.Masculine},
                        {"?", Gender.Indeterminate}
                    };
                    var genderTag = parts.FirstOrDefault(q => q.StartsWith("g=") || q.StartsWith("gender="));

                    if (genderTag is null && parts.Length >= 4)
                        genderTag = parts[3].Split("-")[0]; //if(!nounGenderDict.TryGetValue(parts[3].Split("-")[0], out gender))

                    Gender gender = Gender.Indeterminate;
                    if (genderTag is null || !nounGenderDict.TryGetValue(genderTag, out gender))
                    {
                        var found = false;
                        foreach (var part in parts)
                        {
                            if (!nounGenderDict.ContainsKey(part)) continue;

                            gender = nounGenderDict[part];
                            found = true;
                        }

                        if (!found)
                        {
                            gender = Gender.Indeterminate;
                            Console.WriteLine($"{word} was given indeterminate gender as no gender data could be found!");
                        }
                    }

                    Category nounDecl = Category.IrrD;
                    if (processed.Contains("indecl=")) nounDecl = Category.IrrD;
                    else if (parts.Length < 5 || !nounDict.TryGetValue(parts[4], out nounDecl))
                    {
                        var found = false;
                        foreach (var part in parts)
                        {
                            if (!nounDict.ContainsKey(part)) continue;

                            nounDecl = nounDict[part];
                            found = true;
                        }

                        if (!found)
                        {
                            nounDecl = Category.IrrD;
                            Console.WriteLine($"{word} was given as IrrD because we couldn't find any other value!");
                        }
                    }


                    return (Part.Noun, nounDecl, gender);
                case "la-adj-1&2":
                    return (Part.Adjective, Category.TwoOneTwo, null);
                case "la-adj-3rd-1E":
                case "la-adj-3rd-2E":
                case "la-adj-3rd-3E":
                    return (Part.Adjective, Category.ThreeD, null);
                case "la-adj-comparative":
                    return (Part.Adjective, Category.ThreeD, null);
                case "la-adj-superlative":
                    return (Part.Adjective, Category.TwoOneTwo, null);
                case "la-adj-irr":
                    Console.WriteLine($"Should not exist!! word was {word}");
                    break;
                case "la-adv":
                    return (Part.Adverb, null, null);
                case "la-interj":
                    return (Part.Interjection, null, null);
                case "la-pronoun":
                    return (Part.Pronoun, null, null);
                case "la-prep":
                    return (Part.Preposition, null, null);
                case "head" when parts[2] == "conjunction":
                    return (Part.Conjunction, null, null);
                default:
                    var p = parts[0];
                    var knownAbout = new List<string>()
                    {
                        "Latin-decl", "la-perfect participle", "la-present participle", "la-future participle", "la-suffix-noun", "la-gerundive",
                        "la-phrase", "la-letter", "la-gerund", "la-num-1&2", "la-pronunc", "la-num-card", "la-punctuation-mark", "head",
                        "la-punctuation-mark"
                    };
                    if (!p.StartsWith("la-conj") && !p.StartsWith("la-decl") && !p.StartsWith("la-adecl") && !p.StartsWith("la-suffix") &&
                        !knownAbout.Contains(p))
                        Console.WriteLine($"Unknown tag name found - it was {parts[0]}, from the tag {tag} on the word {word}");
                    break;
            }

            return (null, null, null);
        }

        private static void AddDataToDb(IReadOnlyCollection<(string lemma, (Part? part, Category? cat, Gender? gend) data)> dataIn,
            LatinContext context)
        {
            var data = dataIn.Where(d => !(d.Item2.Item1 is null));
            var ambiguous = new List<(string lemma, (Part? part, Category? cat, Gender? gend) data)>();
            var notFound = new List<string>();

            var contextLemmas = context.Lemmas.ToList();

            Console.WriteLine("Adding data to db");
            var i = 0;
            foreach (var (str, (part, cat, gender)) in data)
            {
                var possibleLemmas = contextLemmas.Where(l => l.LemmaText == str).ToList();
                switch (possibleLemmas.Count)
                {
                    case 0:
                        notFound.Add(str);
                        break;
                    case 1:
                        var lemma = possibleLemmas[0];
                        lemma.LemmaData.PartOfSpeechId = (int?) part;
                        lemma.LemmaData.CategoryId = (int?) cat;
                        lemma.LemmaData.GenderId = (int?) gender;
                        break;
                    default:
                        ambiguous.Add((str, (part, cat, gend: gender)));
                        break;
                }

                if (i % 1000 == 0) Console.WriteLine($"Working on {i} out of {dataIn.Count}");
                i++;
            }

            context.SaveChanges();
            Console.WriteLine($"Successfully saved data on {dataIn.Count - ambiguous.Count - notFound.Count} lemmas");
            Console.WriteLine($"{notFound.Count} lemmas were not found in the database");

            var groupedAmbiguities = ambiguous.GroupBy(l => l.lemma).Select(l => (l.Key, l.ToList().Select(r => r.data).ToList())).ToList();

            Console.WriteLine($"There were {groupedAmbiguities.Count} words that could apply to multiple lemmas: please choose the correct one:\n");

            var count = 0;
            foreach (var (lemma, possibilities) in groupedAmbiguities)
            {
                Console.WriteLine($"{count} out of {groupedAmbiguities.Count}");
                count++;
                var possibleLemmas = context.Lemmas.Where(l => l.LemmaText == lemma).ToList();
                if (possibilities.TrueForAll(o => o == possibilities[0]))
                {
                    possibleLemmas.ForEach(pl =>
                    {
                        pl.LemmaData.PartOfSpeechId = (int?) possibilities[0].part;
                        pl.LemmaData.CategoryId = (int?) possibilities[0].cat;
                        pl.LemmaData.GenderId = (int?) possibilities[0].gend;
                    });
                    context.SaveChanges();
                    continue;
                }

                var tableData = new string[possibilities.Count + 2 + possibleLemmas.Count][];

                tableData[0] = new[] {"Lemmas in DB:", "", "", ""};
                for (var index = 0; index < possibleLemmas.Count; index++)
                {
                    var poss = possibleLemmas[index];
                    var cat = poss.LemmaData.Category;
                    tableData[index + 1] = new[]
                    {
                        $"#{index + 1} {poss.LemmaShortDef.Truncate(30)}", poss.LemmaData.PartOfSpeech?.PartName ?? "",
                        $"{cat?.Number.ToString() ?? ""}{cat?.CategoryIdentifier ?? ""}", poss.LemmaData.Gender?.GenderCode ?? ""
                    };
                }

                tableData[possibleLemmas.Count + 1] = new[] {"Possibilities:", "", "", ""};
                for (var index = 0; index < possibilities.Count; index++)
                {
                    var (part, cat, gend) = possibilities[index];
                    tableData[index + possibleLemmas.Count + 2] = new[] {$"{(char) (index + 65)}.", part.ToString(), cat.ToString(), gend.ToString()};
                }


                var table = new AsciiTableGenerator(new[] {lemma, "Part of speech", "Category", "Gender"}, tableData);
                Console.WriteLine(table.Render());

//                Console.WriteLine($"    {lemma.Pad(4 + 2 + 20 + 14)}Part of speech   Category  Gender");
//                Console.WriteLine(
//                    $"        New value:                          {part.ToString().Pad(15)}  {category.ToString().Pad(8)}  {gender.ToString()}");
//                for (int j = 0; j < possibleLemmas.Count; j++)
//                {
//                    var poss = possibleLemmas[j];
//                    var cat = poss.LemmaData.Category;
//                    Console.WriteLine(
//                        $"        #{j + 1} {poss.LemmaShortDef.Pad(20)} - currently {(poss.LemmaData.PartOfSpeech?.PartName ?? "").Pad(15)}  {(cat?.Number.ToString() ?? "").Pad(1)}{(cat?.CategoryIdentifier ?? "").Pad(7)}  {poss.LemmaData.Gender?.GenderCode}");
//                }

                var success = false;
                while (!success)
                {
                    var input = Console.ReadLine();
                    if (input == "" && possibilities.Count == possibleLemmas.Count)
                    {
                        for (int j = 0; j < possibilities.Count; j++)
                        {
                            var pl = possibleLemmas[j];
                            pl.LemmaData.PartOfSpeechId = (int?) possibilities[j].part;
                            pl.LemmaData.CategoryId = (int?) possibilities[j].cat;
                            pl.LemmaData.GenderId = (int?) possibilities[j].gend;
                        }

                        success = true;
                    }
                    else
                    {
                        var parts = input.Split(" ");
                        var indexesUsed = new List<int>();
                        foreach (var part in parts)
                        {
                            var p = part.Trim();

                            if (!int.TryParse(p[0].ToString(), out var lemmaIndex) || lemmaIndex <= 0 || lemmaIndex > possibleLemmas.Count) continue;

                            if (indexesUsed.Contains(lemmaIndex)) continue;


                            if (p.Length < 2)
                            {
                                indexesUsed.Add(lemmaIndex);
                                continue;
                            }

                            var possChar = p[1];
                            var possIndex = possChar - 65;

                            if (possIndex < 0 || possIndex >= possibilities.Count) continue;

                            var pl = possibleLemmas[lemmaIndex - 1];
                            pl.LemmaData.PartOfSpeechId = (int?) possibilities[possIndex].part;
                            pl.LemmaData.CategoryId = (int?) possibilities[possIndex].cat;
                            pl.LemmaData.GenderId = (int?) possibilities[possIndex].gend;
                            indexesUsed.Add(lemmaIndex);
                        }

                        success = indexesUsed.Count == possibleLemmas.Count;
                        if (!success)
                        {
                            Console.WriteLine(
                                $"You must give each lemma data! You gave {string.Join(", ", parts.Select(p => p[0]))} for lemmas numbered from 1 to {possibleLemmas.Count}");
                        }
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}