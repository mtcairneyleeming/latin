using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using decliner.Database;
using Microsoft.Extensions.CommandLineUtils;

namespace cli
{
    public class DefinitionLoaders
    {
        public Action<CommandLineApplication> LoadDefinitions()
        {
            return command =>
            {
                command.Description = "Load the definitions for lemmas, in a file where each word & definition pair " +
                                      "is on its own line, split with the provided separator where that separator " +
                                      "does not appear in the data itself, at all";
                command.HelpOption("-h|--help");
                var levelArgument = command.Argument("[definitionLevel]",
                    "The level of the definition: 0 for KS3, 1 for GCSE, 2 for ALevel and 3 for dictionary");
                var fileArgument =
                    command.Argument("[filePath]", "The absolute path to the file you'd like to load");
                var separatorArgument =
                    command.Argument("[separator]", "The separator between word & definition");


                command.OnExecute(() =>
                {
                    var level = (DefinitionLevelEnum) Convert.ToInt32(levelArgument.Value);
                    var filepath = fileArgument.Value;
                    var separator = separatorArgument.Value;

                    var matchedLemmas = new List<string>();
                    var unmatchedLemmas = new List<string>();
                    List<(string word, string definition)> wordPairs;
                    try
                    {
                        wordPairs = LoadData(filepath, separator);
                    }
                    catch (InvalidDataException e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    using (var db = new LatinContext())
                    {
                        foreach (var pair in wordPairs)
                        {
                            var currentDefinition = db.Definitions.FirstOrDefault(d =>
                                d.Lemma.LemmaText == pair.word && d.Level == (int) level);
                            if (currentDefinition is null)
                            {
                                var lemma = db.Lemmas.FirstOrDefault(l => l.LemmaText == pair.word);
                                if (lemma is null)
                                {
                                    // Lemma can't be found
                                    unmatchedLemmas.Add(pair.word);
                                }
                                else
                                {
                                    var newDefinition = new Definition
                                    {
                                        Lemma = lemma,
                                        Level = (int) level,
                                        Data = pair.definition
                                    };
                                    db.Definitions.Add(newDefinition);
                                    db.SaveChanges();
                                    matchedLemmas.Add(pair.word);
                                }
                            }
                            else
                            {
                                currentDefinition.Data = pair.definition;
                                currentDefinition.Level = (int) level;

                                db.SaveChanges();
                                matchedLemmas.Add(pair.word);
                            }
                        }
                    }
                    Console.WriteLine($"{matchedLemmas.Count} lemmas matched and added");
                    Console.WriteLine($"{unmatchedLemmas.Count} lemmas not matched:");
                    foreach (var l in unmatchedLemmas)
                        Console.Write(l + "; ");
                    return 0;
                });
            };
        }

        public List<(string word, string definition)> LoadData(string filePath, string separator)
        {
            var data = new List<(string word, string definition)>();

            var rawdata = File.ReadAllLines(filePath);
            foreach (var line in rawdata)
            {
                var parts = line.Split(separator);
                if (parts.Length > 2)
                {
                    Console.WriteLine("The separator must not be present in the data. Aborting.");
                    throw new InvalidDataException("The separator must be present in the data");
                }
                data.Add((parts[0], parts[1]));
            }
            return data;
        }
    }
}