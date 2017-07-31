using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CreateFeatures.Properties;

namespace CreateFeatures
{
    public partial class Form1 : Form
    {
        private TfsHandler m_TfsHandler;
        private Dictionary<int, string> m_TitlesDictionary = new Dictionary<int, string>();
        readonly Image m_ImageNo = new Bitmap(Properties.Resources.NO, 20, 20);
        readonly Image m_ImageYes = new Bitmap(Properties.Resources.YES, 20, 20);
        readonly Image m_ImageWorking = new Bitmap(Properties.Resources.WORKING, 20, 20);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCell c = row.Cells["Status"] as DataGridViewImageCell;
                c.Value = m_ImageNo;
            }
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                //string s = Clipboard.GetText();
                //string[] lines = s.Split('\n');
                //int row = dataGridView1.CurrentCell.RowIndex;
                //int col = dataGridView1.CurrentCell.ColumnIndex;
                //foreach (string line in lines)
                //{
                //}



                DataObject o = (DataObject) Clipboard.GetDataObject();
                //if (o.GetDataPresent(DataFormats.Text))
                //{
                //    if (dataGridView1.RowCount > 0)
                //        dataGridView1.Rows.Clear();

                //    if (dataGridView1.ColumnCount > 0)
                //        dataGridView1.Columns.Clear();

                //    bool columnsAdded = false;
                //    string[] pastedRows = Regex.Split(o.GetData(DataFormats.Text).ToString().TrimEnd("\r\n".ToCharArray()), "\r\n");
                //    foreach (string pastedRow in pastedRows)
                //    {
                //        string[] pastedRowCells = pastedRow.Split(new char[] { '\t' });

                //        if (!columnsAdded)
                //        {
                //            for (int i = 0; i < pastedRowCells.Length; i++)
                //                dataGridView1.Columns.Add("col" + i, pastedRowCells[i]);

                //            columnsAdded = true;
                //            continue;
                //        }

                //        dataGridView1.Rows.Add();
                //        int myRowIndex = dataGridView1.Rows.Count - 1;

