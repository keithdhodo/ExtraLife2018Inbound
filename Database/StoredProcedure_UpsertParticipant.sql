SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE or Alter PROCEDURE InsertParticipant 
	@ParticipantId uniqueidentifier,
	@ExtraLifeParticipantId bigint,
	@ParticipantName varchar(400),
	@TeamId bigint,
	@TeamName varchar(400),
	@EventId bigint,
	@EventName varchar(400),
	@IsTeamCaptain bit,
	@AvatarImageURL varchar(1000),
	@FundraisingGoal decimal(9,2),
	@CreatedDateUTC DATETIME
AS
BEGIN TRY
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION MergeParticipant

		MERGE INTO [ExtraLife2018].[Participant] AS Target  
		USING (SELECT @ExtraLifeParticipantId)  
			   AS Source (ExtraLifeParticipantId)  
		ON (Target.ExtraLifeParticipantId = Source.ExtraLifeParticipantId)
		WHEN MATCHED THEN  
		UPDATE SET 
		ParticipantId = @ParticipantId,
		ExtraLifeParticipantId = @ExtraLifeParticipantId,
		ParticipantName = @ParticipantName,
		TeamId = @TeamId,
		TeamName = @TeamName,
		EventId = @EventId,
		EventName = @EventName,
		IsTeamCaptain = @IsTeamCaptain,
		AvatarImageURL = @AvatarImageURL,
		FundraisingGoal = @FundraisingGoal,
		CreatedDateUTC = @CreatedDateUTC,
		ModifiedDateUTC = GETUTCDATE()  
		WHEN NOT MATCHED BY TARGET THEN  
		INSERT (
		[ParticipantId],
		[ExtraLifeParticipantId],
		[ParticipantName],
		[TeamId],
		[TeamName],
		[EventId],
		[EventName],
		[IsTeamCaptain],
		[AvatarImageURL],
		[FundraisingGoal],
		[CreatedDateUTC],
		[ModifiedDateUTC]
		) 
		VALUES (
		@ParticipantId,
		@ExtraLifeParticipantId,
		@ParticipantName,
		@TeamId,
		@TeamName,
		@EventId,
		@EventName,
		@IsTeamCaptain,
		@AvatarImageURL,
		@FundraisingGoal,
		@CreatedDateUTC,
		GETUTCDATE()
		);

	COMMIT TRANSACTION MergeParticipant
END TRY

BEGIN CATCH
	IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION MergeParticipant;
			THROW;
		END
END CATCH

RETURN @@ROWCOUNT

GO

GRANT EXECUTE TO extralife2018test;  