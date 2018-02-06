USE [PreactorProd]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<B.Levy>
-- Create date: <12/13/2017>
-- Description:	<Adds an entry into LmsCalendarPeriods to denote the start of a downtime>
-- =============================================
CREATE PROCEDURE [Calendar].[InsertLmsCalendarPeriod]
	-- Add the parameters for the stored procedure here
	(
	@ResourceName nvarchar(50),
	@FromDate DateTime,
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
	 INSERT INTO LmsCalendarPeriods (ResourceId, IsException, FromDate, TimeCategory, Level1, Level2, Level3, Level4, Consumed)
	 VALUES (@ResourceId, 0, @FromDate, @TimeCategory, @Level1, @Level2, @Level3, @Level4, 0);
END

GO


