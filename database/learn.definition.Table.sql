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
/****** Object:  Table [learn].[definition]    Script Date: 08/10/2017 03:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [learn].[definition](
	[definition_id] [int] IDENTITY(1,1) NOT NULL,
	[lemma_id] [int] NOT NULL,
	[alevel] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_definition_1] PRIMARY KEY CLUSTERED 
(
	[definition_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [learn].[definition]  WITH CHECK ADD  CONSTRAINT [FK_definition_lemmas] FOREIGN KEY([lemma_id])
REFERENCES [perseus].[lemmas] ([lemma_id])
GO
ALTER TABLE [learn].[definition] CHECK CONSTRAINT [FK_definition_lemmas]
GO
