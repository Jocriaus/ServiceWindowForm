using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Window_Queue;

//This Class will show the content of the queue to the screen


namespace ServiceWindowForm {
    public partial class STIQueueingService : Form {

        private MySQLDBConnection connectionSQL;
        public string one, two, cash, admin, dsa;

        // These are the now serving queue
        /*   public string window3QueueingNum;
           public string window2QueueingNum;
           public string window1QueueingNum;

           public string objstring1;
           public string objstring2;
           public string objstring3;
           public string objstring;*/

        public STIQueueingService() {
            InitializeComponent();
            connectionSQL = new MySQLDBConnection();
            //The timer will always call the method SetNowServing which updates the window Listbox
            Timer timer = new Timer();
            timer.Interval = (1 * 1000); // 1 sec.
            timer.Tick += new EventHandler(SetNowServing);
            timer.Start();

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;

        }


        public void SetNowServing(object sender, EventArgs e) {


            // ----------------Now Serving List Box -----------------
            nowServingListBx.Items.Clear();
            if (!(connectionSQL.retrieveCurrentQueueNumWin1() == "")) {
                nowServingListBx.Items.Add(connectionSQL.retrieveCurrentQueueNumWin1());
                one = connectionSQL.retrieveCurrentQueueNumWin1();
            }
            if (!(connectionSQL.retrieveCurrentQueueNumWin2() == "")) {
                nowServingListBx.Items.Add(connectionSQL.retrieveCurrentQueueNumWin2());
                two = connectionSQL.retrieveCurrentQueueNumWin2();
            }
            if (!(connectionSQL.retrieveCurrentQueueNumCashier() == "")) {
                nowServingListBx.Items.Add(connectionSQL.retrieveCurrentQueueNumCashier());
                cash = connectionSQL.retrieveCurrentQueueNumCashier();
            }
            if (!(connectionSQL.retrieveCurrentQueueNumAdmin() == "")) {
                nowServingListBx.Items.Add(connectionSQL.retrieveCurrentQueueNumAdmin());
                admin = connectionSQL.retrieveCurrentQueueNumAdmin();
            }
            if (!(connectionSQL.retrieveCurrentQueueNumDSA() == "")) {
                nowServingListBx.Items.Add(connectionSQL.retrieveCurrentQueueNumDSA());
                dsa = connectionSQL.retrieveCurrentQueueNumDSA();
            }
            //-------------------------------------------------------
            upcomingListBx.DataSource = connectionSQL.retrieveUpcomingTenQueue(one, two, cash, admin, dsa);
        }


        private void button1_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        //----------------------------------------------------
    }
}
