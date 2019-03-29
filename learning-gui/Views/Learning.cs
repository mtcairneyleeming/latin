using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using database.Database;
using database.Helpers;
using learning_gui.Helpers;
using learning_gui.Types;
using Terminal.Gui;

namespace learning_gui.Views
{
    public class Learning
    {
        private readonly LatinContext _context;
        private int _currentScore;
        private Label _scoreLabel;

        public Learning(LatinContext context, List<WordList> lists)
        {
            _context = context;
            Lists = lists;
        }

        private List<WordList> Lists { get; }
        private Lemma CurrentWord { get; set; }

        private int CurrentScore
        {
            get => _currentScore;
            set
            {
                _currentScore = value;
                if (!(_scoreLabel is null))
                    _scoreLabel.Text = $"score: {CurrentWord.UserLearntWord.RevisionStage.ToString().Pad(4)} (+ {value.ToString().Pad(4)})";
            }
        }

        private List<Lemma> Words { get; set; } = new List<Lemma>();


        private void CompleteLearn()
        {
            var index = Words.IndexOf(CurrentWord);
            Words[index].UserLearntWord.RevisionStage += CurrentScore;
            CurrentScore = 0;
            _context.SaveChanges();
        }


        private void CarryOutTest(View window, bool toLatin, Action<bool> finishTask)
        {
            if (CurrentWord is null)
            {
                Application.RequestStop();
                MessageBox.ErrorQuery(80, 5, "Error", "No words could be found to be learnt.", "Close");
                return;
            }


            var wordPart = CurrentWord.LemmaData?.PartOfSpeech;

            var definition = _context.Definitions.FirstOrDefault(d => d.LemmaId == CurrentWord.LemmaId)?.Data ?? CurrentWord.LemmaShortDef ?? "";

            var labelText = toLatin ? definition.Replace(Environment.NewLine, "") : CurrentWord.LemmaText + ": " + wordPart?.PartName;
            var lemmaLabel = new Label(labelText)
            {
                X = 2, Y = 0, Width = 20, Height = 2
            };
            window.Add(lemmaLabel);
            var wiktionaryLink = new Button("-> Wiktionary ")
            {
                X = labelText.Length + 3,
                Y = 0,
                Height = 1,
                Width = 20,
                Clicked = () => Process.Start(new ProcessStartInfo("cmd", $" /c start http://wiktionary.org/wiki/{CurrentWord.LemmaText}#Latin"))
            };


            var bottomPos = new List<int> {1};

            // meaning test
            var meaningFrame = GenerateMeaningQuestion(toLatin, CurrentWord, bottomPos.Last() + 1, out var currentBottomPos);
            window.Add(meaningFrame);
            bottomPos.Add(currentBottomPos);
            if (!(CurrentWord.LemmaData is null) && !(wordPart is null))
            {
                if (new[] {"Noun", "Verb", "Adjective"}.Contains(wordPart.PartName) && !(CurrentWord.LemmaData.Category is null))
                {
                    // declension/conjugation test
                    var optionsByPartOfSpeech = new Dictionary<int, int[]>
                    {
                        {1, new[] {1, 2, 3, 4, 5, 0}},
                        {2, new[] {1, 2, 3, 4, 0}},
                        {3, new[] {6, 3, 2}}
                    };
                    if (wordPart.PartId > 0 && wordPart.PartId < 4)
                    {
                        var options = optionsByPartOfSpeech[wordPart.PartId];
                        var optionNames = options.Select(o => _context.Category
                            .Single(c => c.Number == o &&
                                         c.CategoryIdentifier == (wordPart.PartName == "Verb" ? "V" : "D"))
                        ).Select(c => c.Name).ToList();

                        var catNum = CurrentWord.LemmaData.Category.Number;
                        var correctAnswerIndex = Array.IndexOf(options, catNum);
                        if (correctAnswerIndex == -1)
                            switch (wordPart.PartName)
                            {
                                case "Noun" when catNum == 6:
                                    correctAnswerIndex = Array.IndexOf(options, 2);
                                    break;
                                case "Adjective" when catNum == 2:
                                    correctAnswerIndex = Array.IndexOf(options, 6);
                                    break;
                            }

                        if (correctAnswerIndex != -1) // check for a second time, in case our checks failed
                        {
                            var generatedQuestionFrame = GenerateMultiChoiceQuestion(
                                optionNames,
                                correctAnswerIndex,
                                wordPart.PartName == "Verb" ? "Conjugation" : "Declension", bottomPos.Last() + 1, out currentBottomPos);
                            window.Add(generatedQuestionFrame);
                            bottomPos.Add(currentBottomPos);
                        }
                    }
                }

                // gender test
                if (wordPart.PartName == "Noun")
                {
                    // gender test
                    var options = new[] {"Masculine", "Feminine", "Neuter", "Indeterminate"};
                    if (!(CurrentWord.LemmaData.Gender is null))
                    {
                        var generatedQuestionFrame = GenerateMultiChoiceQuestion(options, Array.IndexOf(options, CurrentWord.LemmaData.Gender.Name),
                            "Gender",
                            bottomPos.Last() + 1,
                            out currentBottomPos);
                        window.Add(generatedQuestionFrame);
                        bottomPos.Add(currentBottomPos);
                    }
                }
            }

            // form test
            var repeatCount = wordPart?.PartName == "Verb" ? 3 : 1;
            var blacklist = new List<string>();
            for (var i = 0; i < repeatCount; i++)
            {
                var (form, success, treatedAdjAsParticiple) = LearningHelpers.SelectForm(CurrentWord, _context, blacklist);
                if (!success)
                    continue;
                blacklist.Add(form.Text);
                var formFrame = GenerateFormQuestion(toLatin, bottomPos.Last() + 1, form, treatedAdjAsParticiple, out currentBottomPos);
                if (!(formFrame is null))
                    window.Add(formFrame);
                bottomPos.Add(currentBottomPos);
            }


            window.Add(new Button("Finish")
            {
                X = 1,
                Y = bottomPos.Last() + 1,
                Width = 8,
                Height = 1,
                Clicked = () =>
                {
                    CompleteLearn();
                    finishTask(toLatin);
                    //Application.RequestStop();
                }
            });
            window.Add(wiktionaryLink);
        }

