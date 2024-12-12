create table employees
(
employeeid int identity primary key,
[name] nvarchar(30) not null,
[address] nvarchar(30) not null,
salary decimal not null,
joiningdate date not null,
picture nvarchar(40) not null,
isacurrentemployee bit
)
go
create table qualification
(
id int identity primary key,
degree nvarchar(30) not null,
institute nvarchar(40) not null,
passingyear int not null,
result nvarchar(30) not null,
employeeid int not null references employees(employeeid)
)
go