using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Threading.Tasks;

namespace KöTaf.WPFApplication.Helper
{
    /// <summary>
    /// Extension methods for DataGrid
    /// 
    /// URL: https://code.google.com/p/artur02/source/browse/trunk/DataGridExtensions/DataGridHelper.cs
    /// </summary>
    public static class DataGridHelper
    {
        /// <summary>
        /// Gets the visual child of an element
        /// </summary>
        /// <typeparam name="T">Expected type</typeparam>
        /// <param name="parent">The parent of the expected element</param>
        /// <returns>A visual child</returns>
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        /// <summary>
        /// Gets the specified cell of the DataGrid
        /// </summary>
        /// <param name="grid">The DataGrid instance</param>
        /// <param name="row">The row of the cell</param>
        /// <param name="column">The column index of the cell</param>
        /// <returns>A cell of the DataGrid</returns>
        public static DataGridCell GetCell(this DataGrid grid, DataGridRow row, int column)
        {
            if (row != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);

                if (presenter == null)
                {
                    grid.ScrollIntoView(row, grid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(row);
                }

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);

                return cell;
            }
            return null;
        }

        /// <summary>
        /// Use this if a DataTemplate within a cell is used
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static ContentPresenter GetContentPresenter(this DataGrid grid, int row, int column)
        {
            var currentRow = grid.GetRow(row);
            if (currentRow != null)
                return grid.Columns[column].GetCellContent(currentRow) as ContentPresenter;
            return null;
        }

        /// <summary>
        /// Gets the specified cell of the DataGrid
        /// </summary>
        /// <param name="grid">The DataGrid instance</param>
        /// <param name="row">The row index of the cell</param>
        /// <param name="column">The column index of the cell</param>
        /// <returns>A cell of the DataGrid</returns>
        public static DataGridCell GetCell(this DataGrid grid, int row, int column)
        {
            DataGridRow rowContainer = grid.GetRow(row);
            return grid.GetCell(rowContainer, column);
        }

        /// <summary>
        /// Gets the specified row of the DataGrid
        /// </summary>
        /// <param name="grid">The DataGrid instance</param>
        /// <param name="index">The index of the row</param>
        /// <returns>A row of the DataGrid</returns>
        public static DataGridRow GetRow(this DataGrid grid, int index)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        /// <summary>
        /// Gets the selected row of the DataGrid
        /// </summary>
        /// <param name="grid">The DataGrid instance</param>
        /// <returns></returns>
        public static DataGridRow GetSelectedRow(this DataGrid grid)
        {
            return (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem);
        }

        /// <summary>
        /// Try to get the cell value as string. If the cell contains a datatemplate, this method fails. 
        /// If you want to get the element (e.g. ComboBox) in the datatemplate try this:
        ///     ContentPresenter cp = myDataGrid.Columns[column].GetCellContent(row) as ContentPresenter;
        //      var combobox = DataGridHelper.GetVisualChild<ComboBox>(cp);
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static string TryGetCellValueAsString(this DataGridCell cell)
        {
            if (cell.Content is TextBlock)
                return ((TextBlock)cell.Content).Text;

            if (cell.Content is TextBox)
                return ((TextBox)cell.Content).Text;

            if (cell.Content is CheckBox)
                return ((CheckBox)cell.Content).Content.ToString();

            return null;
        }

        public static IList<KeyValuePair<string, string>> SelectedItemToKeyValueList(this DataGrid grid)
        {
            if (grid.SelectedItem != null)
            {
                IList<KeyValuePair<string, string>> keyValueList = new List<KeyValuePair<string, string>>();

                int rowIndex = grid.SelectedIndex;
                for (int j = 0; j < grid.Columns.Count; j++)
                {
                    var cell = grid.GetCell(rowIndex, j);
                    var key = cell.Column.Header.ToString();
                    var value = cell.TryGetCellValueAsString();
                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        keyValueList.Add(new KeyValuePair<string, string>(key, value));
                    }
                }

                return keyValueList;
            }
            return null;
        }

        public static IList<KeyValuePair<string, string>> ToKeyValueList(this DataGrid grid)
        {
            IList<KeyValuePair<string, string>> keyValueList = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < grid.Items.Count; i++)
            {
                for (int j = 0; j < grid.Columns.Count; j++)
                {
                    var cell = grid.GetCell(i, j);
                    if (cell.Column.Header != null)
                    {
                        var key = cell.Column.Header.ToString();
                        var value = cell.TryGetCellValueAsString();
                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                        {
                            var kvp = new KeyValuePair<string, string>(key, value);
                            keyValueList.Add(kvp);
                            //  keyValueList.Add(new string[][] { new string[] { key, value } });
                        }
                    }
                   
                }
            }

            return keyValueList;
        }
    }
}