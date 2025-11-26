----------------------------------------------------------- TẠO DATABASE---------------------------------------------------------
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'HR_DATABASE_NET')
BEGIN
    CREATE DATABASE HR_DATABASE_NET;
END;
GO

USE HR_DATABASE_NET;
GO

----------------------------------------------------------- BẢNG PHÒNG BAN---------------------------------------------------------
IF NOT EXISTS (
    SELECT * FROM sysobjects WHERE name='departments' AND xtype='U'
)
BEGIN
    CREATE TABLE departments (
        dept_id VARCHAR(10) PRIMARY KEY,		-- mã phòng
        dept_name NVARCHAR(100) NOT NULL,		-- tên phòng
		description NVARCHAR(200),				-- mô tả
    );
END;
GO


----------------------------------------------------------- BẢNG NHÂN VIÊN---------------------------------------------------------
IF NOT EXISTS (
    SELECT * FROM sysobjects WHERE name='employees' AND xtype='U'
)
BEGIN
    CREATE TABLE employees (
        id VARCHAR(10) PRIMARY KEY,			-- mã nhân viên
        fullname NVARCHAR(100),				-- tên nhân viên 
		gender NVARCHAR(10),				-- giới tính 
		birthday DATE,						-- ngày sinh
        phone VARCHAR(10),					-- số điện thoại 
        address NVARCHAR(200),				-- địa chỉ 
        dept_id VARCHAR(10),				-- mã phòng ban 
        position NVARCHAR(100),				-- chức vụ 
		salarycoefficient DECIMAL(5,2)		-- hệ số lương
        FOREIGN KEY (dept_id) REFERENCES departments(dept_id)
	);
END;
GO

----------------------------------------------------------- BẢNG LƯƠNG---------------------------------------------------------
IF NOT EXISTS (
    SELECT * FROM sysobjects WHERE name='salaries' AND xtype='U'
)
BEGIN
    CREATE TABLE salaries (
        slr_id INT IDENTITY(1,1) PRIMARY KEY,	-- mã lương 
		emp_id VARCHAR(10),						-- mã nhân viên 
		month TINYINT,							-- tháng
		year SMALLINT,							-- năm
		workingdays INT,						-- số ngày công 
		totalsalary DECIMAL(18,2),				-- tổng lương
        FOREIGN KEY (emp_id) REFERENCES employees(id)
    );
END;
GO

----------------------------------------------------------- BẢNG USER---------------------------------------------------------
IF NOT EXISTS (
    SELECT * FROM sysobjects WHERE name='users' AND xtype='U'
)
BEGIN
    CREATE TABLE users  (
        emp_id VARCHAR(10) PRIMARY KEY,			-- mã nhân viên  
		username VARCHAR(20),					-- mã user 
		password varchar(200),					-- mật khẩu
		role nvarchar(20)						-- admin/user
		FOREIGN KEY (emp_id) REFERENCES employees(id)
    );
END;
GO


----------------------------------------------------------- BẢNG CHẤM CÔNG ---------------------------------------------------------
IF NOT EXISTS (
    SELECT * FROM sysobjects WHERE name='attendance' AND xtype='U'
)
BEGIN
    CREATE TABLE attendance (
		atd_id INT IDENTITY(1,1) PRIMARY KEY,	-- mã chấm công 
		emp_id VARCHAR(10),						-- mã nhân viên
date DATE,								-- ngày chấm công
		checkin TIME,							-- giờ vào làm
		checkout TIME,							-- giờ tan ca 
		note NVARCHAR(200),						-- Ghi chú
		rating NVARCHAR(10),					-- đánh giá(từ A-D)
        FOREIGN KEY (emp_id) REFERENCES employees(id)
    );
END;
GO

INSERT INTO departments (dept_id, dept_name, description)
VALUES
(N'NS', N'Phòng Nhân Sự', N'Quản lý nhân sự'),
(N'KT', N'Phòng Kế Toán', N'Quản lý tài chính'),
(N'IT', N'Phòng IT', N'Hệ thống và phần mềm'),
(N'KD', N'Phòng Kinh Doanh', N'Bán hàng & CSKH');

INSERT INTO employees (id, fullname, gender, birthday, phone, address, dept_id, position, salarycoefficient)
VALUES
(N'NV001', N'Nguyễn Văn A', N'Nam', '1990-03-15', '0912345678', N'Hà Nội', N'NS', N'Chuyên viên', 2.50),
(N'NV002', N'Trần Thị B', N'Nữ', '1995-07-20', '0987654321', N'Hồ Chí Minh', N'KT', N'Kế toán viên', 2.20),
(N'NV003', N'Phạm Văn C', N'Nam', '1988-11-05', '0933334444', N'Hải Phòng', N'IT', N'Lập trình viên', 3.00),
(N'NV004', N'Lê Thị D', N'Nữ', '1992-02-10', '0944445555', N'Đà Nẵng', N'KD', N'Nhân viên kinh doanh', 2.00),
(N'NV005', N'Hoàng Văn E', N'Nam', '1998-09-09', '0909090909', N'Quảng Ninh', N'IT', N'Tester', 1.80);

INSERT INTO attendance (emp_id, date, checkin, checkout, note, rating)
VALUES
(N'NV001', '2025-01-02', '08:00', '17:00', N'Đi làm đúng giờ', N'A'),
(N'NV002', '2025-01-02', '08:15', '17:10', N'Đi muộn 15 phút', N'B'),
(N'NV003', '2025-01-02', '07:55', '17:05', N'Làm thêm 5 phút', N'A'),
(N'NV004', '2025-01-02', '08:30', '17:00', N'Đi muộn 30 phút', N'C'),
(N'NV005', '2025-01-02', '08:05', '17:00', N'Ổn định', N'B');

INSERT INTO salaries (emp_id, month, year, workingdays, totalsalary)
VALUES
(N'NV001', 1, 2025, 26, 15000000),
(N'NV002', 1, 2025, 25, 12000000),
(N'NV003', 1, 2025, 27, 20000000),
(N'NV004', 1, 2025, 24, 10000000),
(N'NV005', 1, 2025, 26, 13000000);

INSERT INTO users (emp_id, username, password, role)
VALUES
(N'NV001', 'admin', 'admin123', 'admin'),
(N'NV002', 'user1', '123456', 'user'),
(N'NV003', 'user2', '123', 'user');