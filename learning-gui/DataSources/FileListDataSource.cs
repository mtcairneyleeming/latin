using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JsonFlatFileDataStore;
using NStack;
using Terminal.Gui;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace learning_gui.DataSources
{
    public class ListItem
    {
        public string FileName { get; set; }
        public string ListName { get; set; }
        public bool Marked { get; set; }

        #region Equality stuff

        private bool Equals(ListItem other)
        {
            return string.Equals(FileName, other.FileName) && string.Equals(ListName, other.ListName) && Marked == other.Marked;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ListItem)) return false;
            return Equals((ListItem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FileName != null ? FileName.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (ListName != null ? ListName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Marked.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ListItem left, ListItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ListItem left, ListItem right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

    internal class FileListDataSource : IListDataSource
    {
        public FileListDataSource(string cacheLocation)
        {
            Items = new List<ListItem>();
            Store = new DataStore(cacheLocation);
            Items = Store.GetCollection<ListItem>().AsQueryable().ToList();
        }

        public List<ListItem> Items { get; }
        private DataStore Store { get; }


        public void Render(ListView container, ConsoleDriver driver, bool selected, int item, int col, int line, int width)
        {
            var text = "";
            text += IsMarked(item) ? "[x] " : "[ ] "; // 4 chars
            text += Items[item].ListName;
            text += " : ";
            text += Items[item].FileName;

            RenderUstr(driver, text, width);
        }

        public bool IsMarked(int item)
        {
            return Items[item].Marked;
        }

        public void SetMark(int i, bool value)
        {
            var item = Items[i];
            item.Marked = value;
            Items[i] = item;
            Store.GetCollection<ListItem>().UpdateOne(li => li.FileName == item.FileName, item);
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

        public void AddItem(string file)
        {
            var newItem = new ListItem {FileName = file, ListName = File.ReadLines(file).First(), Marked = true};
            Items.Add(newItem);
            Store.GetCollection<ListItem>().InsertOne(newItem);
        }

        public void RemoveItem(int index)
        {
            var item = Items[index];
            if (item is null) return;

            Items.Remove(item);
            Store.GetCollection<ListItem>().DeleteMany(li => li == item);
        }
    }
}