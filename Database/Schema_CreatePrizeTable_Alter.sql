SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [ExtraLife2018].[tblPrizes] ADD Tier decimal(18,2) NULL;
ALTER TABLE [ExtraLife2018].[tblPrizes] ADD DisplayDate DATETIME
	constraint DF_tblPrizes_DisplayDate default(getdate());
ALTER TABLE [ExtraLife2018].[tblPrizes] ADD DateAdded DateTime 
	constraint DF_tblPrizes_DateAdded default(getdate());
GO 

CREATE INDEX IX_tblPrizes_PkPrize ON [ExtraLife2018].[tblPrizes] (PkPrize);