CREATE LOGIN agenda_user WITH PASSWORD = 'replace-with-a-secure-database-password';

USE AgendaInteligenteDb;

CREATE USER agenda_user FOR LOGIN agenda_user;

ALTER ROLE db_datareader ADD MEMBER agenda_user;
ALTER ROLE db_datawriter ADD MEMBER agenda_user;
ALTER ROLE db_ddladmin ADD MEMBER agenda_user;