        private FrameView GenerateMeaningQuestion(bool toLatin, Lemma lemma, int yPos, out int bottomY)
        {
            var definition = _context.Definitions.FirstOrDefault(d => d.LemmaId == CurrentWord.LemmaId)?.Data ?? CurrentWord.LemmaShortDef;
            definition = definition.Replace(Environment.NewLine, "");
            var promptText = toLatin ? definition : lemma.LemmaText;

            bottomY = yPos + 6;
            var meaningFrame = new FrameView("Meaning")
            {
                X = 1,
                Y = yPos,
                Width = Dim.Fill(),
                Height = 6
            };

            meaningFrame.Add(new Label(1, 0, $"{promptText}:"));
            var answerBox = new TextField("")
            {
                X = 1,
                Y = 1,
                Width = 30
            };
            meaningFrame.Add(answerBox);
            var answerLabel = new Label("")
            {
                X = 18,
                Y = 2,
                Width = 40,
                Height = 2
            };
            meaningFrame.Add(answerLabel);
            var correctionButton = new Button("I was right")
            {
                X = 1,
                Y = 3,
                Width = 11,
                Height = 1,
                Clicked = () => { CurrentScore += 1; },
                CanFocus = false,
                Visible = false
            };
            meaningFrame.Add(correctionButton);
            meaningFrame.Add(new Button("Check answer")
            {
                X = 1,
                Y = 2,
                Width = 12,
                Height = 1,

                Clicked = () =>
                {
                    answerBox.Disabled = true;
                    var answers = AnswerHelpers.GenerateAnswers(lemma.LemmaShortDef, lemma.Definitions);
                    if (!toLatin && AnswerHelpers.CheckEnglishAnswer(answerBox.Text.ToString(), answers)
                        || toLatin && TextNormaliser.Fix(answerBox.Text.ToString().Trim()) == TextNormaliser.Fix(lemma.LemmaText))
                    {
                        answerLabel.Text = "✓";
                        CurrentScore += 1;
                    }
                    else
                    {
                        var correctAnswers = toLatin ? TextNormaliser.Fix(lemma.LemmaText) : string.Join("; ", answers);
                        correctionButton.CanFocus = true;
                        correctionButton.Visible = true;
                        correctionButton.SetNeedsDisplay();
                        var text = $"X. \"{correctAnswers}\" were the correct answers";
                        answerLabel.Text = LearningHelpers.SplitTextIntoLines(text, meaningFrame.Frame.Width - 20);
                    }
                }
            });
            return meaningFrame;
        }

