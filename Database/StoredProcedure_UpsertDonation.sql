SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE InsertDonation 
	@DonationId uniqueidentifier,
	@ExtraLifeDonationId varchar(400),
	@ParticipantId uniqueidentifier,
	@ExtraLifeParticipantId bigint,
	@DonorName varchar(400),
	@TeamId bigint,
	@Amount decimal(9,2),
	@Message varchar(4000),
	@AvatarImageURL varchar(1000),
	@CreatedDateUTC DATETIME
AS
BEGIN TRY
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION MergeDonation

		MERGE INTO [ExtraLife2018].[Donation] AS Target  
		USING (SELECT @DonationId)  
			   AS Source (DonationId)  
		ON (Target.DonationId = Source.DonationId)
		WHEN MATCHED THEN  
		UPDATE SET 
		DonationId = @DonationId,
		ExtraLifeDonationId = @ExtraLifeDonationId,
		ParticipantId = @ParticipantId,
		ExtraLifeParticipantId = @ExtraLifeParticipantId,
		DonorName = @DonorName,
		TeamId = @TeamId,
		Amount = @Amount,
		[Message] = @Message,
		AvatarImageURL = @AvatarImageURL,
		CreatedDateUTC = @CreatedDateUTC,
		ModifiedDateUTC = GETUTCDATE()  
		WHEN NOT MATCHED BY TARGET THEN  
		INSERT (
		[DonationId],
		[ExtraLifeDonationId],
		[ParticipantId],
		[ExtraLifeParticipantId],
		[DonorName],
		[TeamId],
		[Amount],
		[Message],
		[AvatarImageURL],
		[CreatedDateUTC],
		[ModifiedDateUTC]
		) 
		VALUES (
		@DonationId,
		@ExtraLifeDonationId,
		@ParticipantId,
		@ExtraLifeParticipantId,
		@DonorName,
		@TeamId,
		@Amount,
		@Message,
		@AvatarImageURL,
		@CreatedDateUTC,
		GETUTCDATE()
		);

	COMMIT TRANSACTION MergeDonation
END TRY

BEGIN CATCH
	IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION MergeDonation;
			THROW;
		END
END CATCH

RETURN @@ROWCOUNT

GO

GRANT EXECUTE TO extralife2018test;