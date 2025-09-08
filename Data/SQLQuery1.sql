CREATE PROCEDURE sp_CreateCertificate
    @TraineeId INT,
    @RegistrationId INT,
    @BatchId INT,
    @CourseId INT,
    @RecommendationId INT
AS
BEGIN
    DECLARE @NextNumber INT
    DECLARE @CertificateNumber NVARCHAR(100)

    -- Find the last number from CertificateNumber
    SELECT TOP 1 @NextNumber = 
        CAST(RIGHT(CertificateNumber, LEN(CertificateNumber) - 6) AS INT)
    FROM Certificates
    WHERE CertificateNumber LIKE 'CR No-%'
    ORDER BY CertificateId DESC

    SET @NextNumber = ISNULL(@NextNumber, 0) + 1
    SET @CertificateNumber = 'CR No-' + RIGHT('00' + CAST(@NextNumber AS VARCHAR), 2)

    INSERT INTO Certificates (
        TraineeId,
        RegistrationId,
        BatchId,
        CourseId,
        RecommendationId,
        IssueDate,
        CertificateNumber
    )
    VALUES (
        @TraineeId,
        @RegistrationId,
        @BatchId,
        @CourseId,
        @RecommendationId,
        GETDATE(),
        @CertificateNumber
    )

    SELECT SCOPE_IDENTITY() AS CertificateId
END
