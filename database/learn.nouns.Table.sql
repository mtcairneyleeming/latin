/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2016 (13.0.4206)
    Source Database Engine Edition : Microsoft SQL Server Enterprise Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Standard Edition
    Target Database Engine Type : Standalone SQL Server
*/
USE [latin]
GO
/****** Object:  Table [learn].[nouns]    Script Date: 05/10/2017 05:52:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [learn].[nouns](
	[noun_id] [int] IDENTITY(1,1) NOT NULL,
	[lemma_id] [int] NOT NULL,
	[declension_id] [int] NOT NULL,
	[gender_id] [int] NULL,
	[use_singular] [bit] NOT NULL,
 CONSTRAINT [PK_nouns] PRIMARY KEY CLUSTERED 
(
	[noun_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [learn].[nouns]  WITH CHECK ADD  CONSTRAINT [FK_nouns_declensions] FOREIGN KEY([gender_id])
REFERENCES [link].[genders] ([gender_id])
GO
ALTER TABLE [learn].[nouns] CHECK CONSTRAINT [FK_nouns_declensions]
GO
ALTER TABLE [learn].[nouns]  WITH CHECK ADD  CONSTRAINT [FK_nouns_declensions1] FOREIGN KEY([declension_id])
REFERENCES [link].[declensions] ([declension_id])
GO
ALTER TABLE [learn].[nouns] CHECK CONSTRAINT [FK_nouns_declensions1]
GO
ALTER TABLE [learn].[nouns]  WITH CHECK ADD  CONSTRAINT [FK_nouns_lemmas] FOREIGN KEY([lemma_id])
REFERENCES [perseus].[lemmas] ([lemma_id])
GO
ALTER TABLE [learn].[nouns] CHECK CONSTRAINT [FK_nouns_lemmas]
GO
