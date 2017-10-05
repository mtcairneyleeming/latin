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
/****** Object:  Table [perseus].[hib_parses]    Script Date: 05/10/2017 05:52:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [perseus].[hib_parses](
	[id] [int] IDENTITY(1440255,1) NOT NULL,
	[lemma_id] [int] NOT NULL,
	[morph_code] [nvarchar](13) NULL,
	[expanded_form] [nvarchar](75) NULL,
	[form] [nvarchar](75) NOT NULL,
	[bare_form] [nvarchar](75) NULL,
	[dialects] [nvarchar](50) NULL,
	[misc_features] [nvarchar](50) NULL,
 CONSTRAINT [PK_hib_parses_id] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [bare_form_idx]    Script Date: 05/10/2017 05:52:40 PM ******/
CREATE NONCLUSTERED INDEX [bare_form_idx] ON [perseus].[hib_parses]
(
	[bare_form] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [FK3835E29E9970ED3]    Script Date: 05/10/2017 05:52:40 PM ******/
CREATE NONCLUSTERED INDEX [FK3835E29E9970ED3] ON [perseus].[hib_parses]
(
	[lemma_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [form_idx]    Script Date: 05/10/2017 05:52:40 PM ******/
CREATE NONCLUSTERED INDEX [form_idx] ON [perseus].[hib_parses]
(
	[form] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [form_lemma]    Script Date: 05/10/2017 05:52:40 PM ******/
CREATE NONCLUSTERED INDEX [form_lemma] ON [perseus].[hib_parses]
(
	[lemma_id] ASC,
	[form] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [lemma_id]    Script Date: 05/10/2017 05:52:40 PM ******/
CREATE NONCLUSTERED INDEX [lemma_id] ON [perseus].[hib_parses]
(
	[lemma_id] ASC,
	[morph_code] ASC,
	[expanded_form] ASC,
	[form] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [perseus].[hib_parses] ADD  DEFAULT (NULL) FOR [morph_code]
GO
ALTER TABLE [perseus].[hib_parses] ADD  DEFAULT (NULL) FOR [expanded_form]
GO
ALTER TABLE [perseus].[hib_parses] ADD  DEFAULT (NULL) FOR [bare_form]
GO
ALTER TABLE [perseus].[hib_parses] ADD  DEFAULT (NULL) FOR [dialects]
GO
ALTER TABLE [perseus].[hib_parses] ADD  DEFAULT (NULL) FOR [misc_features]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_SSMA_SOURCE', @value=N'perseus.hib_parses' , @level0type=N'SCHEMA',@level0name=N'perseus', @level1type=N'TABLE',@level1name=N'hib_parses'
GO
