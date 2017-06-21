

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PickandPlaceV2
{
    public class DataHelpers
    {

        public void SetupTextColumn(DataGrid dg, string header, string binding, bool ReadOnly)
        {
            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Left));

            DataGridTextColumn dgvc = new DataGridTextColumn();
            dgvc.Header = header;
            dgvc.Binding = new Binding(binding);
            dgvc.IsReadOnly = ReadOnly;
            dgvc.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dgvc.CellStyle = s;
            dg.Columns.Add(dgvc);
        }
        public void SetupCheckBoxColumn(DataGrid dg, string header, string binding, bool ReadOnly)
        {
            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Left));
          

            DataGridCheckBoxColumn dgvc = new DataGridCheckBoxColumn();
            dgvc.Header = header;
            dgvc.Binding = new Binding(binding);
            dgvc.IsReadOnly = ReadOnly;
            dgvc.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            dgvc.CellStyle = s;
           
            dg.Columns.Add(dgvc);
        }


        public double Nozzle1Xoffset = -20.0;
        public double Nozzle1Yoffset = 7.2;//7.16

        public double Nozzle2Xoffset = 11.82;
        public double Nozzle2Yoffset = 7.22; //7.22
    }
}
