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
using Nemont.WPF.Service;

namespace Nemont.WPF.Controls
{
    public class DataGrid : System.Windows.Controls.DataGrid
    {
        static DataGrid()
        {
            CommandManager.RegisterClassCommandBinding(typeof(DataGrid),
                new CommandBinding(ApplicationCommands.Paste,
                    new ExecutedRoutedEventHandler(OnExecutedPaste),
                    new CanExecuteRoutedEventHandler(OnCanExecutePaste)));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGrid), new FrameworkPropertyMetadata(typeof(DataGrid)));
        }

        [Category("Rows")]
        public bool CanClipboardPaste { get { return (bool)GetValue(CanClipboardPasteProperty); } set { SetValue(CanClipboardPasteProperty, value); } }
        public static readonly DependencyProperty CanClipboardPasteProperty =
            DependencyProperty.RegisterAttached("CanClipboardPaste", typeof(bool), typeof(DataGrid), new FrameworkPropertyMetadata(false));

        [Category("Columns")]
        public bool AllowMultipleSortColumns { get { return (bool)GetValue(AllowMultipleSortColumnsProperty); } set { SetValue(AllowMultipleSortColumnsProperty, value); } }
        public static readonly DependencyProperty AllowMultipleSortColumnsProperty =
            DependencyProperty.RegisterAttached("AllowMultipleSortColumns", typeof(bool), typeof(DataGrid), new FrameworkPropertyMetadata(true));

        #region Sorting
        private List<int> BeforeSortOrder;
        private List<ListSortDirection> BeforeSortDirection;
        private List<int> AfterSortOrder = new List<int>();
        private List<ListSortDirection> AfterSortDirection = new List<ListSortDirection>();

        protected override void OnSorting(DataGridSortingEventArgs e)
        {
            if (AllowMultipleSortColumns == false) {
                foreach (var c in Columns) {
                    if (c.Equals(e.Column) == false) {
                        c.SortDirection = null;
                    }
                }
                switch (e.Column.SortDirection) {
                case ListSortDirection.Ascending:
                    e.Column.SortDirection = ListSortDirection.Descending;
                    break;
                case ListSortDirection.Descending:
                    e.Column.SortDirection = null;
                    break;
                default:
                    e.Column.SortDirection = ListSortDirection.Ascending;
                    break;
                }
                Items.SortDescriptions.Clear();
                if (e.Column.SortDirection != null) {
                    Items.SortDescriptions.Add(new SortDescription(e.Column.SortMemberPath, (ListSortDirection)e.Column.SortDirection));
                }
            }
            else {
                BeforeSortOrder = new List<int>();
                BeforeSortDirection = new List<ListSortDirection>();
                int routedIndex = Columns.IndexOf(e.Column);
                ListSortDirection? routedDirection;

                for (int ai = 0; ai < AfterSortOrder.Count; ai++) {
                    BeforeSortOrder.Add(AfterSortOrder[ai]);
                    BeforeSortDirection.Add(AfterSortDirection[ai]);
                }

                switch (e.Column.SortDirection) {
                case ListSortDirection.Ascending:
                    routedDirection = ListSortDirection.Descending;
                    break;
                case ListSortDirection.Descending:
                    routedDirection = null;
                    break;
                default:
                    routedDirection = ListSortDirection.Ascending;
                    break;
                }

                AfterSortOrder = new List<int>();
                AfterSortDirection = new List<ListSortDirection>();
                if (routedDirection != null) {
                    AfterSortOrder.Add(routedIndex);
                    AfterSortDirection.Add((ListSortDirection)routedDirection);
                }
                for (int bi = 0; bi < BeforeSortOrder.Count; bi++) {
                    if (BeforeSortOrder[bi] != routedIndex) {
                        AfterSortOrder.Add(BeforeSortOrder[bi]);
                        AfterSortDirection.Add(BeforeSortDirection[bi]);
                    }
                }
                
                // 열 컬렉션 순서 재설정
                for (int c = 0; c < Columns.Count; c++) {
                    int index = AfterSortOrder.FindIndex(item => item == c);

                    if (index != -1) {
                        Columns[c].SortDirection = AfterSortDirection[index];
                    }
                    else {
                        Columns[c].SortDirection = null;
                    }
                }

                // 아이템 정렬 순서 재설정
                Items.SortDescriptions.Clear();
                for (int ai = 0; ai < AfterSortOrder.Count; ai++) {
                    Items.SortDescriptions.Add(new SortDescription(Columns[AfterSortOrder[ai]].SortMemberPath, AfterSortDirection[ai]));
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
            if (CanClipboardPaste == false) {
                return;
            }
            // Parse the clipboard data
            List<string[]> data = ClipboardHelper.ParseClipboardData();

            if (data == null) {
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

            for (dispRow = minDispRow, dataRow = 0; dataRow < data.Count; dispRow++, dataRow++) {
                if (dispRow >= maxDispRow) {
                    var itemsSourceList = ItemsSource as IList;
                    var addValue = Activator.CreateInstance(Items[0].GetType());
                    maxDispRow++;
                    itemsSourceList.Add(addValue);
                }
                BeginEditCommand.Execute(null, this);

                for (dispCol = minDispCol, dataCol = 0; dispCol < maxDispCol && dataCol < data[dataRow].Length; dispCol++, dataCol++) {
                    DataGridColumn column = ColumnFromDisplayIndex(dispCol);
                    column.OnPastingCellClipboardContent(Items[dispRow], data[dataRow][dataCol]);

                    if (SelectionUnit == DataGridSelectionUnit.CellOrRowHeader || SelectionUnit == DataGridSelectionUnit.Cell) {
                        SelectedCells.Add(new DataGridCellInfo(Items[dispRow], Columns[dispCol]));
                    }
                }
                if (SelectionUnit == DataGridSelectionUnit.FullRow) {
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
}
