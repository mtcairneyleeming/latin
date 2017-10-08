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
/****** Object:  Table [learn].[part_of_speech]    Script Date: 08/10/2017 03:12:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [learn].[part_of_speech](
	[part_id] [int] IDENTITY(1,1) NOT NULL,
	[part_name] [nvarchar](50) NOT NULL,
	[part_desc] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_part_of_speech] PRIMARY KEY CLUSTERED 
(
	[part_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [learn].[part_of_speech] ON 

INSERT [learn].[part_of_speech] ([part_id], [part_name], [part_desc]) VALUES (1, N'Noun', N'A term indicating a person, being, thing, place, phenomenon, quality or idea')
INSERT [learn].[part_of_speech] ([part_id], [part_name], [part_desc]) VALUES (2, N'Verb', N'A term indicating an action, occurence or state')
INSERT [learn].[part_of_speech] ([part_id], [part_name], [part_desc]) VALUES (3, N'Adjective', N'A term giving attributes to nouns')
INSERT [learn].[part_of_speech] ([part_id], [part_name], [part_desc]) VALUES (4, N'Conjunction', N'A term that connects words, phrases and clauses together')
INSERT [learn].[part_of_speech] ([part_id], [part_name], [part_desc]) VALUES (5, N'Numeral', N'A term for quantifying nouns')
INSERT [learn].[part_of_speech] ([part_id], [part_name], [part_desc]) VALUES (6, N'Adposition', N'A qualifying term, either a postposition or a preposition')
INSERT [learn].[part_of_speech] ([part_id], [part_name], [part_desc]) VALUES (7, N'Pronoun', N'A term that refers to and subsitutes for a noun')
SET IDENTITY_INSERT [learn].[part_of_speech] OFF
