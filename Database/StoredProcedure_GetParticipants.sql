SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE or Alter PROCEDURE GetParticipants
AS
BEGIN TRY
	Select
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
	From [ExtraLife2018].[Participant]
END TRY

GO