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
/****** Object:  Table [link].[lists_lemmas]    Script Date: 05/10/2017 05:52:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [link].[lists_lemmas](
	[list_id] [int] NOT NULL,
	[lemma_id] [int] NOT NULL,
 CONSTRAINT [PK_lists_lemmas_link] PRIMARY KEY CLUSTERED 
(
	[list_id] ASC,
	[lemma_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [link].[lists_lemmas]  WITH CHECK ADD  CONSTRAINT [FK_lists_lemmas_link_lemmas] FOREIGN KEY([lemma_id])
REFERENCES [perseus].[lemmas] ([lemma_id])
GO
ALTER TABLE [link].[lists_lemmas] CHECK CONSTRAINT [FK_lists_lemmas_link_lemmas]
GO
ALTER TABLE [link].[lists_lemmas]  WITH CHECK ADD  CONSTRAINT [FK_lists_lemmas_link_lists_lemmas_link] FOREIGN KEY([list_id])
REFERENCES [learn].[lists] ([id])
GO
ALTER TABLE [link].[lists_lemmas] CHECK CONSTRAINT [FK_lists_lemmas_link_lists_lemmas_link]
GO
