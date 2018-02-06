USE [PreactorProd]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<B.Levy>
-- Create date: <12/15/2017>
-- Description:	<Update all entries in LmsCalendarPeriods that do not have an end date to have one>
-- =============================================
CREATE PROCEDURE [Calendar].[FinalizeLmsCalendarPeriods] 
	(
	@ToDate DateTime
	)
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE LmsCalendarPeriods
	SET ToDate = @ToDate, Consumed = 0
	WHERE ToDate IS NULL;
END


GO


