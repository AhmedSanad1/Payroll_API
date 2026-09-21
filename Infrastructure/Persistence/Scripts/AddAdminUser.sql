-- Adds an extra admin login (Username = Admin, Password = Admin123#).
-- Hash generated with BCrypt.Net-Next 4.2.0, work factor 12 — same as Infrastructure/Services/PasswordHashing.cs.
-- Idempotent: does nothing if the username already exists.
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM security.AdminUsers WHERE Username = N'Admin')
BEGIN
    INSERT INTO security.AdminUsers (Username, PasswordHash, IsActive, CreatedAt)
    VALUES (N'Admin', '$2a$12$4oI6qFmxn4O3L/tO52qWLu7jSGxeS8PlZ8paM18PbUY1GqnvDrKLa', 1, SYSUTCDATETIME());
    PRINT 'Admin user created.';
END
ELSE
    PRINT 'Admin user already exists - nothing changed.';

SELECT Id, Username, IsActive, CreatedAt FROM security.AdminUsers;
