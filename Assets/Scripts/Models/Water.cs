using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water
{

    int amount = 0;

   public Water(int amount)
   {
        this.amount = amount;
   }


   public void setAmount(int amount)
   {
        this.amount = amount;
   }

    public int getAmount()
    {
        return amount;
    }

    public bool subtractWater(int subAmount)
    {
        amount -= subAmount;

        if(amount <= 0)
        {
            return false;
        }

        return true;
    }

    public void addAmount(int add)
    {
        amount = Mathf.Clamp(add + amount, 0, 100);
    }
}
