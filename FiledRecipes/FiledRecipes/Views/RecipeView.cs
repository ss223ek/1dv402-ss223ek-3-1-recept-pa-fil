using FiledRecipes.Domain;
using FiledRecipes.App.Mvp;
using FiledRecipes.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiledRecipes.Views
{
    /// <summary>
    /// 
    /// </summary>
    public class RecipeView : ViewBase, IRecipeView
    {
        public void Show(IRecipe recipe) 
        {
            Header = recipe.Name;
            Console.Clear();
            ShowHeaderPanel();

            //----------------------------------- ingredienser från ref-lista i recipe-objekt

            Console.WriteLine("\nIngredienser");
            Console.WriteLine("============");  
            IEnumerable<IIngredient> tempList;             //temorär lista
            tempList = recipe.Ingredients;
            foreach (IIngredient klo in tempList)           // ut med allt från lista, ToString anpassas efter objekt
            {
                if ( klo != null)
                {
                    Console.WriteLine(klo);
                }
                else
                {
                    Console.WriteLine("[null]");
                }
            }

            //----------------------------------- instruktioner från stringlista i recipe-objekt
            Console.WriteLine("\nGör så här");
            Console.WriteLine("==========");
            IEnumerable<string> tempStringList;         //temporär lista
            tempStringList = recipe.Instructions;
            foreach (string klo in tempStringList)      //ut med allt från lista, ToString anpassas till sträng
            {
                if (klo != null)
                {
                    Console.WriteLine(klo);
                }
                else
                {
                    Console.WriteLine("[null]");
                }
            }
            


        }
        public void Show(IEnumerable<IRecipe> recipes) 
        {
            //Console.WriteLine("show med IEnumeable<>");
            foreach (IRecipe klo in recipes)           //
            {
                if (klo != null)
                {
                    Show(klo);
                }
                else
                {
                    Console.WriteLine("[null]");
                }
                ContinueOnKeyPressed();
            } 
   
        }

    }
}