        private FrameView GenerateFormQuestion(bool toLatin, int yPos, Form form, bool treatedAdjAsParticiple, out int bottomY)
        {
            string promptText;
            var correctAnswers = new List<string>();

            var includeGender = CurrentWord.LemmaData?.PartOfSpeech != null && CurrentWord.LemmaData.PartOfSpeech.PartName != "Noun";
            if (toLatin)
            {
                // display 'morphcode', require latin
                promptText = string.Join(",", MorphHelp.GenerateDescFromMorphCode(form.MorphCode, treatedAdjAsParticiple, includeGender));

                var equivalentForms = _context.Forms
                    .Where(f => f.LemmaId == form.LemmaId &&
                                string.Join(",", MorphHelp.GenerateDescFromMorphCode(f.MorphCode, treatedAdjAsParticiple, includeGender)) ==
                                promptText)
                    .ToList();

                correctAnswers.Add(TextNormaliser.Fix(form.Text));
                correctAnswers.AddRange(equivalentForms.Select(f => TextNormaliser.Fix(f.Text)));
            }
            else
            {
                // display latin, require description of it
                var morphCodeDescription = treatedAdjAsParticiple
                    ? "Case, Number"
                    : string.Join(", ", MorphHelp.DescribeMorphCode(form.MorphCode, includeGender));
                promptText = TextNormaliser.Fix(form.Text) + ": " + morphCodeDescription.ToLower();

                var possibleForms = _context.Forms.Where(f => f.LemmaId == form.LemmaId && f.Text == form.Text);
                correctAnswers.AddRange(possibleForms.Select(f => f.MorphCode));
                correctAnswers = correctAnswers.Select(ca => ca.Substring(1)).ToList();
                if (!includeGender)
                    correctAnswers = correctAnswers.Select(ca =>
                        ca.Remove(5, 1).Insert(5, "-")).ToList();
            }

            correctAnswers = correctAnswers.Select(c => c.ToLower()).ToList();

            bottomY = yPos + 6;
            var formFrame = new FrameView("Forms")
            {
                X = 1,
                Y = yPos,
                Width = Dim.Fill(),
                Height = 6
            };
            formFrame.Add(new Label(1, 0, promptText));
            var answerBox = new TextField("")
            {
                X = 1,
                Y = 1,
                Width = 40
            };
            formFrame.Add(answerBox);
            var answerLabel = new Label("")
            {
                X = 18,
                Y = 2,
                Width = Dim.Fill(),
                Height = 2
            };
            formFrame.Add(answerLabel);

            var correctionButton = new Button("I was right")
            {
                X = 1,
                Y = 3,
                Width = 11,
                Height = 1,
                Clicked = () => { CurrentScore += 1; },
                CanFocus = false,
                Visible = false
            };

            formFrame.Add(new Button("Check answer")
            {
                X = 1,
                Y = 2,
                Width = 12,
                Height = 1,

                Clicked = () =>
                {
                    answerBox.Disabled = true;
                    var answer = answerBox.Text.ToString().Trim();

                    answer = toLatin ? TextNormaliser.Fix(answer) : MorphHelp.GenerateMorphCodeFromDesc(answer).Substring(1);

                    var correct = correctAnswers.Contains(answer);
                    if (!toLatin && !correct) correct = correctAnswers.Contains($"-{answer[1]}r-p{answer.Substring(5)}");

                    if (correct)
                    {
                        answerLabel.Text = "✓";
                        CurrentScore += 1;
                    }
                    else
                    {
                        correctionButton.CanFocus = true;
                        correctionButton.Visible = true;
                        correctionButton.SetNeedsDisplay();

                        var displayAnswers = toLatin
                            ? correctAnswers
                            : correctAnswers
                                .Select(a =>
                                    string.Join(", ", MorphHelp.GenerateDescFromMorphCode("-" + a, treatedAdjAsParticiple, includeGender)))
                                .ToList();

                        var text = displayAnswers.Count > 1
                            ? $"X. {string.Join("; ", displayAnswers)} were the correct answers."
                            : $"X. {displayAnswers[0]} was the correct answer.";

                        answerLabel.Text = LearningHelpers.SplitTextIntoLines(text, formFrame.Frame.Width - 20);
                    }
                }
            });
            formFrame.Add(correctionButton);

            return formFrame;
        }

