create database CookingAppDB;

create table Recipes 
(
    [Id] int primary key identity,
    [Title] nvarchar(50) not null,
    [Description] nvarchar(max) not null,
    [Category] nvarchar(50) not null,
    [Price] int not null,
    [UserId] nvarchar(450) foreign key references AspNetUsers([Id])
)

create table Logs
(
    [Id] int primary key identity,
    [UserId] nvarchar(450),
    [Url] nvarchar(max),
    [MethodType] nvarchar(max),
    [StatusCode] int,
    [RequestBody] nvarchar(max),
    [ResponseBody] nvarchar(max)
)