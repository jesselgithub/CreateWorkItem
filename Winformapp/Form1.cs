using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winformapp
{
    public partial class Form1 : Form
    {
        private string m_TfsHandler;
        delegate void InitTfsHandler();
        private void InitializeTfsHandler()
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.InvokeRequired)
            {
                MessageBox.Show($@"InitializeTfsHandler, InvokeRequired, Thread: {Thread.CurrentThread.Name}");
                var d = new InitTfsHandler(InitializeTfsHandler);
                this.Invoke(d, new object[] { });
            }
            else
            {
                MessageBox.Show($@"InitializeTfsHandler, InvokeNotRequired, Thread: {Thread.CurrentThread.Name}");
                m_TfsHandler = m_TfsHandler ?? "Hello Hello Hello";
            }
        }

        public Form1()
        {
            Thread.CurrentThread.Name = "Main Thread";
            InitializeComponent();
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            MessageBox.Show($@"DoWork, Thread: {Thread.CurrentThread.Name}");
            Thread.Sleep(5000);
            InitializeTfsHandler();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show($@"RunWorkerCompleted, Thread: {Thread.CurrentThread.Name}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show($@"Click, Thread: {Thread.CurrentThread.Name}");
            backgroundWorker1.RunWorkerAsync();
        }
    }
}
