USE [PreactorProd]
GO

/****** Object:  View [dbo].[View_EquipmentNames]    Script Date: 12/19/2017 11:41:01 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE View [dbo].[View_EquipmentNames] as
Select equips.equip_name as EquipmentName,
equips.equip_id as EquipmentId,
equips.equip_superior as EquipmentSuperior
From SitMesDb.dbo.BPMV_EQPT equips
Join SitMesDb.dbo.BPMV_EQPT_CLS class on equips.equip_class_pk = class.equip_class_pk
where equips.equip_in_plant =1 and class.equip_class_name = 'Wi-equipment'


GO