IF NOT EXISTS (SELECT 1 FROM Patients WHERE Email = 'john.doe@example.com')
BEGIN
    INSERT INTO Patients (Id, Name, Email, CreatedAt, UpdatedAt)
    VALUES (NEWID(), 'John Doe', 'john.doe@example.com', GETUTCDATE(), GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM Patients WHERE Email = 'jane.smith@example.com')
BEGIN
    INSERT INTO Patients (Id, Name, Email, CreatedAt, UpdatedAt)
    VALUES (NEWID(), 'Jane Smith', 'jane.smith@example.com', GETUTCDATE(), GETUTCDATE());
END