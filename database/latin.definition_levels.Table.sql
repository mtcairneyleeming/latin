USE [latin]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [learn].[definition_levels](
	[level_name] [nvarchar](50) NOT NULL,
	[level_number] [int] NOT NULL
) ON [PRIMARY]
GO
INSERT [learn].[definition_levels] ([level_name], [level_number]) VALUES (N'KS3', 0)
INSERT [learn].[definition_levels] ([level_name], [level_number]) VALUES (N'GCSE', 1)
INSERT [learn].[definition_levels] ([level_name], [level_number]) VALUES (N'A Level', 2)
INSERT [learn].[definition_levels] ([level_name], [level_number]) VALUES (N'Dictionary', 3)
INSERT [learn].[definition_levels] ([level_name], [level_number]) VALUES (N'Old GCSE', -1)
INSERT [learn].[definition_levels] ([level_name], [level_number]) VALUES (N'Old A Level', -2)
