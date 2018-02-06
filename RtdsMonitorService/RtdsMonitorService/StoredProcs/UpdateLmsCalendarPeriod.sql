USE [PreactorProd]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<B.Levy>
-- Create date: <12/13/2017>
-- Description:	<Updates a given entry in LmsCalendarPeriods to have a ToDate to end a downtime>
-- =============================================
CREATE PROCEDURE [Calendar].[UpdateLmsCalendarPeriod]
	(
	@ResourceName nvarchar(50),
	@ToDate DateTime,
	@TimeCategory nvarchar(50),
	@Level1 nvarchar(50),
	@Level2 nvarchar(50),
	@Level3 nvarchar(50),
	@Level4 nvarchar(50)
	)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @ResourceId int;
	SET @ResourceId = (SELECT Calendar.PrimaryResources.Id
	 from Calendar.PrimaryResources
	 where Calendar.PrimaryResources.Name = @ResourceName);
	UPDATE LmsCalendarPeriods 
	SET ToDate = @ToDate, Consumed = 0
	WHERE ResourceId = @ResourceId AND ToDate IS NULL AND TimeCategory = @TimeCategory AND Level1 = @Level1 AND Level2 = @Level2 AND Level3 = @Level3 AND Level4 = @Level4

END

GO

