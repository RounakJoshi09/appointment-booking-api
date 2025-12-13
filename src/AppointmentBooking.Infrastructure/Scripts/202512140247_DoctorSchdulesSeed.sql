
DECLARE @DrSmithId UNIQUEIDENTIFIER;
SELECT @DrSmithId = Id FROM Doctors WHERE Email = 'dr.smith@example.com';

IF @DrSmithId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE DoctorId = @DrSmithId AND DayOfWeek = 1)
    BEGIN
        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, StartTime, EndTime, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrSmithId, 1, '09:00:00', '17:00:00', 0, GETUTCDATE(), GETUTCDATE());
    END

    IF NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE DoctorId = @DrSmithId AND DayOfWeek = 2)
    BEGIN
        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, StartTime, EndTime, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrSmithId, 2, '09:00:00', '17:00:00', 0, GETUTCDATE(), GETUTCDATE());
    END

    IF NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE DoctorId = @DrSmithId AND DayOfWeek = 3)
    BEGIN
        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, StartTime, EndTime, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrSmithId, 3, '09:00:00', '17:00:00', 0, GETUTCDATE(), GETUTCDATE());
    END

    IF NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE DoctorId = @DrSmithId AND DayOfWeek = 4)
    BEGIN
        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, StartTime, EndTime, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrSmithId, 4, '09:00:00', '17:00:00', 0, GETUTCDATE(), GETUTCDATE());
    END

    IF NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE DoctorId = @DrSmithId AND DayOfWeek = 5)
    BEGIN
        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, StartTime, EndTime, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrSmithId, 5, '09:00:00', '17:00:00', 0, GETUTCDATE(), GETUTCDATE());
    END

    IF NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE DoctorId = @DrSmithId AND DayOfWeek = 6)
    BEGIN
        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrSmithId, 6, 1, GETUTCDATE(), GETUTCDATE());
    END

    IF NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE DoctorId = @DrSmithId AND DayOfWeek = 0)
    BEGIN
        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrSmithId, 0, 1, GETUTCDATE(), GETUTCDATE());
    END
END

DECLARE @DrJonesId UNIQUEIDENTIFIER;
SELECT @DrJonesId = Id FROM Doctors WHERE Email = 'dr.jones@example.com';

IF @DrJonesId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE DoctorId = @DrJonesId AND DayOfWeek = 1)
    BEGIN
        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, StartTime, EndTime, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrJonesId, 1, '10:00:00', '18:00:00', 0, GETUTCDATE(), GETUTCDATE());
    END

    IF NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE DoctorId = @DrJonesId AND DayOfWeek = 2)
    BEGIN
        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, StartTime, EndTime, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrJonesId, 2, '10:00:00', '18:00:00', 0, GETUTCDATE(), GETUTCDATE());

        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, StartTime, EndTime, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrJonesId, 2, '22:00:00', '23:00:00', 0, GETUTCDATE(), GETUTCDATE());
    END

    IF NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE DoctorId = @DrJonesId AND DayOfWeek = 3)
    BEGIN
        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, StartTime, EndTime, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrJonesId, 3, '10:00:00', '18:00:00', 0, GETUTCDATE(), GETUTCDATE());
    END

    IF NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE DoctorId = @DrJonesId AND DayOfWeek = 4)
    BEGIN
        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, StartTime, EndTime, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrJonesId, 4, '10:00:00', '18:00:00', 0, GETUTCDATE(), GETUTCDATE());
    END

    IF NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE DoctorId = @DrJonesId AND DayOfWeek = 5)
    BEGIN
        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, StartTime, EndTime, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrJonesId, 5, '10:00:00', '14:00:00', 0, GETUTCDATE(), GETUTCDATE());
    END

    IF NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE DoctorId = @DrJonesId AND DayOfWeek = 6)
    BEGIN
        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrJonesId, 6, 1, GETUTCDATE(), GETUTCDATE());
    END

    IF NOT EXISTS (SELECT 1 FROM DoctorSchedules WHERE DoctorId = @DrJonesId AND DayOfWeek = 0)
    BEGIN
        INSERT INTO DoctorSchedules (Id, DoctorId, DayOfWeek, IsOffDay, CreatedAt, UpdatedAt)
        VALUES (NEWID(), @DrJonesId, 0, 1, GETUTCDATE(), GETUTCDATE());
    END
END
