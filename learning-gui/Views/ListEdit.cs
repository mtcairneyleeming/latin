using System.Collections.Generic;
using System.Linq;
using database.Database;
using learning_gui.DataSources;
using learning_gui.Helpers;
using learning_gui.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Terminal.Gui;

namespace learning_gui.Views
{
    public class ListEditor
    {
        private readonly LatinContext _context;
        private readonly IEnumerable<WordList> _lists;

        public ListEditor(LatinContext context, IEnumerable<WordList> lists = null, bool ignoreUnknown = false)
        {
            _context = context;
            _lists = lists;

            GenerateListData(ignoreUnknown);
        }

        private EditListDataSource Data { get; } = new EditListDataSource();

        private void GenerateListData(bool ignoreUnknown = false)
        {
            var lemmas = _lists is null
                ? _context.Lemmas
                    .Include(l => l.UserLearntWord)
                    .Include(l => l.LemmaData)
                    .ThenInclude(ld => ld.Category)
                    .Include(l => l.LemmaData)
                    .ThenInclude(ld => ld.PartOfSpeech)
                    .Include(l => l.LemmaData)
                    .ThenInclude(ld => ld.Gender)
                    .Where(l => l.UserLearntWord != null)
                    .ToList()
                : FileHelpers.GenerateData(_lists.ToList(), _context, ignoreUnknown);

            Data.Items.AddRange(lemmas.Select(l => new EditListItem
            {
                LemmaName = l.LemmaText,
                EditLevel = l.UserLearntWord.RevisionStage,
                Category = l.LemmaData.Category?.Name,
                PartOfSpeech = l.LemmaData.PartOfSpeech?.PartName,
                Gender = l.LemmaData.Gender?.Name,
                LemmaId = l.LemmaId
            }));
        }


        public Window CreateUI()
        {
            var window = new Window("Edit")
            {
                X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill()
            };
            var closeButton = new Button("X")
            {
                Clicked = Application.RequestStop,
                X = Pos.Right(window) - 8,
                Y = 1,
                Width = 5,
                Height = 1
            };
            var list = new ListView(Data)
            {
                Width = Dim.Fill(),
                X = 1,
                Y = 5,
                Height = window.Height - 3,
                AllowsMarking = false
            };
            var possLabels = new[]
            {
                new[]
                {
                    "noun", "verb", "adj", "conj", "num", "prep", "pron", "adv", " "
                },
                new[]
                {
                    "1d", "2d", "3d", "4d", "5d", "212", "id", "1c", "2c", "3c", "4c", "ic", " "
                },
                new[] {"m", "f", "n", "i", " "}
            };

            var fieldSelect = new RadioGroup(1, 1, new[] {"Part", "Category", "Gender"})
            {
                SelectionChanged = (index) => { }
            };
            var valueInput = new TextField("")
            {
                X = 20, Y = 1, Height = 1, Width = 6
            };
            var saveButton = new Button("Save")
            {
                X = 30, Y = 1, Height = 1,
                Clicked = () =>
                {
                    var newVal = possLabels[fieldSelect.Selected].IndexOf(valueInput.Text.ToString());
                    if (newVal < 0)
                    {
                        MessageBox.ErrorQuery(80, 10, "Input error",
                            "Please enter a valid input for the new value of this field. It must be one of:\n1)"
                            + string.Join(", ", possLabels[0])
                            + "\n2)"
                            + string.Join(", ", possLabels[1])
                            + "\n3)"
                            + string.Join(", ", possLabels[2]),
                            "Close"
                        );
                        return;
                    }

                    var lemmas = _context.Lemmas.Where(l => Data.Items.Select(d => d.LemmaId).Contains(l.LemmaId));
                    switch (fieldSelect.Selected)
                    {
                        case 0:
                            foreach (var lemma in lemmas)
                            {
                                lemma.LemmaData.PartOfSpeechId = newVal + 1;
                                if (newVal == possLabels[fieldSelect.Selected].Length - 1)
                                {
                                    lemma.LemmaData.PartOfSpeechId = null;
                                }
                            }

                            break;
                        case 1:
                            foreach (var lemma in lemmas)
                            {
                                lemma.LemmaData.CategoryId = newVal + 1;

                                if (newVal == possLabels[fieldSelect.Selected].Length - 1)
                                {
                                    lemma.LemmaData.CategoryId = null;
                                }
                            }

                            break;
                        case 2:
                            foreach (var lemma in lemmas)
                            {
                                lemma.LemmaData.GenderId = newVal + 1;

                                if (newVal == possLabels[fieldSelect.Selected].Length - 1)
                                {
                                    lemma.LemmaData.GenderId = null;
                                }
                            }

                            break;
                    }

                    _context.SaveChanges();
                    Data.Items.Clear();
                    GenerateListData();
                    list.SetNeedsDisplay();
                }
            };
            window.Add(fieldSelect);
            window.Add(valueInput);
            window.Add(saveButton);

            window.Add(list);
            window.Add(closeButton);

            return window;
        }
    }
}