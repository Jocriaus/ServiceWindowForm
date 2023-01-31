using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Window_Queue {
    class MySQLDBConnection {


        public MySqlConnection connection;
        public MySQLDBConnection() {
            try {
                string ConnectionString = "server=localhost;uid=root;pwd=mypasswordis01;database=queueing_system";
                connection = new MySqlConnection(ConnectionString);
                connection.Open();
            }
            catch (MySqlException e) {
                MessageBox.Show("Connection Error" + e);
            }
        }


        public string loginUser(string uname, string pword) {
            string loginquery= "select * from user where username='" + uname+ "' and password='" + pword + "'";
            MySqlCommand loginCommand = new MySqlCommand(loginquery, connection);
            MySqlDataReader reader = loginCommand.ExecuteReader();
            if (reader.Read()) {
                reader.Close();
                string message = "Successfully Login";
                return message;
            }
            else {
                reader.Close();
                string message = "No Existing Account";
                return message;
            }
        }

        public string usernameExist(string uname) {
            
            string usernameQuery = "select * from user where username='" + uname + "'";
            MySqlCommand usernameCommand = new MySqlCommand(usernameQuery, connection);
            MySqlDataReader reader = usernameCommand.ExecuteReader();
            if (reader.Read()) {
                reader.Close();
                string message = "Record Already Exists.";
                return message;
            }
            else {
                reader.Close();
                string message = "Record Not Exists.";
                return message;
            }
            
        }

        public void registerUser(string uname, string pword, string win) {
            string registerQuery = "insert into user (UserName, Password, Windows) values (@username,@password,@window)";
            MySqlCommand registerCommand = new MySqlCommand(registerQuery, connection);
            registerCommand.Parameters.AddWithValue("username", uname);
            registerCommand.Parameters.AddWithValue("password", pword);
            registerCommand.Parameters.AddWithValue("window", win);
            registerCommand.ExecuteNonQuery();
        }

        public void updateUser(string uname, string pword, string win) {
            string updateUserQuery = "update user SET Password =  @password, Windows = @window WHERE UserName = @username;";
            MySqlCommand updateUserCommand = new MySqlCommand(updateUserQuery, connection);
            updateUserCommand.Parameters.AddWithValue("password", pword);
            updateUserCommand.Parameters.AddWithValue("window", win);
            updateUserCommand.Parameters.AddWithValue("username", uname);
            updateUserCommand.ExecuteNonQuery();
        }

        public List<string> retrieveAllUser() {
            string retrieveUserQuery = "SELECT * FROM user";
            MySqlCommand retrieveUserCommand = new MySqlCommand(retrieveUserQuery, connection);
            MySqlDataReader reader = retrieveUserCommand.ExecuteReader();
            List<string> items = new List<string>();
            while (reader.Read()) {

                items.Add((string)reader["UserName"]);
            }
            reader.Close();
            return items;

        }

        public void deleteUser(string uname) {
            string deleteUserQuery = "DELETE FROM user WHERE UserName = @username;";
            MySqlCommand deleteUserCommand = new MySqlCommand(deleteUserQuery, connection);
            deleteUserCommand.Parameters.AddWithValue("username", uname);
            deleteUserCommand.ExecuteNonQuery();
        }
        
        public MySqlDataAdapter displayUser() {
            string displayUsersQuery = "SELECT * FROM user";
            MySqlCommand displayUsersCommand = new MySqlCommand(displayUsersQuery, connection);
            MySqlDataAdapter sda = new MySqlDataAdapter(displayUsersCommand);
            return sda;
        }
        public MySqlDataAdapter displayClient() {
            string displayClientQuery = "SELECT * FROM client";
            MySqlCommand displayClientCommand = new MySqlCommand(displayClientQuery, connection);
            MySqlDataAdapter sda = new MySqlDataAdapter(displayClientCommand);
            return sda;
        }

        public string retrieveUserWindow(string uname) {
            string retrieveUserWindowQuery = "SELECT Windows FROM user Where UserName = @username; ";
            MySqlCommand retrieveUserWindowCommand = new MySqlCommand(retrieveUserWindowQuery, connection);
            retrieveUserWindowCommand.Parameters.AddWithValue("username", uname);
            string windowName = Convert.ToString(retrieveUserWindowCommand.ExecuteScalar());
            return windowName;
        }
        
        public void OnlineUser(string uname, string pword) {
            string onlineUserQuery = "update user SET OnlineStatus = 'online' WHERE UserName = @username AND Password =  @password;";
            MySqlCommand onlineUserCommand = new MySqlCommand(onlineUserQuery, connection);
            onlineUserCommand.Parameters.AddWithValue("username", uname);
            onlineUserCommand.Parameters.AddWithValue("password", pword);
            onlineUserCommand.ExecuteNonQuery();
        }


        public void OfflineUser(string uname, string pword) {
            string offlineUserQuery = "update user SET OnlineStatus = 'offline' WHERE UserName = @username AND Password =  @password;";
            MySqlCommand offlineUserCommand = new MySqlCommand(offlineUserQuery, connection);
            offlineUserCommand.Parameters.AddWithValue("username", uname);
            offlineUserCommand.Parameters.AddWithValue("password", pword);
            offlineUserCommand.ExecuteNonQuery();
        }


        public string retrieveCurrentQueueNumWin1() {
            string retrieveCurrentQueueNumWin1Query = "SELECT MIN(TicketNo) FROM client INNER JOIN user ON client.UserID = user.UserID Where Windows = 'Window 1' AND Status = 'Ongoing' AND TicketNo IS NOT NULL; ";
            MySqlCommand retrieveCurrentQueueNumWin1Command = new MySqlCommand(retrieveCurrentQueueNumWin1Query, connection);
            var output = retrieveCurrentQueueNumWin1Command.ExecuteScalar();
            if (!(output is DBNull)) {
                string ticketNum = Convert.ToString(retrieveCurrentQueueNumWin1Command.ExecuteScalar());
                return ticketNum + "                            1";
            }
            else {
                return "";
            }
        }

        public string retrieveCurrentQueueNumWin2() {
            string retrieveCurrentQueueNumWin2Query = "SELECT MIN(TicketNo) FROM client INNER JOIN user ON client.UserID = user.UserID Where Windows = 'Window 2' AND Status = 'Ongoing' AND TicketNo IS NOT NULL; ";
            MySqlCommand retrieveCurrentQueueNumWin2Command = new MySqlCommand(retrieveCurrentQueueNumWin2Query, connection);
            var output = retrieveCurrentQueueNumWin2Command.ExecuteScalar();
            if (!(output is DBNull)) {
                string ticketNum = Convert.ToString(retrieveCurrentQueueNumWin2Command.ExecuteScalar());
                return ticketNum + "                            2";
            }
            else {
                return "";
            }
        }


        public string retrieveCurrentQueueNumCashier() {
            string retrieveCurrentQueueNumCashierQuery = "SELECT MIN(TicketNo) FROM client INNER JOIN user ON client.UserID = user.UserID Where Windows = 'Cashier' AND Status = 'Ongoing' AND TicketNo IS NOT NULL; ";
            MySqlCommand retrieveCurrentQueueNumCashierCommand = new MySqlCommand(retrieveCurrentQueueNumCashierQuery, connection);
            var output = retrieveCurrentQueueNumCashierCommand.ExecuteScalar();
            if (!(output is DBNull)) {
                string ticketNum = Convert.ToString(retrieveCurrentQueueNumCashierCommand.ExecuteScalar());
                return ticketNum + "                 Cashier";
            }
            else {
                return "";
            }
        }



        public string retrieveCurrentQueueNumAdmin() {
            string retrieveCurrentQueueNumAdminQuery = "SELECT MIN(TicketNo) FROM client INNER JOIN user ON client.UserID = user.UserID Where Windows = 'Administrator' AND Status = 'Ongoing' AND TicketNo IS NOT NULL; ";
            MySqlCommand retrieveCurrentQueueNumAdminCommand = new MySqlCommand(retrieveCurrentQueueNumAdminQuery, connection);
            var output = retrieveCurrentQueueNumAdminCommand.ExecuteScalar();
            if (!(output is DBNull)) {
                string ticketNum = Convert.ToString(retrieveCurrentQueueNumAdminCommand.ExecuteScalar());
                return ticketNum + "      Administrator";
            }
            else {
                return "";
            }
        }


        public string retrieveCurrentQueueNumDSA() {
            string retrieveCurrentQueueNumDSAQuery = "SELECT MIN(TicketNo) FROM client INNER JOIN user ON client.UserID = user.UserID Where Windows = 'DSA Office' AND Status = 'Ongoing' AND TicketNo IS NOT NULL; ";
            MySqlCommand retrieveCurrentQueueNumDSACommand = new MySqlCommand(retrieveCurrentQueueNumDSAQuery, connection);
            var output = retrieveCurrentQueueNumDSACommand.ExecuteScalar();
            if (!(output is DBNull)) {
                string ticketNum = Convert.ToString(retrieveCurrentQueueNumDSACommand.ExecuteScalar());
                return ticketNum + "           DSA Office";
            }
            else {
                return "";
            }
        }

        public List<string> retrieveUpcomingTenQueue(string window1, string window2, string Cashier, string Admin, string DSA) {
            string retrieveUpcomingTenQueueQuery = "SELECT TicketNo FROM client Where Status = 'Ongoing' AND TicketNo NOT IN (@win1, @win2, @cash , @admin , @dsa) LIMIT 10; ";
            MySqlCommand retrieveUpcomingTenQueueCommand = new MySqlCommand(retrieveUpcomingTenQueueQuery, connection);
            retrieveUpcomingTenQueueCommand.Parameters.AddWithValue("win1", window1);
            retrieveUpcomingTenQueueCommand.Parameters.AddWithValue("win2", window2);
            retrieveUpcomingTenQueueCommand.Parameters.AddWithValue("cash", Cashier);
            retrieveUpcomingTenQueueCommand.Parameters.AddWithValue("admin", Admin);
            retrieveUpcomingTenQueueCommand.Parameters.AddWithValue("dsa", DSA);
            MySqlDataReader reader = retrieveUpcomingTenQueueCommand.ExecuteReader();
            List<string> win = new List<string>();
            while (reader.Read()) {

                win.Add((string)Convert.ToString(reader["TicketNo"]));
            }
            reader.Close();
            return win;

        }



        public int retrieveRemainingQueue(int UID) {
            string retrieveRemainingQueueQuery = "SELECT COUNT(FullName) FROM client Where UserID = @UID AND Status = 'Ongoing'; ";
            MySqlCommand retrieveRemainingQueueCommand = new MySqlCommand(retrieveRemainingQueueQuery, connection);
            retrieveRemainingQueueCommand.Parameters.AddWithValue("UID", UID);
            int remQueue = Convert.ToInt32(retrieveRemainingQueueCommand.ExecuteScalar());
            return remQueue;
        }







    }
}
