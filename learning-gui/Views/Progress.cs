using System.Collections.Generic;
using System.Linq;
using database.Database;
using learning_gui.DataSources;
using learning_gui.Helpers;
using learning_gui.Types;
using Microsoft.EntityFrameworkCore;
using Terminal.Gui;

namespace learning_gui.Views
{
    public class Progress
    {
        public Progress(LatinContext context, IEnumerable<WordList> lists = null, bool ignoreUnknown = false)
        {
            var lemmas = lists is null
                ? context.Lemmas
                    .Include(l => l.UserLearntWord)
                    .Include(l => l.Definitions)
                    .Where(l => l.UserLearntWord != null)
                    .ToList()
                : FileHelpers.GenerateData(lists.ToList(), context, ignoreUnknown);

            Data.Items.AddRange(lemmas.Select(l => new ProgressListItem
            {
                LemmaName = l.LemmaText, ProgressLevel = l.UserLearntWord.RevisionStage,
                Definition = l.Definitions.FirstOrDefault()?.Data ?? l.LemmaShortDef
            }));
        }

        private ProgressListDataSource Data { get; } = new ProgressListDataSource();


        public Window CreateUI()
        {
            var window = new Window("Progress")
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
                Y = 3,
                Height = window.Height - 1,
                AllowsMarking = false
            };
            var sortAscButton = new Button("Sort ascending")
            {
                Clicked = () =>
                {
                    Data.SortAscending();
                    list.SetNeedsDisplay();
                },
                X = 47,
                Y = 1,
                Width = 18,
                Height = 1
            };
            var sortDescButton = new Button("sort Descending")
            {
                Clicked = () =>
                {
                    Data.SortDescending();
                    list.SetNeedsDisplay();
                },
                X = 65,
                Y = 1,
                Width = 19,
                Height = 1
            };
            var sortAlphaButton = new Button("sort Alphabetically")
            {
                Clicked = () =>
                {
                    Data.SortAlphabetically();
                    list.SetNeedsDisplay();
                },
                X = 1,
                Y = 1,
                Width = 23,
                Height = 1
            };
            var sortProgressButton = new Button("sort by Progress")
            {
                Clicked = () =>
                {
                    Data.SortByProgress();
                    list.SetNeedsDisplay();
                },
                X = 25,
                Y = 1,
                Width = 20,
                Height = 1
            };
            window.Add(sortAlphaButton);
            window.Add(sortProgressButton);
            window.Add(sortAscButton);
            window.Add(sortDescButton);


            window.Add(list);
            window.Add(closeButton);

            return window;
        }
    }
}