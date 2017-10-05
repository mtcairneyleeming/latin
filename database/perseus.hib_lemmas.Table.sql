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
/****** Object:  Table [perseus].[hib_lemmas]    Script Date: 05/10/2017 05:52:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [perseus].[hib_lemmas](
	[lemma_id] [int] NOT NULL,
	[lemma_text] [nvarchar](100) NULL,
	[bare_headword] [nvarchar](100) NULL,
	[lemma_sequence_number] [int] NULL,
	[lemma_short_def] [nvarchar](255) NULL,
 CONSTRAINT [PK_hib_lemmas_lemma_id] PRIMARY KEY CLUSTERED 
(
	[lemma_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [bare_idx]    Script Date: 05/10/2017 05:52:40 PM ******/
CREATE NONCLUSTERED INDEX [bare_idx] ON [perseus].[hib_lemmas]
(
	[bare_headword] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [FK319881499970ED3]    Script Date: 05/10/2017 05:52:40 PM ******/
CREATE NONCLUSTERED INDEX [FK319881499970ED3] ON [perseus].[hib_lemmas]
(
	[lemma_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [perseus].[hib_lemmas] ADD  DEFAULT (NULL) FOR [lemma_text]
GO
ALTER TABLE [perseus].[hib_lemmas] ADD  DEFAULT (NULL) FOR [bare_headword]
GO
ALTER TABLE [perseus].[hib_lemmas] ADD  DEFAULT (NULL) FOR [lemma_sequence_number]
GO
ALTER TABLE [perseus].[hib_lemmas] ADD  DEFAULT (NULL) FOR [lemma_short_def]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'perseus.hib_lemmas' , @level0type=N'SCHEMA',@level0name=N'perseus', @level1type=N'TABLE',@level1name=N'hib_lemmas'
GO
