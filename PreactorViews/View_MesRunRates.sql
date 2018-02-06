USE [PreactorProd]
GO

/****** Object:  View [dbo].[View_MesRunRates]    Script Date: 12/19/2017 11:42:15 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[View_MesRunRates] as
select
etry.pom_entry_id as OperationName,
etry.ERP_WRKCNTR as Equipment,
rates.VAL as UnitsPerHour
from SitMesDb.dbo.POMV_ETRY etry
join SitMesDb.dbo.PDMV_PS_PRP rates on etry.ps_name = rates.PS and etry.ps_ver = rates.PS_VER and etry.ppr_name = rates.PPR and etry.ppr_version = rates.PPR_VER and rates.name = 'TheoreticalRate'

GO


