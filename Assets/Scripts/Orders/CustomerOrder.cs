using System.Collections.Generic;
using UnityEngine;
using YesChef.Ingredients;

namespace YesChef.Orders
{
    public class CustomerOrder
    {
        public List<IngredientData> RequiredIngredients { get; private set; }
        public List<IngredientData> FulfilledIngredients { get; private set; }
        public float CreatedTimestamp { get; private set; }

        public float ElapsedSeconds => Time.time - CreatedTimestamp;
        public bool IsComplete => FulfilledIngredients.Count >= RequiredIngredients.Count;

        public CustomerOrder(List<IngredientData> ingredients)
        {
            RequiredIngredients = new List<IngredientData>(ingredients);
            FulfilledIngredients = new List<IngredientData>();
            CreatedTimestamp = Time.time;
        }

        public bool TryFulfill(IngredientInstance instance)
        {
            if (instance == null || !instance.IsPrepared) return false;

            // Find an unfulfilled required ingredient match
            for (int i = 0; i < RequiredIngredients.Count; i++)
            {
                IngredientData req = RequiredIngredients[i];
                if (req.Type == instance.Data.Type)
                {
                    // Count how many of this type are required vs already delivered
                    int neededCount = 0;
                    int deliveredCount = 0;

                    foreach (var r in RequiredIngredients) if (r.Type == req.Type) neededCount++;
                    foreach (var f in FulfilledIngredients) if (f.Type == req.Type) deliveredCount++;

                    if (deliveredCount < neededCount)
                    {
                        FulfilledIngredients.Add(req);
                        return true;
                    }
                }
            }

            return false;
        }

        public int CalculateFinalScore()
        {
            int baseSum = 0;
            foreach (var ing in FulfilledIngredients)
            {
                baseSum += ing.BaseScore;
            }

            int elapsedRoundedDown = Mathf.FloorToInt(ElapsedSeconds);
            return baseSum - elapsedRoundedDown;
        }
    }
}