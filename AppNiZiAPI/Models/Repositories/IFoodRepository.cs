﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    interface IFoodRepository
    {
        Food Select(string foodName);
        List<Food> Search(string foodName);
        List<Food> Favorites(int patient_id);
        bool Favorite(int patient_id,int food_id);
    }
}
