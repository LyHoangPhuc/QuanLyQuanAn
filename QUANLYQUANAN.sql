CREATE DATABASE QUANLYQUANAN
GO

USE QUANLYQUANAN
GO
drop Database QUANLYQUANAN
CREATE TABLE TableFood
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100)NOT NULL 	DEFAULT N'Bàn chưa tên',
	status NVARCHAR (100)NOT NULL DEFAULT N'Trống'
)
GO

CREATE TABLE Account
(
	UserName NVARCHAR(100) PRIMARY KEY,
	DisplayName NVARCHAR(100) NOT NULL DEFAULT N'TMPP',
	PassWord NVARCHAR(1000)NOT NULL DEFAULT 0,
	Type INT NOT NULL DEFAULT 0 --(1: MANAGERMENT || 0: EMPLOYEE)
)
GO

CREATE TABLE FoodCategory
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100)NOT NULL DEFAULT N'Chưa có tên'
)
GO

CREATE TABLE Food
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100)NOT NULL DEFAULT N'Chưa có tên',
	idCategory INT NOT NULL,
	price FLOAT NOT NULL

	FOREIGN KEY (idCategory) REFERENCES dbo.foodCategory(id)
)
GO

CREATE TABLE Bill
(
	id INT IDENTITY PRIMARY KEY,
	DateCheckIn DATE NOT NULL DEFAULT GETDATE(),
	DateCheckOut DATE,
	idTable INT NOT NULL,
	status INT NOT NULL DEFAULT 0    --Bill đã tính tiền hay chưa (1: đã thanh toán || 0: Chưa thanh toán)

	FOREIGN KEY (idTable) REFERENCES dbo.TableFood(id)

)
GO

CREATE TABLE BillInfo
(
	id INT IDENTITY PRIMARY KEY,
	idBill INT NOT NULL,
	idFood INT NOT NULL,
	COUNT INT NOT NULL DEFAULT 0

	FOREIGN KEY (idBill) REFERENCES dbo.Bill(id),
	FOREIGN KEY (idFood) REFERENCES dbo.Food(id)
)
GO
-------------------DATA PROVIDER
INSERT INTO dbo.Account (UserName,DisplayName,PassWord,Type)
VALUES
(N'M',N'HOCONGMINH',N'1',1),
(N'T',N'NGUYENTRANBAOTHUONG',N'2',1),
(N'P',N'LYHOANGPHUC',N'3',1),
(N'PP',N'DOTHANHPHAT',N'4',1)

update  dbo.Account set type = 0 where UserName = N'PP'
select *from dbo.Account

select UserName, DisplayName, Type from dbo.Account

CREATE PROC USP_GetAccountByUserName
@userName NVARCHAR(100)
AS
BEGIN
	SELECT*FROM dbo.Account WHERE UserName = @userName
END
GO
EXEC dbo.USP_GetAccountByUserName @userName = N'T'
GO

--
CREATE PROC USP_Login
@userName NVARCHAR(100), @passWord NVARCHAR(100)
AS
BEGIN
	SELECT*FROM dbo.Account WHERE UserName = @userName AND PassWord = @passWord   
END
GO
exec dbo.USP_Login @userName = N'M', @passWord = N'1'
--
--mặc định 10 bàn
DECLARE @i INT = 0 
WHILE @i <= 10
BEGIN 
	INSERT dbo.TableFood (name) VALUES (N'Bàn ' + CAST(@i AS NVARCHAR(100)))	   
	SET @i = @i + 1
END
--
CREATE PROC USP_GetTableList
AS
	SELECT*FROM dbo.TableFood
GO
EXEC dbo.USP_GetTableList
GO

--Thêm CAtegory
INSERT dbo.FoodCategory (name)
VALUES 
(N'Hải sản'),--1
(N'Nông sản'),--2
(N'Lâm sản'),--3
(N'Nước')--4
--thêm món
INSERT dbo.Food (name, idCategory, price)
VALUES
(N'Mực nướng sa tế quá đã', 1, 150000),
(N'Nghêu hấp xối xã', 1, 80000),
(N'Bánh gạo nướng thập cẩm', 2, 200000),
(N'Heo rừng ngũ vị hương', 3, 650000),
(N'Sting', 4, 10000),
(N'7up', 4, 10000)
--thêm bill (dựa theo bàn)
INSERT dbo.Bill(DateCheckIn, DateCheckOut, idTable, status)
VALUES
(GETDATE(),NULL,  1, 0),
(GETDATE(),NULL,  2, 0),
(GETDATE(),GETDATE(),  3, 1)
--THÊM BILLINFO
INSERT dbo.BillInfo (idBill, idFood, count)
VALUES
--bill 1
(1,2,2),
(1,3,4),
(1,5,1),
--Bill 2
(2,6,2),
--Bill3
(3,5,2)
ALTER TABLE dbo.Bill
ADD discount INT

UPDATE dbo.Bill SET discount = 0
GO

Select * from dbo.Bill where idTable = 3 And status = 1
Select * from dbo.BillInfo where idBill = 3

Select * from dbo.BillInfo 
Select * from dbo.Bill
--SELECT f.name, bi.COUNT, f.price, f.price*bi.COUNT AS totalPrice From dbo.BillInfo as bi, dbo.Bill as b, dbo.Food as f where bi.idBill = b.id AND bi.idFood = f.id AND b.idTable = 3
CREATE PROC USP_InsertBill
@idTable INT
AS
BEGIN
	INSERT dbo.Bill(DateCheckIn, DateCheckOut, idTable, status, discount)
	VALUES
	(GETDATE(), NULL,  @idTable, 0, 0)
END 
GO
--
CREATE PROC USP_InsertBillInfo
@idBill INT, @idFood INT, @count INT
AS
BEGIN
	DECLARE @isExitBillInfo INT
	DECLARE @foodCount INT = 1
	SELECT @isExitBillInfo = id, @foodCount = b.count From dbo.BillInfo as b where idBill = @idBill AND idFood = @idFood
	if (@isExitBillInfo > 0)
	BEGIN
		DECLARE @newCount INT = @foodCount + @count
		if(@newCount > 0)
			UPDATE dbo.BillInfo SET count = @foodCount + @count where idFood = @idFood
		else
			DELETE dbo.BillInfo where idBill = @idBill AND idFood = @idFood
	END
	else
	BEGIN
		INSERT dbo.BillInfo (idBill, idFood, count)
		VALUES
		(@idBill, @idFood, @count)
	END 
END
GO
--
DELETE dbo.BillInfo
DELETE dbo.Bill
--
CREATE TRIGGER UTG_UpdateBillInfo
ON dbo.BillInfo For INSERT, UPDATE
AS
BEGIN
	DECLARE @idBill INT
	SELECT @idBill = idBill FROM inserted
	DECLARE @idTable INT 
	select @idTable = idTable from dbo.Bill where id = @idBill AND status = 0

	declare @count int
	select @count = COUNT(*) from dbo.BillInfo where idBill = @idBill
	if (@count > 0 )
		UPDATE dbo.TableFood SET status = N'Có người' where id = @idTable
	else 
		UPDATE dbo.TableFood SET status = N'Trống' where id = @idTable

END
GO
--

--
CREATE TRIGGER UTG_UpdateBill
ON dbo.Bill For UPDATE
AS
BEGIN
	DECLARE @idBill INT
	SELECT @idBill = id FROM Inserted
	DECLARE @idTable INT 
	select @idTable = idTable from dbo.Bill where id = @idBill
	DECLARE @count INT = 0 
	select @count = count(*) from dbo.Bill where idTable = @idTable and status = 0 
	if (@count = 0)
		UPDATE dbo.TableFood SET status = N'Trống' where id = @idTable
END
GO
--

--
CREATE PROC USP_SwitchTable1
@idTable1 int, @idTable2 int
AS BEGIN
	DECLARE @idFirstBill int
	DECLARE @idSeconrdBill int 

	declare @isFirstTableEmpty INT =1
	declare @isSeconrdTableEmpty INT= 1

	SELECT @idSeconrdBill = id FROM dbo.Bill WHERE idTable = @idTable2 AND status = 0
	SELECT @idFirstBill = id FROM dbo.Bill WHERE idTable = @idTable1 AND status = 0

	if (@idFirstBill IS NULL)
	BEGIN
		INSERT dbo.Bill(DateCheckIn, DateCheckOut, idTable, status)
		values (GETDATE(), nULL, @idTable1, 0)
		SELECT @idFirstBill = MAX (	id) FROM dbo.Bill where idTable =  @idTable1 AND status = 0
	END
	select @isFirstTableEmpty = count(*) from dbo.BillInfo where idBill = @idFirstBill
	--
	if (@idSeconrdBill IS NULL)
	BEGIN
		INSERT dbo.Bill(DateCheckIn, DateCheckOut, idTable, status)
		values (GETDATE(), nULL, @idTable2, 0)
		SELECT @idSeconrdBill = MAX (id) FROM dbo.Bill where idTable =  @idTable2 AND status = 0
	END
		select @isSeconrdTableEmpty = count(*) from dbo.BillInfo where idBill = @idSeconrdBill


	SELECT id INTO  IDBillInfoTable FROM dbo.BillInfo WHERE idBill = @idSeconrdBill
	UPDATE dbo.BillInfo SET idBill = @idSeconrdBill WHERE idBill = @idFirstBill
	UPDATE dbo.BillInfo SET idBill = @idFirstBill WHERE id  IN (SELECT*From IDBillInfoTable)
	Drop TABLE IDBillInfoTable

	if (@isFirstTableEmpty = 0)
		update dbo.TableFood SET status = N'Trống' where id = @idTable2
	if (@isSeconrdTableEmpty = 0)
		update dbo.TableFood SET status = N'Trống' where id = @idTable1
