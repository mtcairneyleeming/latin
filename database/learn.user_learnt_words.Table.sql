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
/****** Object:  Table [learn].[user_learnt_words]    Script Date: 05/10/2017 05:52:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [learn].[user_learnt_words](
	[user_id] [int] NOT NULL,
	[lemma_id] [int] NOT NULL,
 CONSTRAINT [PK_user_learnt_words] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC,
	[lemma_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [learn].[user_learnt_words]  WITH CHECK ADD  CONSTRAINT [FK_user_learnt_words_lemmas] FOREIGN KEY([lemma_id])
REFERENCES [perseus].[lemmas] ([lemma_id])
GO
ALTER TABLE [learn].[user_learnt_words] CHECK CONSTRAINT [FK_user_learnt_words_lemmas]
GO
ALTER TABLE [learn].[user_learnt_words]  WITH CHECK ADD  CONSTRAINT [FK_user_learnt_words_user_learnt_words] FOREIGN KEY([user_id])
REFERENCES [learn].[users] ([user_id])
GO
ALTER TABLE [learn].[user_learnt_words] CHECK CONSTRAINT [FK_user_learnt_words_user_learnt_words]
GO
