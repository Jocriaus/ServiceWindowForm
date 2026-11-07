-- DEVELOPMENT ONLY. Do not run on the live queue database.
-- Loads the worked example from the design spec (§3.3). Re-running resets rows 1-10 to this state.
USE queueing_system;
INSERT INTO `user` (UserID, UserName, Password, Windows) VALUES
  (1, 'window1',   'dev123', 'Window 1'),
  (2, 'window2',   'dev123', 'Window 2'),
  (3, 'cashier',   'dev123', 'Cashier'),
  (4, 'admission', 'dev123', 'Admission'),
  (5, 'dsa',       'dev123', 'DSA Office'),
  (6, 'bookshop',  'dev123', 'Bookshop')
ON DUPLICATE KEY UPDATE Windows = VALUES(Windows);

INSERT INTO client (ClientID, FullName, TicketNo, Status, UserID) VALUES
  (1, 'Ana Cruz',       101,  'Done',    1),
  (2, 'Ben Reyes',      102,  'Ongoing', 1),
  (3, 'Carla Santos',   103,  'Ongoing', 2),
  (4, 'Dan Lim',        104,  'Ongoing', 3),
  (5, 'Ella Tan',       105,  'Ongoing', 1),
  (6, 'Finn Go',        106,  'Ongoing', 3),
  (7, 'Gia Ramos',      107,  'Ongoing', 4),
  (8, 'Hugo Dela Cruz', 108,  'Ongoing', 2),
  (9, 'Ivy Mendoza',    NULL, 'Ongoing', 5),
  (10,'Jon Bautista',   110,  'Ongoing', NULL)
ON DUPLICATE KEY UPDATE TicketNo = VALUES(TicketNo), Status = VALUES(Status), UserID = VALUES(UserID);
