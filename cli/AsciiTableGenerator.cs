using System.Collections.Generic;
using System.Linq;
using System.Text;

// https://github.com/tarikguney/ascii-table-creator/blob/master/AsciiTableGenerator.cs

namespace cli
{
    public class AsciiTableGenerator
    {
        private readonly string[] _columnNames;
        private readonly string[][] _data;

        public AsciiTableGenerator(string[] columnNames, string[][] data)
        {
            _columnNames = columnNames;
            _data = data;
        }

        public string Render()
        {
            var maxWidths = _columnNames.Select(row => row.Length).ToList();
            for (var colNum = 0; colNum < _columnNames.Length; colNum++)
            {
                foreach (var t in _data)
                {
                    if (t[colNum].Length > maxWidths[colNum]) maxWidths[colNum] = t[colNum].Length;
                }
            }

            var builder = new StringBuilder();
            // top border
            RenderRowSeparator(builder, maxWidths);
            // header row
            builder.Append("|");
            for (var i = 0; i < _columnNames.Length; i++)
            {
                var name = _columnNames[i];
                builder.Append(name.Pad(maxWidths[i]));
                builder.Append("|");
            }

            builder.AppendLine();
            RenderRowSeparator(builder, maxWidths);

            foreach (var row in _data)
            {
                builder.Append("|");
                for (var i = 0; i < row.Length; i++)
                {
                    var name = row[i];
                    if (name == "")
                    {
                        builder.Append(' ', maxWidths[i] + 1);
                    }
                    else
                    {
                        builder.Append(name.Pad(maxWidths[i]));
                        builder.Append("|");
                    }
                }

                builder.AppendLine();
            }

            // Bottom border
            RenderRowSeparator(builder, maxWidths);
            return builder.ToString();
        }

        private static void RenderRowSeparator(StringBuilder builder, IEnumerable<int> maxWidths)
        {
            builder.Append("+");
            foreach (var width in maxWidths)
            {
                builder.Append('-', width);
                builder.Append("+");
            }

            builder.AppendLine();
        }
    }
}