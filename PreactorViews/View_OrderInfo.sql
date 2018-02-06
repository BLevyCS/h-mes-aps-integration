USE [PreactorProd]
GO

/****** Object:  View [dbo].[View_OrderInfo]    Script Date: 12/19/2017 11:42:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[View_OrderInfo] as
SELECT 
 -- Orders Fields
 orders.pom_order_pk as OrderPrimaryKey,
 orders.pom_order_id as OrderId,
 COALESCE(orders.matl_def_id, '') as MaterialDefinitionId,
 COALESCE(orders.ppr_name,'') as Recipe,
 COALESCE(orders.ppr_version,'') as RecipeVersion,
 COALESCE(orders.actual_start_time,'') as ActualStart,
 COALESCE(orders.actual_end_time,'') as ActualEndTime,
 COALESCE(orders.release_date,'') as ReleaseDate,
 COALESCE(orders.due_date,'') as DueDate,
 COALESCE(orders.LatestStartTime,'') as LatestStartTime,
 COALESCE(orders.[estimated_start_time],'') as EstimatedStart,
 COALESCE(orders.[estimated_end_time],'') as EstimatedEnd,
 COALESCE(orders.[pom_matl_qty],'') as MaterialQuantity,
 COALESCE(orders.[initial_qty],'') as  InitialQuantity,
 COALESCE(orders.[produced_qty],'') as ProducedQuantity,
 COALESCE(orders.[scrapped_qty],'') as ScrapQuantity,
 COALESCE(orders.[pom_order_status_id],'') as OrderStatus,
 COALESCE(orders.[pom_order_family_id],'') as OrderFamily,
 COALESCE(orders.[pom_order_type_id],'') as OrderType,
 COALESCE(orders.[ERP_ID],'') as ErpOrder,
 -- Entries Fields
 entries.[pom_entry_id] as EntryId,
 entries.[pom_entry_pk] as EntryPrimaryKey,
 COALESCE(
 CASE [fixed_duration_uom_id]
  WHEN 's'  THEN entries.[fixed_duration]/60/60/24 
  WHEN 'min'  THEN entries.[fixed_duration]/60/24 
  WHEN 'hour' THEN entries.[fixed_duration]/24
  WHEN 'day'  THEN entries.[fixed_duration]
  ELSE entries.[fixed_duration]/24
 END
 ,'')  AS FixedDuration, 
 COALESCE([fixed_duration_uom_id],'') as FixedDurationUoM,
 COALESCE(entries.[variable_duration],'') as VariableDuration,
 COALESCE([variable_duration_uom_id],'') as VariableDurationUoM,
 COALESCE([setup_duration],'') as SetupDuration,
 COALESCE([setup_duration_uom_id],'') as SetupDurationUoM,
 COALESCE(entries.[estimated_start_time],'') as EntryEstimatedStartTime,
 COALESCE(entries.[estimated_end_time],'') as EntryEstimatedEndTime,
 COALESCE(entries.[actual_start_time],'') as EntryActualStartTime,
 COALESCE(entries.[actual_end_time],'') as EntryActualEndTime,
 COALESCE([earliest_start_time],'') as EarliestStartTime,
 COALESCE([latest_end_time],'') as LatestEndTime,
 COALESCE([imposed_start_time],'') as ImposedStartTime,
 COALESCE([imposed_end_time],'') as ImposedEndTime,
 COALESCE(entries.[exec_time],'') as EntryExecutionTime,
 COALESCE(entries.[initial_qty],'') as EntryInitialQuantity,
 COALESCE(entries.[produced_qty],'') as EntryProducedQuantity,
 COALESCE(entries.[scrapped_qty],'') as EntryScrappedQuantity,
 COALESCE(entries.[pom_entry_status_id],'') as EntryStatusId,
 COALESCE(entries.[matl_def_id],'') as EntryMaterialDefinitionId,
 COALESCE(entries.[bom_id],'') as EntryBillOfMaterialId,
 COALESCE(entries.[pom_matl_qty],'') as EntryMaterialDefinitionQuantity,
 COALESCE([sequence],'') as Sequence,
 COALESCE([ps_name],'') as ProductSegmentName,
 COALESCE(specification.NAME,'') as Workcenter,
 COALESCE(specification.NAME,'') as WorkcenterUnit,
'Res. Specific Rate Per Hour' as ProcTimeType
FROM
SitMesDB.dbo.POMV_ORDR orders
	INNER JOIN SitMesDB.dbo.POMV_ORDR orders2 on orders.pom_order_id = orders2.ERP_ID
	INNER JOIN SitMesDB.dbo.POMV_ETRY entries on entries.pom_order_pk = orders2.pom_order_pk  
 --SitMesDb.Dbo.POMV_ETRY entries
 --INNER JOIN  SitMesDb.Dbo.POMV_ORDR orders on entries.pom_order_pk = orders.pom_order_pk
 AND entries.prnt_pom_entry_pk IS NULL
 INNER JOIN  SitMesDb.Dbo.POM_EQUIPMENT_SPECIFICATION as specification ON entries.pom_entry_pk = specification.pom_entry_pk 
 INNER JOIN  view_EquipmentNames as eqNames ON specification.name = eqNames.equipmentName






GO


