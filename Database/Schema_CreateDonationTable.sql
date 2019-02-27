/****** Object:  Table [ExtraLife2018].[Donation]    Script Date: 11/3/2018 2:42:23 PM ******/
CREATE TABLE [ExtraLife2018].[Donation](
	[DonationId] [uniqueidentifier] NOT NULL,
	[ExtraLifeDonationId] [nvarchar](400) NOT NULL,
	[ParticipantId] [uniqueidentifier] NOT NULL,
	[ExtraLifeParticipantId] [int] NULL,
	[DonorName] [nvarchar](400) NULL,
	[TeamId] [int] NULL,
	[Amount] [decimal](9, 2),
	[Message] [nvarchar](4000) NULL,
	[AvatarImageURL] [nvarchar](1000) NULL,
	[CreatedDateUTC] [datetime] NULL,
	[ModifiedDateUTC] [datetime] NULL,
 CONSTRAINT [PK_Donation] PRIMARY KEY CLUSTERED 
(
	[DonationId] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
CONSTRAINT FK_Donation_ParticipantId FOREIGN KEY (ExtraLifeParticipantId)  
REFERENCES [ExtraLife2018].[Participant] (ExtraLifeParticipantId)     
	ON DELETE CASCADE    
	ON UPDATE CASCADE
) ON [PRIMARY]

