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
        public string one, two, cash, admission, dsa,bs;


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
            one = "";
            two = "";
            cash = "";
            admission = "";
            dsa = "";
            bs = "";
        }


        public void SetNowServing(object sender, EventArgs e) {


            // ----------------Now Serving List Box -----------------
            if (!(connectionSQL.retrieveCurrentQueueNumWin1() == " ")) {
                window1_text.Text = connectionSQL.retrieveCurrentQueueNumWin1();
                one = connectionSQL.retrieveCurrentQueueNumWin1();
            }
            if (!(connectionSQL.retrieveCurrentQueueNumWin2() == " ")) {
                window2_text.Text = connectionSQL.retrieveCurrentQueueNumWin2();
                two = connectionSQL.retrieveCurrentQueueNumWin2();
            }
            if (!(connectionSQL.retrieveCurrentQueueNumCashier() == " ")) {
                Cashier_text.Text = connectionSQL.retrieveCurrentQueueNumCashier();
                cash = connectionSQL.retrieveCurrentQueueNumCashier();
                Console.WriteLine(connectionSQL.retrieveCurrentQueueNumCashier());
            }
            if (!(connectionSQL.retrieveCurrentQueueNumAdmission() == " ")) {
                admission_text.Text = connectionSQL.retrieveCurrentQueueNumAdmission();
                admission = connectionSQL.retrieveCurrentQueueNumAdmission();
            }
            if (!(connectionSQL.retrieveCurrentQueueNumDSA() == " ")) {
                DSA_Office_text.Text = connectionSQL.retrieveCurrentQueueNumDSA();
                dsa = connectionSQL.retrieveCurrentQueueNumDSA();
                
            }
            if (!(connectionSQL.retrieveCurrentQueueNumBookshop() == " ")) {
                bookshop_text.Text = connectionSQL.retrieveCurrentQueueNumBookshop();
                bs = connectionSQL.retrieveCurrentQueueNumBookshop();

            }
            //-------------------------------------------------------
            Console.WriteLine(connectionSQL.retrieveUpcomingTenQueue(one, two, cash, admission, dsa,bs));
            upcomingListBx.DataSource = connectionSQL.retrieveUpcomingTenQueue(one, two, cash, admission, dsa,bs);
        }


        private void button1_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        //----------------------------------------------------
    }
}
