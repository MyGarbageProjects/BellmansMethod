using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BellmansMethod.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        
        int[][] InputTable;
        int[][] X;
        int[][] ResultTable;//
        void SetLOG(string s)
        {
            txtLOG.Text += s+"\r\n";
        }
        void UpdateMatrix(int[][] table)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Xi", typeof(string)));
            for (int i = 0; i < table[0].Length - 1; i++)
                dt.Columns.Add(new DataColumn("f" + i, typeof(string)));
            for (int i = 0; i < table.Length; i++)
            {
                DataRow r = dt.NewRow();
                for (int c = 0; c < table[0].Length; c++)
                    r[c] = table[i][c];
                dt.Rows.Add(r);
            }
            DataViewer.ItemsSource = dt.DefaultView;
        }
        void UpdateMatrix(int[][] table, ref DataGrid DataViewer)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("B", typeof(string)));
            for (int i = 0; i < table[0].Length - 1; i++)
                dt.Columns.Add(new DataColumn(String.Format("Z{0}", i)));

            //---
            DataRow r = dt.NewRow();
            for (int i = 1; i < table[0].Length; i++)
                r[i] = String.Format(" f{0} ", i);
            dt.Rows.Add(r);
            //---
            for (int i = 0; i < table.Length; i++)
            {
                r = dt.NewRow();
                for (int c = 0; c < table[0].Length; c++)
                    if (c == 0)
                    {
                        r[c] = i;
                    }
                    else
                    {
                        r[c] = String.Format("  {0} ", table[i][c]);
                    }
                dt.Rows.Add(r);
            }


            DataViewer.ItemsSource = dt.DefaultView;
            //DataTable dt = new DataTable();
            //dt.Columns.Add(new DataColumn("B", typeof(string)));
            //for (int i = 0; i < table[0].Length - 1; i++)
            //    dt.Columns.Add(new DataColumn(String.Format("Z{0}\\f{0}",i)));
            //for (int i = 0; i < table.Length; i++)
            //{
            //    DataRow r = dt.NewRow();
            //    for (int c = 0; c < table[0].Length; c++)
            //        if (c == 0)
            //        {
            //            r[c] = i;
            //        }
            //        else
            //        {
            //            r[c] = table[i][c];
            //        }
            //    dt.Rows.Add(r);
            //}
            //DataViewer.ItemsSource = dt.DefaultView;
        }
        int max(int val1, int val2,out int index)
        {
            index = val1 > val2 ? 0 : 1;
            return val1 > val2 ? val1 : val2;
        }
        int fmax(int fi, int Xi)
        {
            int _max = int.MinValue;
            int _i = 0;
            for (int XofF = 0, XofLastF = Xi; XofLastF >= 0; XofF++, XofLastF--)
            {
                //InputTable[XofF][fi] + ResultTable[XofLastF, fi+1];
                int _temp = InputTable[XofF][fi] + ResultTable[XofLastF][fi + 1];
                SetLOG(String.Format("f{0}({1}) + f({2}-{1}) = {3}", fi, XofF, Xi,_temp));
                txtLOG.ScrollToEnd();
                //SetLOG($"f{fi}({XofF}) + f({Xi}-{XofF}) = {_temp}");
                if (_temp >= _max)
                {
                    _max = _temp;
                    //X[fi][fi] = XofF;
                }
            }
            //Console.WriteLine();
            //X[fi][_i] = _i;
            return _max;
        }
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        private void btnLoadTable_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if(ofd.ShowDialog() == true)
            {
                string[] _temp = System.IO.File.ReadAllLines(ofd.FileName);
                InputTable = new int[_temp.Length][];
                for (int i = 0; i < _temp.Length; i++)
                    InputTable[i] = _temp[i].Split(' ').Select(Int32.Parse).ToArray<int>();
                UpdateMatrix(InputTable);
            }
            //------------------
            txtN.Text = InputTable.Length.ToString();
            txtA.Text = (InputTable.Length - 1).ToString();
            txtgx.Text = (InputTable.Length - 1).ToString();
            btnStart.Visibility = Visibility.Visible;
            //-------------
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            ResultTable = new int[InputTable.Length][];
            X = new int[InputTable.Length][];
            for (int i = 0; i < InputTable.Length; i++)
            {
                ResultTable[i] = new int[InputTable[i].Length];
                X[i] = new int[InputTable[i].Length];
            }
            //X = new int[InputTable.Length][InputTable[0].Length];


            ///------------------------------------------------------------------------------------
            ResultTable[0][InputTable.Length] = InputTable[0][InputTable.Length];
            for (int Xi = 1; Xi < InputTable.Length; Xi++)
            {
                int _i = 0;
                SetLOG(String.Format("f{0}({1}) + f({2}-{1}) = {3}", 4, Xi, Xi, Xi));
                ResultTable[Xi][InputTable.Length] = max(InputTable[Xi][InputTable.Length], InputTable[Xi-1][InputTable.Length],out _i);
                if (_i == 0)
                {
                    X[Xi][InputTable.Length] = Xi;
                }
                else
                {
                    X[Xi][InputTable.Length] = Xi-1;
                }
            }

            for (int fi = InputTable.Length-1; fi > 0 ; fi--)
            {
                for (int Xi = 0; Xi < InputTable.Length; Xi++)
                {
                    ResultTable[Xi][fi] = fmax(fi, Xi);
                }
            }
            lblResult.Content = "f*= " + ResultTable[ResultTable.Length-1][1];
            UpdateMatrix(ResultTable, ref DataViewerResult);
        }

    }
}
