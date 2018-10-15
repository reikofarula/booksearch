USE OPENXMLTesting
GO


DECLARE @XML AS XML, @hDoc AS INT, @SQL NVARCHAR (MAX)


SELECT @XML = XMLData FROM XMLwithOpenXML


EXEC sp_xml_preparedocument @hDoc OUTPUT, @XML

DROP TABLE IF EXISTS Book 
CREATE TABLE Book
(
Id VARCHAR(3),
Author VARCHAR(50),
Title VARCHAR(200),
Genere VARCHAR(50),
Price DECIMAL(19,6),
PublishDate VARCHAR(10),
Description VARCHAR(500),
PRIMARY KEY (Id)	
)

INSERT INTO dbo.Book
(
    Id,
    Author,
    Title,
    Genere,
    Price,
    PublishDate,
    [Description]

)
SELECT Id, Author, Title, Genere, Price,PublishDate, [Description]
FROM OPENXML(@hDoc, 'catalog/book')
WITH 
(
Id VARCHAR(3) '@id',
Author VARCHAR(50) 'author',
Title VARCHAR(200) 'title',
Genere VARCHAR(50) 'genre',
Price DECIMAL(19,6) 'price',
PublishDate VARCHAR(10) 'publish_date',
Description VARCHAR(500) 'description'

)

SELECT * FROM Book 

EXEC sp_xml_removedocument @hDoc
GO