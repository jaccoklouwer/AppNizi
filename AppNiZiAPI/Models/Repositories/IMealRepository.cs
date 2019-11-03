using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    interface IMealRepository
    {
        Meal AddMeal(Meal meal);
        bool DeleteMeal(int patient_id, int meal_id);
        List<Meal> GetMyMeals(int patient_id);

        Meal GetMealbyId(int id);
    }
}
