using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppNiZiAPI.Models.Repositories
{
    interface IMealRepository
    {
        Task<Meal> AddMeal(Meal meal);
        Task<bool> DeleteMeal(int patient_id, int meal_id);
        Task<List<Meal>> GetMyMeals(int patient_id);
        Task<Meal> GetMealbyName(string name);
        Task<Meal> PutMeal(Meal meal);
    }
}
