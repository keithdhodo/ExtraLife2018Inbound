SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE or Alter PROCEDURE GetDonations
AS
BEGIN TRY
	Select
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
	From [ExtraLife2018].[Donation]
END TRY

BEGIN CATCH
	
END CATCH

RETURN

GO