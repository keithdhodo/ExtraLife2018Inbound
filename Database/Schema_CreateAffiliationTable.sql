SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [ExtraLife2018].[Affiliation](
    [AffiliationId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [AffiliationName] VARCHAR(200) NOT NULL
) ON [PRIMARY]
GO

CREATE INDEX IX_Affiliation_AffiliationId ON [ExtraLife2018].[Affiliation] (AffiliationId); 