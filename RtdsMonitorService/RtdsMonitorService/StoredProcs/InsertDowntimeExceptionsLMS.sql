USE [PreactorProd]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<G.Miller>
-- Create date: <12/13/2017>
-- Description:	<Insert Calendar Exceptions from LMS into Calendar.PrimaryCalendarPeriods>
-- =============================================
CREATE PROCEDURE [Calendar].[InsertDowntimeExceptionsLMS]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

Delete From Calendar.PrimaryCalendarPeriods
Where IsException =1 and CostFactor = 1 and DatasetId =15


INSERT INTO Calendar.PrimaryCalendarPeriods 
	(ResourceID,TemplateId,StateId,IsException,FromDate,ToDate,ReferenceDate,ReferenceDateType,Efficiency,CostFactor,DatasetId)
Select ResourceID,TemplateId,StateId,IsException,FromDate,ToDate,ReferenceDate,ReferenceDateType,Efficiency,CostFactor,DatasetId
From (Select ResourceId,
	   TemplateId,
	   cs.Id as StateId,
	   cs.Name as State,
	   1 as IsException,
	   Cast(FromDate as datetime) as FromDate,
	   Case When Todate is null THen Cast(FromDate + ((Select DowntimeOffsetHours from UserData.SequencerConfiguration)/24.0) as datetime) ELSE Todate end as ToDate,
	   '' as ReferenceDate,
	   0 as ReferenceDateType,
	   '' as Efficiency,
	   1 as CostFactor,
	   15 as DatasetID
From Calendar.LmsCalendarPeriods cp
Left Join Calendar.CalendarStates cs on cp.TimeCategory = cs.Name
--Where ResourceId = 2
 ) e

 Update Calendar.LmsCalendarPeriods
 Set Consumed = 1
 --Delete old entries that preactor is no longer concerned with
 DELETE FROM LmsCalendarPeriods
	WHERE ToDate IS NOT NULL AND ToDate < GETDATE() - 5

END





GO


