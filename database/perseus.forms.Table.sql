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
/****** Object:  Table [perseus].[forms]    Script Date: 08/10/2017 03:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [perseus].[forms](
	[id] [int] NOT NULL,
	[lemma_id] [int] NOT NULL,
	[morph_code] [nvarchar](13) NULL,
	[form] [nvarchar](75) NULL,
	[misc_features] [nvarchar](50) NULL,
 CONSTRAINT [PK_forms] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [perseus].[forms]  WITH CHECK ADD  CONSTRAINT [FK_forms_lemmas] FOREIGN KEY([lemma_id])
REFERENCES [perseus].[lemmas] ([lemma_id])
GO
ALTER TABLE [perseus].[forms] CHECK CONSTRAINT [FK_forms_lemmas]
GO
