using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Window_Queue;

namespace ServiceWindowForm {
    /// <summary>
    /// Full-screen queue display: shows the ticket each window is now serving
    /// and the next upcoming tickets, refreshed from the database every second.
    /// </summary>
    public partial class STIQueueingService : Form {

        private MySQLDBConnection connectionSQL;

        // Number label per DB window name, in screen order.
        // (DB "Bookshop" is labelled "Book Shop" on screen; only the number label is set from code.)
        private Dictionary<string, Label> nowServingLabels;

        public STIQueueingService() {
            InitializeComponent();
            nowServingLabels = new Dictionary<string, Label> {
                { "Window 1", window1_text },
                { "Window 2", window2_text },
                { "Cashier", Cashier_text },
                { "Admission", admission_text },
                { "DSA Office", DSA_Office_text },
                { "Bookshop", bookshop_text },
            };
            connectionSQL = new MySQLDBConnection();

            // Refresh the screen every second.
            Timer timer = new Timer();
            timer.Interval = (1 * 1000); // 1 sec.
            timer.Tick += new EventHandler(SetNowServing);
            timer.Start();

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
        }

        /// <summary>
        /// Runs every second from the timer: updates each window's "now serving" number
        /// and the upcoming list. A window with no ongoing ticket keeps its last number.
        /// </summary>
        public void SetNowServing(object sender, EventArgs e) {
            Dictionary<string, string> serving = connectionSQL.retrieveNowServing();
            foreach (KeyValuePair<string, Label> window in nowServingLabels) {
                string ticketNo;
                if (serving.TryGetValue(window.Key, out ticketNo)) {
                    window.Value.Text = ticketNo;
                }
            }

            upcomingListBx.DataSource = connectionSQL.retrieveUpcomingTenQueue();
            upcomingListBx.ClearSelected(); // no blue highlight on the first item
        }

        // Close button
        private void button1_Click(object sender, EventArgs e) {
            Application.Exit();
        }
    }
}
