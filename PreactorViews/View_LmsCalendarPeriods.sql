USE [PreactorProd]
GO

/****** Object:  View [WI\BRLE17].[View_LmsCalendarPeriods]    Script Date: 12/19/2017 11:49:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[View_LmsCalendarPeriods] AS
SELECT 
	lcp.Id, 
	lcp.ResourceId, 
	lcp.FromDate, 
	lcp.ToDate,
	DATEDIFF(MINUTE, lcp.FromDate, lcp.ToDate) as MinutesInState,
	lcp.TimeCategory, 
	lcp.Level1, 
	lcp.Level2, 
	lcp.Level3, 
	lcp.Level4,
	equip.EquipmentId as Name,
	lcp.Consumed 
FROM [PreactorProd].[Calendar].[LmsCalendarPeriods] lcp
JOIN [PreactorProd].[Calendar].[PrimaryResources] pr on lcp.ResourceId = pr.Id
JOIN [PreactorProd].[dbo].[View_EquipmentNames] equip on pr.name = equip.EquipmentName



GO


