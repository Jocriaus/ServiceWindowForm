-- Creates the `user` table: one row per teller account and the window it serves.
USE queueing_system;
CREATE TABLE IF NOT EXISTS `user` (
  UserID       INT          NOT NULL AUTO_INCREMENT,
  UserName     VARCHAR(50)  NOT NULL,
  Password     VARCHAR(255) NOT NULL,
  Windows      VARCHAR(50)  NOT NULL,
  OnlineStatus VARCHAR(10)  NOT NULL DEFAULT 'offline',
  PRIMARY KEY (UserID),
  UNIQUE KEY uq_user_username (UserName)
) ENGINE=InnoDB;
