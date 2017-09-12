using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateFeatures
{
    public partial class GetIdsForm : Form
    {
        List<int> m_List;

        public GetIdsForm(List<string> ids)
        {
            m_List = new List<int>();
            InitializeComponent();
            textBox1.Clear();
            textBox1.Text = GetText(ids);
        }

        private string GetText(List<string> ids)
        {
            StringBuilder s = new StringBuilder();
            foreach (string id in ids)
            {
                s.AppendLine(id);
            }
            return s.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        public List<int> GetList()
        {
            foreach (string numberAsString in textBox1.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                int num;
                if (Int32.TryParse(numberAsString, out num))
                {
                    if (!m_List.Contains(num))
                    {
                        m_List.Add(num);
                    }
                }
            }
            return m_List;
        }
    }
}
