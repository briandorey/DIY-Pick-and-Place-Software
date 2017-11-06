using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace PickandPlaceV2.Views
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class ComponentsEditor : Page
    {
        DataSet ds = new DataSet();
        Components comp = new Components();
        
        public ComponentsEditor()
        {
            InitializeComponent();
            ds = comp.POPComponentsTable();
            dg_data.DataContext = ds.Tables[0].DefaultView;
        }

        private void bt_save_Click(object sender, RoutedEventArgs e)
        {
            comp.SaveDataSet(ds);
            MessageBox.Show("Saved Changes");
        }
        private void bt_Load_Click(object sender, RoutedEventArgs e)
        {
            ds.Clear();
            ds = comp.POPComponentsTable();
            dg_data.DataContext = ds.Tables[0].DefaultView;
        }

        

        private void dg_data_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {

                // FormulaOneDriver driver = e.Row.DataContext as FormulaOneDriver;

                // driver.Save();

            }
        }
    }
}
