 drop table food
 
 CREATE TABLE Food(
    id int Identity(1,1) PRIMARY KEY,
    name varchar(128) not null,
    kcal float not null,
	protein float not null,
	fiber float not null,
	calcium float not null,
	sodium float not null,
	portion_size float not null,
	weight_unit_id int not null Foreign Key References dbo.WeightUnit(id)
);