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


        /// <summary>
        /// Returns window name -> current ticket number (as string) for every window
        /// that has an ongoing ticket. Windows with no ongoing ticket are absent.
        /// Keys are the database window names, e.g. "Window 1", "DSA Office", "Bookshop".
        /// </summary>
        public Dictionary<string, string> retrieveNowServing() {
            string nowServingQuery =
                "SELECT u.Windows, MIN(c.TicketNo) AS TicketNo " +
                "FROM client AS c " +
                "INNER JOIN `user` AS u ON u.UserID = c.UserID " +
                "WHERE c.Status = 'Ongoing' " +
                "AND c.TicketNo IS NOT NULL " +
                "GROUP BY u.Windows " +
                "ORDER BY u.Windows;";
            Dictionary<string, string> nowServing = new Dictionary<string, string>();
            using (MySqlCommand nowServingCommand = new MySqlCommand(nowServingQuery, connection))
            using (MySqlDataReader reader = nowServingCommand.ExecuteReader()) {
                while (reader.Read()) {
                    nowServing[Convert.ToString(reader["Windows"])] = Convert.ToString(reader["TicketNo"]);
                }
            }
            return nowServing;
        }

        /// <summary>
        /// Returns up to 10 ongoing ticket numbers, ascending, excluding the tickets
        /// currently shown under "Now Serving" (the lowest ongoing ticket of each window).
        /// Unassigned tickets (no UserID yet) are included: they are still waiting.
        /// </summary>
        public List<string> retrieveUpcomingTenQueue() {
            string upcomingQuery =
                "SELECT c.TicketNo " +
                "FROM client AS c " +
                "WHERE c.Status = 'Ongoing' " +
                "AND c.TicketNo IS NOT NULL " +
                "AND c.TicketNo NOT IN ( " +
                    // the tickets shown under "Now Serving" (same logic as retrieveNowServing)
                    "SELECT MIN(s.TicketNo) " +
                    "FROM client AS s " +
                    "INNER JOIN `user` AS u ON u.UserID = s.UserID " +
                    "WHERE s.Status = 'Ongoing' " +
                    "AND s.TicketNo IS NOT NULL " +
                    "GROUP BY u.Windows) " +
                "ORDER BY c.TicketNo " +
                "LIMIT 10;";
            List<string> upcoming = new List<string>();
            using (MySqlCommand upcomingCommand = new MySqlCommand(upcomingQuery, connection))
            using (MySqlDataReader reader = upcomingCommand.ExecuteReader()) {
                while (reader.Read()) {
                    upcoming.Add(Convert.ToString(reader["TicketNo"]));
                }
            }
            return upcoming;
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
