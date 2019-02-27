IF schema_id('ExtraLife2018') IS NULL
    EXECUTE('CREATE SCHEMA [ExtraLife2018]')
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [ExtraLife2018].[Participant](
	[ParticipantId] [uniqueidentifier] NOT NULL,
	[ExtraLifeParticipantId] [int] NOT NULL,
	[ParticipantName] [nvarchar](400) NULL,
	[TeamId] [int] NULL,
	[TeamName] [nvarchar](400) NULL,
	[EventId] [int] NULL,
	[EventName] [nvarchar](400) NULL,
	[IsTeamCaptain] [bit] NULL,
	[AvatarImageURL] [nvarchar](1000) NULL,
	[FundraisingGoal] [decimal](9, 2),
	[CreatedDateUTC] [datetime] NULL,
	[ModifiedDateUTC] [datetime] NULL,
 CONSTRAINT [PK_ExtraLifeParticipantId] PRIMARY KEY CLUSTERED 
(
	[ExtraLifeParticipantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO