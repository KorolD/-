select name as "Name", surname as "Surname", sum from enrollees join
(select enrollee_ID,sum(result) as "SUM" from exams_results group by enrollee_ID) tbl
on enrollees.ID=tbl.enrollee_ID where sum between 200 and 240 order by sum desc