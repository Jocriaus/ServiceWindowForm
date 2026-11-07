using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Window_Queue {
    /// <summary>
    /// Opens one connection to the queueing_system database and serves the queue display's
    /// queries. The user/client methods are not used by the display; they are kept for the
    /// teller/admin apps that share this file.
    /// </summary>
    class MySQLDBConnection {

        public MySqlConnection connection;

        public MySQLDBConnection() {
            try {
                // Hard-coded for now; should move to App.config.
                string ConnectionString = "server=localhost;uid=root;pwd=mypasswordis01;database=queueing_system";
                connection = new MySqlConnection(ConnectionString);
                connection.Open();
            }
            catch (MySqlException e) {
                // After a failure `connection` stays unusable and every later query throws.
                // There is no retry.
                MessageBox.Show("Connection Error" + e);
            }
        }

        public string loginUser(string uname, string pword) {
            string loginQuery = "SELECT UserID FROM `user` WHERE UserName = @username AND Password = @password LIMIT 1";
            using (MySqlCommand loginCommand = new MySqlCommand(loginQuery, connection)) {
                loginCommand.Parameters.AddWithValue("username", uname);
                loginCommand.Parameters.AddWithValue("password", pword);
                using (MySqlDataReader reader = loginCommand.ExecuteReader()) {
                    if (reader.Read()) {
                        return "Successfully Login";
                    }
                    return "No Existing Account";
                }
            }
        }

        public string usernameExist(string uname) {
            string usernameQuery = "SELECT 1 FROM `user` WHERE UserName = @username LIMIT 1";
            using (MySqlCommand usernameCommand = new MySqlCommand(usernameQuery, connection)) {
                usernameCommand.Parameters.AddWithValue("username", uname);
                using (MySqlDataReader reader = usernameCommand.ExecuteReader()) {
                    if (reader.Read()) {
                        return "Record Already Exists.";
                    }
                    return "Record Not Exists.";
                }
            }
        }

        public void registerUser(string uname, string pword, string win) {
            string registerQuery = "INSERT INTO `user` (UserName, Password, Windows) VALUES (@username, @password, @window)";
            using (MySqlCommand registerCommand = new MySqlCommand(registerQuery, connection)) {
                registerCommand.Parameters.AddWithValue("username", uname);
                registerCommand.Parameters.AddWithValue("password", pword);
                registerCommand.Parameters.AddWithValue("window", win);
                registerCommand.ExecuteNonQuery();
            }
        }

        public void updateUser(string uname, string pword, string win) {
            string updateUserQuery = "UPDATE `user` SET Password = @password, Windows = @window WHERE UserName = @username";
            using (MySqlCommand updateUserCommand = new MySqlCommand(updateUserQuery, connection)) {
                updateUserCommand.Parameters.AddWithValue("password", pword);
                updateUserCommand.Parameters.AddWithValue("window", win);
                updateUserCommand.Parameters.AddWithValue("username", uname);
                updateUserCommand.ExecuteNonQuery();
            }
        }

        public List<string> retrieveAllUser() {
            string retrieveUserQuery = "SELECT UserName FROM `user` ORDER BY UserName";
            List<string> items = new List<string>();
            using (MySqlCommand retrieveUserCommand = new MySqlCommand(retrieveUserQuery, connection))
            using (MySqlDataReader reader = retrieveUserCommand.ExecuteReader()) {
                while (reader.Read()) {
                    items.Add((string)reader["UserName"]);
                }
            }
            return items;
        }

        public void deleteUser(string uname) {
            string deleteUserQuery = "DELETE FROM `user` WHERE UserName = @username";
            using (MySqlCommand deleteUserCommand = new MySqlCommand(deleteUserQuery, connection)) {
                deleteUserCommand.Parameters.AddWithValue("username", uname);
                deleteUserCommand.ExecuteNonQuery();
            }
        }

        // Returns an adapter that the caller fills and disposes (e.g. for a DataGridView).
        public MySqlDataAdapter displayUser() {
            string displayUsersQuery = "SELECT * FROM user";
            MySqlCommand displayUsersCommand = new MySqlCommand(displayUsersQuery, connection);
            MySqlDataAdapter sda = new MySqlDataAdapter(displayUsersCommand);
            return sda;
        }

        // Returns an adapter that the caller fills and disposes (e.g. for a DataGridView).
        public MySqlDataAdapter displayClient() {
            string displayClientQuery = "SELECT * FROM client";
            MySqlCommand displayClientCommand = new MySqlCommand(displayClientQuery, connection);
            MySqlDataAdapter sda = new MySqlDataAdapter(displayClientCommand);
            return sda;
        }

        public string retrieveUserWindow(string uname) {
            string retrieveUserWindowQuery = "SELECT Windows FROM `user` WHERE UserName = @username LIMIT 1";
            using (MySqlCommand retrieveUserWindowCommand = new MySqlCommand(retrieveUserWindowQuery, connection)) {
                retrieveUserWindowCommand.Parameters.AddWithValue("username", uname);
                return Convert.ToString(retrieveUserWindowCommand.ExecuteScalar());
            }
        }

        public void OnlineUser(string uname, string pword) {
            string onlineUserQuery = "UPDATE `user` SET OnlineStatus = 'online' WHERE UserName = @username AND Password = @password";
            using (MySqlCommand onlineUserCommand = new MySqlCommand(onlineUserQuery, connection)) {
                onlineUserCommand.Parameters.AddWithValue("username", uname);
                onlineUserCommand.Parameters.AddWithValue("password", pword);
                onlineUserCommand.ExecuteNonQuery();
            }
        }

        public void OfflineUser(string uname, string pword) {
            string offlineUserQuery = "UPDATE `user` SET OnlineStatus = 'offline' WHERE UserName = @username AND Password = @password";
            using (MySqlCommand offlineUserCommand = new MySqlCommand(offlineUserQuery, connection)) {
                offlineUserCommand.Parameters.AddWithValue("username", uname);
                offlineUserCommand.Parameters.AddWithValue("password", pword);
                offlineUserCommand.ExecuteNonQuery();
            }
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
            string retrieveRemainingQueueQuery = "SELECT COUNT(*) FROM client WHERE UserID = @UID AND Status = 'Ongoing'";
            using (MySqlCommand retrieveRemainingQueueCommand = new MySqlCommand(retrieveRemainingQueueQuery, connection)) {
                retrieveRemainingQueueCommand.Parameters.AddWithValue("UID", UID);
                return Convert.ToInt32(retrieveRemainingQueueCommand.ExecuteScalar());
            }
        }
    }
}
