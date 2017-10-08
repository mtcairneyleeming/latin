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
/****** Object:  Table [perseus].[lemmas]    Script Date: 08/10/2017 03:02:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [perseus].[lemmas](
	[lemma_id] [int] IDENTITY(1,1) NOT NULL,
	[lemma_text] [nvarchar](100) NULL,
	[lemma_short_def] [nvarchar](255) NULL,
 CONSTRAINT [PK_lemmas] PRIMARY KEY CLUSTERED 
(
	[lemma_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
