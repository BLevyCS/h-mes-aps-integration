USE [PreactorProd]
GO

/****** Object:  View [dbo].[View_ToolDetails]    Script Date: 12/19/2017 11:44:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create view [dbo].[View_ToolDetails] as
SELECT * FROM(
Select 
orders.pom_order_pk as OrderPrimaryKey,
orders.pom_order_id as OrderId,
COALESCE([sequence],'') as Sequence,
ver.TOOL_DEF_NAME as ToolName,
COUNT(ver.TOOL_DEF_NAME) OVER (PARTITION BY ver.TOOL_DEF_NAME, orders.pom_order_id ORDER BY orders.pom_order_id) as ToolCount,
ROW_NUMBER() OVER (PARTITION BY ver.TOOL_DEF_NAME, orders.pom_order_id ORDER BY orders.pom_order_id) as RowNum
FROM SitMesDB.dbo.POMV_ORDR orders
	INNER JOIN SitMesDB.dbo.POMV_ORDR orders2 on orders.pom_order_id = orders2.ERP_ID
	INNER JOIN SitMesDB.dbo.POMV_ETRY entries on entries.pom_order_pk = orders2.pom_order_pk  
 AND entries.prnt_pom_entry_pk IS NULL
 INNER JOIN  SitMesDb.Dbo.POM_EQUIPMENT_SPECIFICATION as specification ON entries.pom_entry_pk = specification.pom_entry_pk 
 INNER JOIN  preactorprod.dbo.view_EquipmentNames as eqNames ON specification.name = eqNames.equipmentName
	INNER JOIN SitMesDB.dbo.PDMV_PS_TOOL_SPEC tool on entries.ps_name = tool.ps
	INNER JOIN SitMesDB.dbo.PDMV_PS_TOOL_ITM item on tool.NUM = item.SPEC_NUM
	INNER JOIN SitMesDB.dbo.MDSMM_TOOL_DEF_VER ver on item.id = ver.TOOL_DEF_ID) a
	where rownum = 1
GO


