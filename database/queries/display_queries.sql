USE queueing_system;

-- Now Serving: lowest ongoing ticket per window (MySQLDBConnection.retrieveNowServing)
SELECT u.Windows, MIN(c.TicketNo) AS TicketNo
FROM client AS c
INNER JOIN `user` AS u ON u.UserID = c.UserID
WHERE c.Status = 'Ongoing'
  AND c.TicketNo IS NOT NULL
GROUP BY u.Windows
ORDER BY u.Windows;

-- Upcoming: next 10 ongoing tickets not being served (MySQLDBConnection.retrieveUpcomingTenQueue)
SELECT c.TicketNo
FROM client AS c
WHERE c.Status = 'Ongoing'
  AND c.TicketNo IS NOT NULL
  AND c.TicketNo NOT IN (
      -- the tickets shown under "Now Serving" (same logic as above)
      SELECT MIN(s.TicketNo)
      FROM client AS s
      INNER JOIN `user` AS u ON u.UserID = s.UserID
      WHERE s.Status = 'Ongoing'
        AND s.TicketNo IS NOT NULL
      GROUP BY u.Windows)
ORDER BY c.TicketNo
LIMIT 10;
