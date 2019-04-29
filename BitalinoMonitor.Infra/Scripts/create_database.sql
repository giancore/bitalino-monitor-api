CREATE TABLE [Customer]
(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	[FirstName] VARCHAR(40) NOT NULL,
	[LastName] VARCHAR(40) NOT NULL,
	[Document] CHAR(11) NOT NULL,
	[Email] VARCHAR(160) NOT NULL,
	[Phone] VARCHAR(13) NOT NULL
)

CREATE TABLE [Address]
(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	[CustomerId] UNIQUEIDENTIFIER NOT NULL,
	[Number] VARCHAR(10) NOT NULL,
	[Complement] VARCHAR(40) NOT NULL,
	[District] VARCHAR(60) NOT NULL,
	[City] VARCHAR(60) NOT NULL,
	[State] CHAR(2) NOT NULL,
	[Country] CHAR(2) NOT NULL,
	[ZipCode] CHAR(8) NOT NULL,
	[Type] INT NOT NULL DEFAULT(1),
	FOREIGN KEY ([CustomerId]) REFERENCES [Customer]([Id])
)

CREATE TABLE [Product]
(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	[Title] VARCHAR(255) NOT NULL,
	[Description] TEXT NOT NULL,
	[Image] VARCHAR(1024) NOT NULL,
	[Price] MONEY NOT NULL,
	[QuantityOnHand] DECIMAL(10, 2) NOT NULL,
)

CREATE TABLE [Order]
(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	[CustomerId] UNIQUEIDENTIFIER NOT NULL,
	[CreateDate] DATETIME NOT NULL DEFAULT(GETDATE()),
	[Status] INT NOT NULL DEFAULT(1),
	FOREIGN KEY([CustomerId]) REFERENCES [Customer]([Id])
)

CREATE TABLE [OrderItem] (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	[OrderId] UNIQUEIDENTIFIER NOT NULL,
	[ProductId] UNIQUEIDENTIFIER NOT NULL,
	[Quantity] DECIMAL(10, 2) NOT NULL,
	[Price] MONEY NOT NULL,
	FOREIGN KEY([OrderId]) REFERENCES [Order]([Id]),
	FOREIGN KEY([ProductId]) REFERENCES [Product]([Id])
)

CREATE TABLE [Delivery] (
	[Id] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	[OrderId] UNIQUEIDENTIFIER NOT NULL,
	[CreateDate] DATETIME NOT NULL DEFAULT(GETDATE()),
	[EstimatedDeliveryDate]  DATETIME NOT NULL,
	[Status] INT NOT NULL DEFAULT(1),
	FOREIGN KEY([OrderId]) REFERENCES [Order]([Id])
)


-----------------------------------------------------------

CREATE TABLE [Patient]
(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	[Name] VARCHAR(180) NOT NULL,
	[Phone] VARCHAR(13) NOT NULL,
    [PhotoPath] VARCHAR(180) NULL,
    [DateOfBirth] DATE NOT NULL,
    [CreateDate] DATETIME NOT NULL DEFAULT(GETDATE()),
)

CREATE TABLE [Exam]
(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    [IdPatient] UNIQUEIDENTIFIER NOT NULL,
	[Type] INT NOT NULL DEFAULT(1),
	[Date] DATETIME NOT NULL DEFAULT(GETDATE()),
    [Channel] INT NOT NULL,
    FOREIGN KEY ([IdPatient]) REFERENCES [Patient]([Id])
)

CREATE TABLE [BitalinoFrame]
(
	[Id] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    [IdExam] UNIQUEIDENTIFIER NOT NULL,
	[Identifier] VARCHAR(20) NOT NULL,
    [Seq] INT NOT NULL,
    [A0] INT NOT NULL,
    [A1] INT NOT NULL,
    [A2] INT NOT NULL,
    [A3] INT NOT NULL,
    [A4] INT NOT NULL,
    [A5] INT NOT NULL,
    [D0] INT NOT NULL,
    [D1] INT NOT NULL,
    [D2] INT NOT NULL,
    [D3] INT NOT NULL,
    FOREIGN KEY ([IdExam]) REFERENCES [Exam]([Id])
)