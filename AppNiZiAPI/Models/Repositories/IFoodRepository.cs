using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppNiZiAPI.Models.Repositories
{
    public interface IFoodRepository
    {
        Task<Food> SelectAsync(int food_id);
        Task<List<Food>> Search(string foodName, int count);
        Task<List<Food>> Favorites(int patient_id);
        Task<bool> Favorite(int patient_id,int food_id);
        Task<bool> UnFavorite(int patient_id, int food_id);
        Task<bool> Delete(int patientId);
    }
}
