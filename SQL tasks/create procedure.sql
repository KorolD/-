create procedure ClearLocker(@seconds int = 1200)
as
delete writing_locker where DATEDIFF(second, begin_time, GETDATE())>@seconds;