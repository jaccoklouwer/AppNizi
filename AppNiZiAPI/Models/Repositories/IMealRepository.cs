using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    interface IMealRepository
    {
        bool AddMeal(Meal meal);
        bool DeleteMeal(int patient_id, int meal_id);
        List<Meal> GetMyMeals(int patient_id);

    }
}