                //        using (DataGridViewRow dataGridView1Row = dataGridView1.Rows[myRowIndex])
                //        {
                //            for (int i = 0; i < pastedRowCells.Length; i++)
                //                dataGridView1Row.Cells[i].Value = pastedRowCells[i];
                //        }
                //    }
                //}
            }
        }

        private void getIdsButton_Click(object sender, EventArgs e)
        {
            var frm = new GetIdsForm(GetListOfIds());
            frm.ShowInTaskbar = false;
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog(this);
            
            List<int> listofIds = frm.GetList();

            PopulateDataGridView(listofIds);
            if (listofIds.Count > 0)
            {
                progressBar1.Visible = true;
                //TODO: Connect to TFS
                m_TfsHandler = m_TfsHandler ?? new TfsHandler(GetLastSavedValue("TfsUri", "https://venus.tfs.siemens.net:443/tfs/tia"));
            }
            progressBar1.Visible = false;
        }

        private List<string> GetListOfIds()
        {
            List<string> ids = new List<string>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                ids.Add(row.Cells[0].Value as string);
            }
            return ids;
        }

        private void PopulateDataGridView(List<int> listofIds)
        {
            dataGridView1.Rows.Clear();
            foreach (int id in listofIds)
            {
                AddRowToDataGridView(id);
            }
        }

        private void AddRowToDataGridView(int id)
        {
            var index = dataGridView1.Rows.Add();
            dataGridView1.Rows[index].Cells["RequestId"].Value = $"{id}";
            dataGridView1.Rows[index].Cells["FeatureAreaPath"].Value = GetLastSavedValue("AreaPath", @"TIA\Development\TIA Portal\Engineering Platform\UIA\Applications");
            dataGridView1.Rows[index].Cells["FeatureIncrementPath"].Value = GetLastSavedValue("IncPath", @"TIA\TIA Portal\V15\V15.0.0.0\Inc 15");
            dataGridView1.Rows[index].Cells["AssignedTo"].Value = GetLastSavedValue("AssignedTo", @"M, Juliya James (CT DD DS AA DF-PD FH ES)");
            dataGridView1.Rows[index].Cells["FeatureParent"].Value = GetLastSavedValue("ParentFeature", string.Empty);
            dataGridView1.Rows[index].Cells["CreateTask"].Value = (GetLastSavedValue("CreateTask", "true") == "true");
            dataGridView1.Rows[index].Cells["FeatureEstimatedEffort"].Value = GetLastSavedValue("Hrs", "24");
            dataGridView1.Rows[index].Cells["Status"].Value = m_ImageNo;
            dataGridView1.Rows[index].Cells["Comment"].Value = null;
            //dataGridView1.Rows.Add()
        }

        private string GetLastSavedValue(string areapath, string defaultValue)
        {
            if (SettingsReader.SettingExists(areapath))
            {
                return SettingsReader.GetSetting(areapath, defaultValue);
            }
            return defaultValue;
        }

        private void createFeaturesButton_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            dataGridView1.ReadOnly = true;
            createFeaturesButton.Enabled = getIdsButton.Enabled = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells["Status"].Value = m_ImageWorking;
                row.Cells["Status"].Value = DoUpdate(row, m_TfsHandler);
            }

            createFeaturesButton.Enabled = getIdsButton.Enabled = true;
            dataGridView1.ReadOnly = false;
            progressBar1.Visible = false;
        }

        private Image DoUpdate(DataGridViewRow row, TfsHandler tfsHandler)
        {
            //TODO: Do TFS Update
            int id;
            if (Int32.TryParse(row.Cells[0].Value as string, out id))
            {
                string areaPath = row.Cells[1].Value as string;
                string incPath = row.Cells[2].Value as string;
                string assignedTo = row.Cells[3].Value as string;
                int parentFeature = GetStringAsInt(row.Cells[4].Value as string);
                bool createTask = (bool) row.Cells[5].Value;
                int originalEstimateEffort = GetStringAsInt(row.Cells[6].Value as string);
                string comment;
                if (tfsHandler.UpdateWorkItems(id, areaPath, incPath, assignedTo, parentFeature, createTask, originalEstimateEffort, out comment))
                {
                    row.Cells[8].Value = comment;
                    return m_ImageYes;
                }
                row.Cells[8].Value = comment;
            }
            return m_ImageNo;
        }

        private int GetStringAsInt(string str)
        {
            int featureId = 0;
            Int32.TryParse(str, out featureId);
            return featureId;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                var stringCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string;
                SettingsReader.SetSetting("AreaPath", stringCell);
            }
            if (e.ColumnIndex == 2)
            {
                var stringCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string;
                SettingsReader.SetSetting("IncPath", stringCell);
            }
            if (e.ColumnIndex == 3)
            {
                var stringCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string;
                SettingsReader.SetSetting("AssignedTo", stringCell);
            }
            if (e.ColumnIndex == 4)
            {
                var stringCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string;
                SettingsReader.SetSetting("ParentFeature", stringCell);
            }
            if (e.ColumnIndex == 5)
            {
                var stringCell = (bool) dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                SettingsReader.SetSetting("CreateTask", stringCell ? "true" : "false");
            }
            if (e.ColumnIndex == 6)
            {
                var stringCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string;
                SettingsReader.SetSetting("Hrs", stringCell);
            }

        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 8)
            {
                var cell = dataGridView1.CurrentCell;
                var cellDisplayRect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                toolTip1.Show(cell.Value as string,
                    dataGridView1,
                    cellDisplayRect.X + cell.Size.Width / 2,
                    cellDisplayRect.Y + cell.Size.Height / 3,
                    5000);
                dataGridView1.ShowCellToolTips = false;
            }
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 1 && e.ColumnIndex <= 4)
            {
                if (e.RowIndex >= 0)
                {
                    string idd = dataGridView1.Rows[e.RowIndex].Cells[0].Value as string;
                    int rqId;
                    if (!Int32.TryParse(idd, out rqId))
                    {
                        return;
                    }
                    if (!m_TitlesDictionary.ContainsKey(rqId))
                    {
                        string title = m_TfsHandler.GetWorkItemTitle(rqId);
                        m_TitlesDictionary.Add(rqId, title);
                    }
                    var cellDisplayRect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    titleToolTip.Show(m_TitlesDictionary[rqId],
                        dataGridView1,
                        //MousePosition.X + 1,
                        //MousePosition.Y + 1,
                        cellDisplayRect.X + cellDisplayRect.Width / 3,
                        cellDisplayRect.Y + cellDisplayRect.Height / 3,
                        5000);
                    dataGridView1.ShowCellToolTips = false;
                }
            }
        }
    }
}
