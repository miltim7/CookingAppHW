create database CookingAppDB;

create table Recipe (
    Id primary key identity,
    Title nvarchar(50) not null,
    [Description] nvarchar(max) not null,
    Category nvarchar(50) not null,
    Price int not null,
)