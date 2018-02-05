drop table parents;
create table parents(
enrollee_ID int not null,
name nvarchar(50) not null,
surname nvarchar(50) not null,
patronymic nvarchar(50)not null,
birthdate date,
primary key (enrollee_ID),
foreign key (enrollee_ID) references enrollees (ID)
);