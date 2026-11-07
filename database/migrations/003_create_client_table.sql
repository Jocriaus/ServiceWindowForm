-- Creates the `client` table: one row per queued person, their ticket and status.
USE queueing_system;
CREATE TABLE IF NOT EXISTS client (
  ClientID INT          NOT NULL AUTO_INCREMENT,
  FullName VARCHAR(100) NOT NULL,
  TicketNo INT          NULL,
  Status   VARCHAR(20)  NOT NULL DEFAULT 'Ongoing',
  UserID   INT          NULL,
  PRIMARY KEY (ClientID),
  KEY idx_client_status_ticket (Status, TicketNo),
  KEY idx_client_user (UserID),
  CONSTRAINT fk_client_user FOREIGN KEY (UserID)
    REFERENCES `user` (UserID) ON DELETE SET NULL
) ENGINE=InnoDB;
