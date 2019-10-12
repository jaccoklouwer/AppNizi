Drop table MyFood

CREATE TABLE MyFood(
    food_id int not null Foreign Key References Food(id),
	patient_id int not null Foreign Key References Patient(id)
);