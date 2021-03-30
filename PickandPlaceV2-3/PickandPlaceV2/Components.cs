using System.Data;
using System.IO;
using System.Windows.Forms;


namespace PickandPlaceV2
{
    public class Components
    {
        private DataSet dscomponents = new DataSet();

        public DataSet POPComponentsTable()
        {
            dscomponents.EnforceConstraints = false;
            FileStream finschema = new FileStream(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/datafiles/components.xsd", FileMode.Open, FileAccess.Read, FileShare.Read);
            dscomponents.ReadXmlSchema(finschema);
            finschema.Close();
            FileStream findata = new FileStream(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/datafiles/components.xml", FileMode.Open,
                                 FileAccess.Read, FileShare.ReadWrite);
            dscomponents.ReadXml(findata);
            findata.Close();
            return dscomponents;
        }

        public void SaveDataSet(DataSet ds)
        {
            using (System.IO.StreamWriter xmlSW = new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/datafiles/components.xml"))
            {
                ds.WriteXml(xmlSW, XmlWriteMode.WriteSchema);
                xmlSW.Close();
            }
        }

        public void CheckComponentTable()
        {
            if (dscomponents.Tables.Count == 0)
            {
                POPComponentsTable();
            }
        }
        public string GetComponentValue(string fid)
        {
            CheckComponentTable();
            string returnval = "";

            using (DataView dv = new DataView(dscomponents.Tables[0], "ComponentCode = " + fid, "ComponentCode", DataViewRowState.CurrentRows))
            {
                if (dv.Count > 0)
                {
                    returnval = dv[0]["ComponentValue"].ToString();
                }
            }
            return returnval;
        }
        public string GetComponentPackage(string fid)
        {
            CheckComponentTable();
            string returnval = "";
            using (DataView dv = new DataView(dscomponents.Tables[0], "ComponentCode = " + fid, "ComponentCode", DataViewRowState.CurrentRows))
            {

                if (dv.Count > 0)
                {
                    returnval = dv[0]["Package"].ToString();
                }
            }
            return returnval;
        }
        public double GetPlacementHeight(string fid)
        {
            CheckComponentTable();
            double returnval = 0.0;
            using (DataView dv = new DataView(dscomponents.Tables[0], "ComponentCode = " + fid, "ComponentCode", DataViewRowState.CurrentRows))
            {

                if (dv.Count > 0)
                {
                    returnval = double.Parse(dv[0]["PlacementHeight"].ToString());
                }
            }
            return returnval;
        }
        public double GetFeederHeight(string fid)
        {
            CheckComponentTable();
            double returnval = 0.0;
            using (DataView dv = new DataView(dscomponents.Tables[0], "ComponentCode = " + fid, "ComponentCode", DataViewRowState.CurrentRows))
            {
                
                if (dv.Count > 0)
                {
                    returnval = double.Parse(dv[0]["FeederHeight"].ToString());
                }
            }
            return returnval;
        }

        public double GetPlaceSpeed(string fid, double feedrate)
        {
            CheckComponentTable();
            double returnval = feedrate;
            using (DataView dv = new DataView(dscomponents.Tables[0], "ComponentCode = " + fid, "ComponentCode", DataViewRowState.CurrentRows))
            {
                if (dv.Count > 0)
                {
                    returnval = double.Parse(dv[0]["PlaceSpeed"].ToString());
                }
            }
            return returnval;
        }


        public double GetFeederX(string fid)
        {
            CheckComponentTable();
            double returnval = 0.0;

            using (DataView dv = new DataView(dscomponents.Tables[0], "ComponentCode = " + fid, "ComponentCode", DataViewRowState.CurrentRows))
            {

                if (dv.Count > 0)
                {
                    returnval = double.Parse(dv[0]["FeederX"].ToString());
                }
            }
            return returnval;
        }

        public double GetFeederY(string fid)
        {
            CheckComponentTable();
            double returnval = 0.0;
            using (DataView dv = new DataView(dscomponents.Tables[0], "ComponentCode = " + fid, "ComponentCode", DataViewRowState.CurrentRows))
            {

                if (dv.Count > 0)
                {
                    returnval = double.Parse(dv[0]["FeederY"].ToString());
                }
            }
            return returnval;
        }

        public int GetPickerNozzle(string fid)
        {
            CheckComponentTable();
            int returnval = 0;
            using (DataView dv = new DataView(dscomponents.Tables[0], "ComponentCode = " + fid, "ComponentCode", DataViewRowState.CurrentRows))
            {

                if (dv.Count > 0)
                {
                    returnval = int.Parse(dv[0]["PickerNozzle"].ToString());
                }
            }
            return returnval;
        }

        public int GetFeederID(string fid)
        {
            CheckComponentTable();
            int returnval = 0;

            using (DataView dv = new DataView(dscomponents.Tables[0], "ComponentCode = " + fid, "ComponentCode", DataViewRowState.CurrentRows))
            {

                if (dv.Count > 0)
                {
                    returnval = int.Parse(dv[0]["FeederID"].ToString());
                }
            }
            return returnval;
        }
        public bool GetComponentVerifywithCamera(string fid)
        {
            CheckComponentTable();
            bool returnval = false;
            using (DataView dv = new DataView(dscomponents.Tables[0], "ComponentCode = " + fid, "ComponentCode", DataViewRowState.CurrentRows))
            {

                if (dv.Count > 0)
                {
                    if (dv[0]["VerifywithCamera"].ToString().Equals("True"))
                    {
                        returnval = true;
                    }
                    else
                    {
                        returnval = false;
                    }
                }
            }
            return returnval;
        }

        public bool GetComponentTapeFeeder(string fid)
        {
            CheckComponentTable();
            bool returnval = false;
            using (DataView dv = new DataView(dscomponents.Tables[0], "ComponentCode = " + fid, "ComponentCode", DataViewRowState.CurrentRows))
            {
                if (dv.Count > 0)
                {
                    if (dv[0]["TapeFeeder"].ToString().Equals("True"))
                    {
                        returnval = true;
                    }
                    else
                    {
                        returnval = false;
                    }
                }
            }
            return returnval;
        }
    }
}
