insert into enrollees(name,surname,patronymic,town_ID,street,house,birthdate,passport) 
values ('name','sadasd','asdasd',1,'sdfsdf',5,'2000.01.01','rtfdbdb1'),
('name','sadasd','asdasd',1,'sdfsdf',5,'2000.01.01','rtfdbdb2'),
('name','sadasd','asdasd',1,'sdfsdf',5,'2000.01.01','rtfdbdb3');
update enrollees set surname='another', town_ID=2 where name='name'