        private FrameView GenerateMultiChoiceQuestion(IEnumerable<string> options, int correctAnswerIndex, string frameName, int yCoord,
            out int bottomY)
        {
            var optionsArr = options as string[] ?? options.ToArray();
            var frame = new FrameView(frameName) // wordPart.PartName == "Verb" ? "Conjugation" : "Declension" + " test"
            {
                X = 1,
                Y = yCoord,
                Width = Dim.Fill(),
                Height = optionsArr.Length + 3
            };
            var categoryRadio = new RadioGroup(1, 0, optionsArr.ToArray());
            /* */
            frame.Add(categoryRadio);
            var answerLabel = new Label("")
            {
                Y = optionsArr.Length,
                X = 11,
                Width = 13,
                Height = 1
            };
            frame.Add(answerLabel);
            frame.Add(new Button("Check")
            {
                X = 1,
                Y = optionsArr.Length,
                Width = 7,
                Height = 1,

                Clicked = () =>
                {
                    // check value

                    categoryRadio.Disabled = true;

                    if (categoryRadio.Selected == correctAnswerIndex)
                    {
                        // correct
                        answerLabel.Text = "✓";
                        CurrentScore += 1;
                    }
                    else
                    {
                        //incorrect
                        answerLabel.Text = $"X. {optionsArr[correctAnswerIndex]} was the correct answer.";
                    }
                }
            });
            bottomY = yCoord + optionsArr.Length + 3;
            return frame;
        }


        public Window DisplayUI(bool toLatin, bool ignoreUnknown)
        {
            Words = FileHelpers.GenerateData(Lists, _context, ignoreUnknown);
            var window = new Window("Learning: " + (toLatin ? "English to Latin" : "Latin to English"))
            {
                X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() - 1
            };

            var closeButton = new Button("X")
            {
                Clicked = Application.RequestStop,
                X = Pos.Right(window) - 8,
                Y = 0,
                Width = 5,
                Height = 1
            };

            void OnFinishLearning(bool funcToLatin)
            {
                window.RemoveAll();

                CurrentWord = LearningHelpers.SelectWord(Words);
                CurrentScore = 0;
                _scoreLabel = new Label($"score: {CurrentWord.UserLearntWord.RevisionStage} (+ {CurrentScore})")
                {
                    X = Pos.Right(window) - 30,
                    Y = 0,
                    Width = 5,
                    Height = 1
                };
                window.Add(_scoreLabel);
                CarryOutTest(window, funcToLatin, OnFinishLearning);
                window.Add(closeButton);
                Application.TerminalResized();
                window.FocusFirst();
            }

            OnFinishLearning(toLatin);


            return window;
        }
    }
}