END 
Go
--
ALTER TABLE dbo.Bill ADD totalPrice FLOAT
DELETE dbo.BillInfo
DELETE dbo.Bill
GO

CREATE PROC  USP_GetListBillByDate
@checkIn date, @checkOut date
as
Begin
	select t.name as [Tên bàn], b.totalPrice as [Tổng tiền], DateCheckIn as [Ngày vào], DateCheckOut as [Ngày ra], discount as [Giảm giá]
	from dbo.Bill as b, dbo.TableFood as t 
	where  DateCheckIn >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1 AND t.id = b.idTable
END
go
--
Create proc USP_UpdateAccount
@userName Nvarchar(100), @displayName NVARCHAR (100), @password NVARCHAR(100), @newPassword NVARCHAR (100)
as
BEGIN
	declare @isRightPass INT = 0
	select @isRightPass = count(*) from dbo.Account where USERName = @userName and PassWord = @password
	if (@isRightPass = 1)
	Begin 
		if (@newPassword = NULL or @newPassword = '')
		begin 
			update dbo.Account SET DisplayName = @displayName WHERE UserName = @userName 
		end
		else
			update dbo.Account SET DisplayName = @displayName , PassWord = @newPassword where UserName = @userName 
	END
end
GO
--
CREATE TRIGGER UTG_DeleteBillInfo
ON dbo.BillInfo FOR DELETE
as
BEGIN
	declare @idBillInfo INT
	declare @idBill int
	select @idBillInfo = id, @idBill = Deleted.idBill from deleted

	declare @idTable int
	select @idTable = idTable from dbo.Bill where id = @idBill

	declare @count int = 0 

	select @count = count(*) from dbo.BillInfo as bi, dbo.Bill as b where b.id = bi.idBill and b.id	 = @idBill and b.status = 0

	if (@count = 0 )
		update dbo.TableFood set  status = N'Trống' where id = @idTable
END
GO
--
Create FUNCTION [dbo].[fuConvertToUnsign1] ( @strInput NVARCHAR(4000) ) 
RETURNS NVARCHAR(4000) 
AS 
BEGIN 
	IF @strInput IS NULL 
	RETURN @strInput 
		IF @strInput = '' 
		RETURN @strInput 
	DECLARE @RT NVARCHAR(4000) 

	DECLARE @SIGN_CHARS NCHAR(136) 

	DECLARE @UNSIGN_CHARS NCHAR (136) 
		SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' +NCHAR(272)+ NCHAR(208) SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD' DECLARE @COUNTER int DECLARE @COUNTER1 int SET @COUNTER = 1 WHILE (@COUNTER <=LEN(@strInput)) BEGIN SET @COUNTER1 = 1 WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1) BEGIN IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) ) BEGIN IF @COUNTER=1 SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1) ELSE SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER) BREAK END SET @COUNTER1 = @COUNTER1 +1 END SET @COUNTER = @COUNTER +1 END SET @strInput = replace(@strInput,' ','-') RETURN @strInput END
--
create PROC  USP_GetListBillByDateAndPage
@checkIn date, @checkOut date, @page int
as
Begin
	declare @pageRows int = 10
	declare @selectRows int = @pageRows
	declare @exceptRows int = (@page - 1)*@pageRows
	--tao bảng tạm và dấu ";" phải đầu câu lệnh 
	;WITH BillShow as (select b.ID, t.name as [Tên bàn], b.totalPrice as [Tổng tiền], DateCheckIn as [Ngày vào], DateCheckOut as [Ngày ra], discount as [Giảm giá]
	from dbo.Bill as b, dbo.TableFood as t 
	where  DateCheckIn >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1 AND t.id = b.idTable)

	select top (@selectRows) * from BillShow where id  not in (select top (@exceptRows) id from BillShow)

END
go
--
CREATE PROC  USP_GetNumBillByDate
@checkIn date, @checkOut date
as
Begin
	select count(*)
	from dbo.Bill as b, dbo.TableFood as t 
	where  DateCheckIn >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1 AND t.id = b.idTable
END
go
--
CREATE PROC  USP_GetListBillByDateForReport
@checkIn date, @checkOut date
as
Begin
	select t.name, b.totalPrice , DateCheckIn, DateCheckOut, discount
	from dbo.Bill as b, dbo.TableFood as t 
	where  DateCheckIn >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1 AND t.id = b.idTable
END
go