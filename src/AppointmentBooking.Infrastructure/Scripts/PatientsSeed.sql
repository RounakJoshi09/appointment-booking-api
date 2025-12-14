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

IF NOT EXISTS (SELECT 1 FROM Patients WHERE Email = 'michael.johnson@example.com')
BEGIN
    INSERT INTO Patients (Id, Name, Email, CreatedAt, UpdatedAt)
    VALUES (NEWID(), 'Michael Johnson', 'michael.johnson@example.com', GETUTCDATE(), GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM Patients WHERE Email = 'sarah.williams@example.com')
BEGIN
    INSERT INTO Patients (Id, Name, Email, CreatedAt, UpdatedAt)
    VALUES (NEWID(), 'Sarah Williams', 'sarah.williams@example.com', GETUTCDATE(), GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM Patients WHERE Email = 'david.brown@example.com')
BEGIN
    INSERT INTO Patients (Id, Name, Email, CreatedAt, UpdatedAt)
    VALUES (NEWID(), 'David Brown', 'david.brown@example.com', GETUTCDATE(), GETUTCDATE());
END
