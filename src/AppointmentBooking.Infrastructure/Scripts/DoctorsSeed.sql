IF NOT EXISTS (SELECT 1 FROM Doctors WHERE Email = 'dr.smith@example.com')
BEGIN
    INSERT INTO Doctors (Id, Name, Email, CreatedAt, UpdatedAt)
    VALUES (NEWID(), 'Dr. John Smith', 'dr.smith@example.com', GETUTCDATE(), GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM Doctors WHERE Email = 'dr.jones@example.com')
BEGIN
    INSERT INTO Doctors (Id, Name, Email, CreatedAt, UpdatedAt)
    VALUES (NEWID(), 'Dr. Jane Jones', 'dr.jones@example.com', GETUTCDATE(), GETUTCDATE());
END