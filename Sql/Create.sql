create database CookingAppDB;

create table Recipes 
(
    [Id] int primary key identity,
    [Title] nvarchar(50) not null,
    [Description] nvarchar(max) not null,
    [Category] nvarchar(50) not null,
    [Price] int not null,
    [UserId] int foreign key references Users([Id])
)

create table Logs
(
    [Id] int primary key identity,
    [UserId] int,
    [Url] nvarchar(max),
    [MethodType] nvarchar(max),
    [StatusCode] int,
    [RequestBody] nvarchar(max),
    [ResponseBody] nvarchar(max)
)

create table Users
(
    [Id] int primary key identity,
    [Name] nvarchar(100) not null,
    [Surname] nvarchar(100) not null,
    [Login] nvarchar(100) not null unique,
    [Password] nvarchar(100) not null,
)