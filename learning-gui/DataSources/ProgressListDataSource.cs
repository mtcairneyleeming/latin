using System;
using System.Collections.Generic;
using System.Linq;
using NStack;
using Terminal.Gui;

namespace learning_gui.DataSources
{
    public class ProgressListItem
    {
        public string LemmaName { get; set; }
        public string Definition { get; set; }
        public int ProgressLevel { get; set; }
    }

    internal class ProgressListDataSource : IListDataSource
    {
        private bool _alphabetically;
        private bool _ascending;

        public ProgressListDataSource()
        {
            Items = new List<ProgressListItem>();
        }

        public List<ProgressListItem> Items { get; private set; }


        public void Render(ListView container, ConsoleDriver driver, bool selected, int item, int col, int line, int width)
        {
            var i = Items[item];
            var text = "";
            text += i.LemmaName.PadRight(20);
            text += "|";
            text += i.Definition is null
                ? new string(' ', width - 30)
                : i.Definition.PadRight(width - 30).Substring(0, width - 30);
            text += "|";
            text += i.ProgressLevel.ToString().PadRight(6);
            RenderUstr(driver, text.Replace("�", " "), width);
        }

        public bool IsMarked(int item)
        {
            return false;
        }

        public void SetMark(int i, bool value)
        {
        }

        public int Count => Items.Count;

        private void RenderUstr(ConsoleDriver driver, ustring ustr, int width)
        {
            var byteLen = ustr.Length;
            var used = 0;
            for (var i = 0; i < byteLen;)
            {
                var (rune, size) = Utf8.DecodeRune(ustr, i, i - byteLen);
                var count = Rune.ColumnWidth(rune);
                if (used + count >= width)
                    break;
                driver.AddRune(rune);
                used += count;
                i += size;
            }

            for (; used < width; used++) driver.AddRune(' ');
        }

        public void SortAscending()
        {
            _ascending = true;
            Items = _alphabetically ? Items.OrderBy(i => i.LemmaName).ToList() : Items.OrderBy(i => i.ProgressLevel).ToList();
        }

        public void SortDescending()
        {
            _ascending = false;
            Items = _alphabetically ? Items.OrderByDescending(i => i.LemmaName).ToList() : Items.OrderByDescending(i => i.ProgressLevel).ToList();
        }

        public void SortAlphabetically()
        {
            _alphabetically = true;
            Items = _ascending ? Items.OrderBy(i => i.LemmaName).ToList() : Items.OrderByDescending(i => i.LemmaName).ToList();
        }

        public void SortByProgress()
        {
            _alphabetically = false;
            Items = _ascending ? Items.OrderBy(i => i.ProgressLevel).ToList() : Items.OrderByDescending(i => i.ProgressLevel).ToList();
        }
    }
}