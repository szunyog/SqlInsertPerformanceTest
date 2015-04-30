

CREATE TABLE [dbo].[CurveData](
	[CurveId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[Value] [decimal](23, 6) NOT NULL,
 CONSTRAINT [PK_dbo.CurveDatas] PRIMARY KEY CLUSTERED 
(
	[CurveId] ASC,
	[TimeStamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TYPE dbo.CurveDataType
AS TABLE
(
    [CurveId] int,
	[TimeStamp] datetime,
	[Value] decimal(23, 6)
);

GO

CREATE PROCEDURE dbo.InsertCurveDataList
  @List AS dbo.CurveDataType READONLY
AS
BEGIN
  SET NOCOUNT ON;

  INSERT INTO dbo.CurveData ([CurveId], [TimeStamp], [Value])
  SELECT [CurveId], [TimeStamp], [Value] FROM @List ; 
END

go

CREATE PROCEDURE dbo.InsertCurveData
    @CurveId int,
	@TimeStamp datetime,
	@Value decimal(23, 6)
AS
BEGIN
  SET NOCOUNT ON;

  INSERT INTO dbo.CurveData ([CurveId], [TimeStamp], [Value])
  VALUES(@CurveId, @TimeStamp, @Value)
END

GO



CREATE PROCEDURE dbo.InsertCurveDataXml
    @data xml
AS
BEGIN
  SET NOCOUNT ON;

  INSERT INTO dbo.CurveData ([CurveId], [TimeStamp], [Value])
  SELECT DISTINCT
    DataToInsert.data.value('(CurveId/text())[1]','int') as [CurveId],
    DataToInsert.data.value('(TimeStamp/text())[1]','datetime') as [TimeStamp],
	DataToInsert.data.value('(Value/text())[1]','decimal') as [Value]
    FROM @data.nodes('/DocumentElement/data') DataToInsert(data)
END
