using System;
using System.Diagnostics;
using System.Linq;
using database.Database;
using JsonFlatFileDataStore;
using learning_gui.DataSources;
using learning_gui.Helpers;
using learning_gui.Types;
using Terminal.Gui;

#pragma warning disable 4014

namespace learning_gui.Views
{
    public static class Welcome
    {
        private static bool Quit()
        {
            var n = MessageBox.Query(50, 7, "Quit", "Are you sure you want to quit this learning tool?", "Yes", "No");
            return n == 0;
        }

        public static void CreateWelcomeUI(Toplevel top)
        {
// Creates the top-level window to show
            var win = new Window("Learning")
            {
                ColorScheme = Colors.Base,
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };


            var menu = new MenuBar(new[]
            {
                new MenuBarItem("_File", new[]
                {
                    new MenuItem("_Quit", "", () =>
                    {
                        if (Quit()) top.Running = false;
                    })
                })
            });
            top.Add(menu);

            // load 


            var fileData = new FileListDataSource(
                $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/LatinLearning/filesCache.json");
            var list = new ListView(fileData)
            {
                X = 2,
                Y = 2,
                Width = Dim.Fill(),
                Height = Dim.Height(win) - 10,
                AllowsMarking = true
            };


            var addFileButton = new Button("Add vocab list")
            {
                Clicked = () =>
                {
                    var dialog = new OpenDialog("Vocab list", "Select a text file containing an appropriately-formatted vocab list")
                    {
                        AllowsMultipleSelection = true,
                        CanChooseFiles = true,
                        CanChooseDirectories = false,
                        CanCreateDirectories = false,
                        DirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Vocab",
                        AllowedFileTypes = new[] {"txt"}
                    };
                    Application.Run(dialog);
                    if (dialog.FilePaths.Any())
                        foreach (var path in dialog.FilePaths)
                            ((FileListDataSource) list.Source).AddItem(path);

                    list.SetNeedsDisplay();
                },
                X = 2,
                Y = 0,
                Width = 18,
                Height = 1
            };
            var removeFileButton = new Button("Remove a vocab list")
            {
                Clicked = () =>
                {
                    var entry = new TextField("")
                    {
                        X = 1,
                        Y = 5,
                        Width = Dim.Fill(),
                        Height = 1
                    };

                    void OkClicked()
                    {
                        if (entry.Text.ToString().Trim().ToLower() == "all")
                            for (var i = list.Source.Count - 1; i >= 0; i--)
                                ((FileListDataSource) list.Source).RemoveItem(i);

                        var parts = entry.Text.ToString().Split(",").Select(p => p.Trim()).ToList();
                        foreach (var part in parts)
                        {
                            var worked = !int.TryParse(part, out var index);
                            if (worked || index <= 0 || index > list.Source.Count) continue;
                            ((FileListDataSource) list.Source).RemoveItem(index - 1);
                        }

                        list.SetNeedsDisplay();

                        Application.RequestStop();
                    }

                    var ok = new Button("Ok")
                    {
                        Clicked = OkClicked
                    };
                    var cancel = new Button("Cancel")
                    {
                        Clicked = Application.RequestStop
                    };
                    var desc = new Label(
                        "Please enter the index(es) (1-based) of the file you \nwould like to remove, 'all' to remove all of them, \nor click Cancel to cancel this action")
                    {
                        X = 1,
                        Y = 1,
                        Height = 4
                    };

                    var dialog = new Dialog("Remove vocab list", 60, 14, ok, cancel) {desc, entry};
                    dialog.SetFocus(entry);
                    Application.Run(dialog);
                },
                X = Pos.Right(addFileButton) + 2,
                Y = 0,
                Width = 21,
                Height = 1
            };
            win.Add(addFileButton);
            win.Add(removeFileButton);
            win.Add(list);

            var checkBoxDataStore =
                new DataStore($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/LatinLearning/checkboxData.json");
            var toLatinCheckBox = new CheckBox("to Latin?")
            {
                X = 2,
                Y = Pos.Bottom(win) - 6,
                Width = 10,
                Height = 1,
                Disabled = false,
                Checked = checkBoxDataStore.GetItem<bool>("toLatin")
            };

            toLatinCheckBox.Toggled += (sender, args) =>
            {
                checkBoxDataStore.ReplaceItemAsync("toLatin", ((CheckBox) sender).Checked, true);
                Debug.WriteLine("Done");
            };
            win.Add(toLatinCheckBox);
            var skipUnknownWordsCheckBox = new CheckBox("ignore Unknown words?")
            {
                X = 16,
                Y = Pos.Bottom(win) - 6,
                Width = 30,
                Height = 1,
                Disabled = false,
                Checked = checkBoxDataStore.GetItem<bool>("skipUnknown")
            };
            skipUnknownWordsCheckBox.Toggled += (sender, args) =>
            {
                checkBoxDataStore.ReplaceItemAsync("skipUnknown", ((CheckBox) sender).Checked, true);
                Debug.WriteLine("Done");
            };

            win.Add(skipUnknownWordsCheckBox);
            var goButton = new Button("Go!")
            {
                X = 2,
                Y = Pos.Bottom(win) - 5,
                Width = 7,
                Height = 1,
                Clicked = () =>
                {
                    var selectedFiles = ((FileListDataSource) list.Source).Items.Where(i => i.Marked).Select(i => WordList.Load(i.FileName)).ToList();
                    if (!selectedFiles.Any())
                        return;

                    var learning = new Learning(new LatinContext(), selectedFiles);
                    //var learnWindow = learning.DisplayUI(top.Frame);


                    var newWindow = learning.DisplayUI(toLatinCheckBox.Checked, skipUnknownWordsCheckBox.Checked);

                    Application.Run(newWindow);
                }
            };
            win.Add(goButton);
            var progressButton = new Button("check Progress")
            {
                X = 10,
                Y = Pos.Bottom(win) - 5,
                Width = 18,
                Height = 1,
                Clicked = () =>
                {
                    var selectedFiles = ((FileListDataSource) list.Source).Items.Where(i => i.Marked).Select(i => WordList.Load(i.FileName)).ToList();
                    if (!selectedFiles.Any())
                        return;

                    var progress = new Progress(new LatinContext(), selectedFiles, skipUnknownWordsCheckBox.Checked);
                    var newWindow = progress.CreateUI();
                    Application.Run(newWindow);
                }
            };
            win.Add(progressButton);
            var checkDataButton = new Button("check Data")
            {
                X = 29,
                Y = Pos.Bottom(win) - 5,
                Width = 12,
                Height = 1,
                Clicked = () =>
                {
                    var selectedFiles = ((FileListDataSource) list.Source).Items.Where(i => i.Marked).Select(i => WordList.Load(i.FileName)).ToList();
                    if (!selectedFiles.Any())
                        return;

                    var listEditor = new ListEditor(new LatinContext(), selectedFiles, skipUnknownWordsCheckBox.Checked);
                    var newWindow = listEditor.CreateUI();
                    Application.Run(newWindow);
                }
            };
            win.Add(checkDataButton);
            var addDefinitionsButton = new Button("Import definitions")
            {
                X = 45,
                Y = Pos.Bottom(win) - 5,
                Width = 12,
                Height = 1,
                Clicked = () =>
                {
                    var selectedFiles = ((FileListDataSource) list.Source).Items.Where(i => i.Marked).Select(i => i.FileName).ToList();
                    if (!selectedFiles.Any())
                        return;
                    selectedFiles.ForEach(sf => FileHelpers.AddDefinitions(sf, skipUnknownWordsCheckBox.Checked));
                    MessageBox.Query(60, 10, "Definitions", "The definitions have been successfully imported.", "Close");
                    top.Add(win);
                }
            };
            win.Add(addDefinitionsButton);

            var closeButton = new Button("X")
            {
                Clicked = () => top.Running = false,
                X = Pos.Right(win) - 8,
                Y = 0,
                Width = 5,
                Height = 1
            };

            win.Add(closeButton);

            top.Add(win);
        }
    }
}