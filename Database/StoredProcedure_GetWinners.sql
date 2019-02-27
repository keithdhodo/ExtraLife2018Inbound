SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE or Alter PROCEDURE GetWinners
AS
BEGIN TRY
	SELECT
	  [pkPrize] as prizeId
	  ,[DisplayDate] as dateToDisplay
	  ,[DateAdded] as dateAdded
	  ,(select [WinningTimeStamp] from [ExtraLife2018].[tblWinning] tw where pkPrize = tw.fkPrize and IsDeclined = 0) as dateWon
	  ,[PrizeDesc] as prizeName
      ,'' as donor
	  ,CONVERT(DECIMAL(4,2), [TIER]/1.0) as tier
	  ,'' as notes
	  ,(select [AffiliationName] from [ExtraLife2018].[Affiliation] A where fkPrizeAffiliation = A.AffiliationId) as restriction
	  ,(select [DonorName] from [ExtraLife2018].[tblWinning] tw where pkPrize = tw.fkPrize and IsDeclined = 0) as wonBy
	  ,(select [TeamMember] from [ExtraLife2018].[tblWinning] tw where pkPrize = tw.fkPrize and IsDeclined = 0) as donatedThrough 
    FROM [ExtraLife2018].[tblPrizes]
    --Where [IsDeclined] <> 1
    For JSON PATH
END TRY

BEGIN CATCH
	
END CATCH

RETURN

GO