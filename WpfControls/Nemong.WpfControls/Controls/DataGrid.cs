using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfControls.Controls
{
    public class DataGrid : System.Windows.Controls.DataGrid
    {
        static DataGrid()
        {
            CommandManager.RegisterClassCommandBinding(
                typeof(DataGrid),
                new CommandBinding(ApplicationCommands.Paste,
                    new ExecutedRoutedEventHandler(OnExecutedPaste),
                    new CanExecuteRoutedEventHandler(OnCanExecutePaste)));
        }

        [Category("Rows")]
        public bool CanClipboardPaste { get { return (bool)GetValue(CanClipboardPasteProperty); } set { SetValue(CanClipboardPasteProperty, value); } }
        public static readonly DependencyProperty CanClipboardPasteProperty =
            DependencyProperty.RegisterAttached("CanClipboardPaste", typeof(bool), typeof(DataGrid), new FrameworkPropertyMetadata(false));

        [Category("Columns")]
        public bool CanMultipleSortColumns { get { return (bool)GetValue(CanMultipleSortColumnsProperty); } set { SetValue(CanMultipleSortColumnsProperty, value); } }
        public static readonly DependencyProperty CanMultipleSortColumnsProperty =
            DependencyProperty.RegisterAttached("CanMultipleSortColumns", typeof(bool), typeof(DataGrid), new FrameworkPropertyMetadata(true));

        #region OnSorting
        protected override void OnSorting(DataGridSortingEventArgs e)
        {
            if (CanMultipleSortColumns == false)
            {
                foreach (var c in Columns)
                {
                    if (c.Equals(e.Column) == false) c.SortDirection = null;
                }
                switch (e.Column.SortDirection)
                {
                    case ListSortDirection.Ascending: e.Column.SortDirection = ListSortDirection.Descending; break;
                    case ListSortDirection.Descending: e.Column.SortDirection = null; break;
                    default: e.Column.SortDirection = ListSortDirection.Ascending; break;
                }
                Items.SortDescriptions.Clear();
                if (e.Column.SortDirection != null)
                {
                    Items.SortDescriptions.Add(new SortDescription(e.Column.SortMemberPath, (ListSortDirection)e.Column.SortDirection));
                }
            }
            else
            {
                var befSortDesc = new SortDescriptionCollection();
                var curSortDesc = Items.SortDescriptions;

                foreach (var s in curSortDesc)
                {
                    befSortDesc.Add(s);
                }
                switch (e.Column.SortDirection)
                {
                    case ListSortDirection.Ascending: e.Column.SortDirection = ListSortDirection.Descending; break;
                    case ListSortDirection.Descending: e.Column.SortDirection = null; break;
                    default: e.Column.SortDirection = ListSortDirection.Ascending; break;
                }
                curSortDesc.Clear();
                if (e.Column.SortDirection != null)
                {
                    curSortDesc.Add(new SortDescription(e.Column.SortMemberPath, (ListSortDirection)e.Column.SortDirection));
                }
                foreach (var s in befSortDesc)
                {
                    if (s.PropertyName != e.Column.SortMemberPath)
                    {
                        curSortDesc.Add(s);
                    }
                }
                foreach (var s in curSortDesc)
                {
                    var idx = Columns.ToList().FindIndex(i => i.SortMemberPath == s.PropertyName);
                    Columns[idx].SortDirection = s.Direction;
                }
            }
            e.Handled = true;
        }
        #endregion

        #region Clipboard Paste

        private static void OnCanExecutePaste(object target, CanExecuteRoutedEventArgs args)
        {
            (target as DataGrid).OnCanExecutePaste(args);
        }

        /// <summary>
        /// This virtual method is called when ApplicationCommands.Paste command query its state.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnCanExecutePaste(CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = CurrentCell != null;
            args.Handled = true;
        }

        private static void OnExecutedPaste(object target, ExecutedRoutedEventArgs args)
        {
            (target as DataGrid).OnExecutedPaste(args);
        }

        /// <summary>
        /// This virtual method is called when ApplicationCommands.Paste command is executed.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnExecutedPaste(ExecutedRoutedEventArgs args)
        {
            if (CanClipboardPaste == false)
            {
                return;
            }
            // Parse the clipboard data
            List<string[]> data = ClipboardHelper.ParseClipboardData();

            if (data == null)
            {
                return;
            }

            UnselectAll();
            UnselectAllCells();

            // Call OnPastingCellClipboardContent for each cell
            int minDispRow = Items.IndexOf(CurrentItem);
            int maxDispRow = Items.Count;
            int minDispCol = Columns.IndexOf(CurrentColumn);
            int maxDispCol = Columns.Count;
            int dispRow, dispCol, dataRow, dataCol;

            for (dispRow = minDispRow, dataRow = 0; dataRow < data.Count; dispRow++, dataRow++)
            {
                if (dispRow >= maxDispRow)
                {
                    var itemsSourceList = ItemsSource as IList;
                    var addValue = Activator.CreateInstance(Items[0].GetType());
                    maxDispRow++;
                    itemsSourceList.Add(addValue);
                }
                BeginEditCommand.Execute(null, this);

                for (dispCol = minDispCol, dataCol = 0; dispCol < maxDispCol && dataCol < data[dataRow].Length; dispCol++, dataCol++)
                {
                    DataGridColumn column = ColumnFromDisplayIndex(dispCol);
                    column.OnPastingCellClipboardContent(Items[dispRow], data[dataRow][dataCol]);

                    if (SelectionUnit == DataGridSelectionUnit.CellOrRowHeader || SelectionUnit == DataGridSelectionUnit.Cell)
                    {
                        SelectedCells.Add(new DataGridCellInfo(Items[dispRow], Columns[dispCol]));
                    }
                }
                if (SelectionUnit == DataGridSelectionUnit.FullRow)
                {
                    SelectedItems.Add(Items[dispRow]);
                }
                CommitEditCommand.Execute(this, this);
            }
        }

        /// <summary>
        ///     Whether the end-user can add new rows to the ItemsSource.
        /// </summary>
        public bool CanUserPasteToNewRows
        {
            get { return (bool)GetValue(CanUserPasteToNewRowsProperty); }
            set { SetValue(CanUserPasteToNewRowsProperty, value); }
        }

        /// <summary>
        ///     DependencyProperty for CanUserAddRows.
        /// </summary>
        public static readonly DependencyProperty CanUserPasteToNewRowsProperty =
            DependencyProperty.Register("CanUserPasteToNewRows",
                                        typeof(bool), typeof(DataGrid),
                                        new FrameworkPropertyMetadata(true, null, null));

        #endregion Clipboard Paste
    }

    public static class ClipboardHelper
    {
        public delegate string[] ParseFormat(string value);

        public static List<string[]> ParseClipboardData()
        {
            List<string[]> clipboardData = null;
            object clipboardRawData = null;
            ParseFormat parseFormat = null;

            // get the data and set the parsing method based on the format
            // currently works with CSV and Text DataFormats            
            System.Windows.IDataObject dataObj = Clipboard.GetDataObject();
            if ((clipboardRawData = dataObj.GetData(DataFormats.CommaSeparatedValue)) != null)
            {
                parseFormat = ParseCsvFormat;
            }
            else if ((clipboardRawData = dataObj.GetData(DataFormats.Text)) != null)
            {
                parseFormat = ParseTextFormat;
            }

            if (parseFormat != null)
            {
                string rawDataStr = clipboardRawData as string;

                if (rawDataStr == null && clipboardRawData is MemoryStream)
                {
                    // cannot convert to a string so try a MemoryStream
                    MemoryStream ms = clipboardRawData as MemoryStream;
                    StreamReader sr = new StreamReader(ms);
                    rawDataStr = sr.ReadToEnd();
                }
                Debug.Assert(rawDataStr != null, string.Format("clipboardRawData: {0}, could not be converted to a string or memorystream.", clipboardRawData));

                string[] rows = rawDataStr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (rows != null && rows.Length > 0)
                {
                    clipboardData = new List<string[]>();
                    foreach (string row in rows)
                    {
                        clipboardData.Add(parseFormat(row));
                    }
                }
                else
                {
                    Debug.WriteLine("unable to parse row data.  possibly null or contains zero rows.");
                }
            }

            return clipboardData;
        }

        public static string[] ParseCsvFormat(string value)
        {
            return ParseCsvOrTextFormat(value, true);
        }

        public static string[] ParseTextFormat(string value)
        {
            return ParseCsvOrTextFormat(value, false);
        }

        private static string[] ParseCsvOrTextFormat(string value, bool isCSV)
        {
            List<string> outputList = new List<string>();

            char separator = isCSV ? ',' : '\t';
            int startIndex = 0;
            int endIndex = 0;

            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                if (ch == separator)
                {
                    outputList.Add(value.Substring(startIndex, endIndex - startIndex));

                    startIndex = endIndex + 1;
                    endIndex = startIndex;
                }
                else if (ch == '\"' && isCSV)
                {
                    // skip until the ending quotes
                    i++;
                    if (i >= value.Length)
                    {
                        throw new FormatException(string.Format("value: {0} had a format exception", value));
                    }
                    char tempCh = value[i];
                    while (tempCh != '\"' && i < value.Length)
                        i++;

                    endIndex = i;
                }
                else if (i + 1 == value.Length)
                {
                    // add the last value
                    outputList.Add(value.Substring(startIndex));
                    break;
                }
                else
                {
                    endIndex++;
                }
            }

            return outputList.ToArray();
        }
    }
}
