using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FiledRecipes.Domain
{
    /// <summary>
    /// Holder for recipes.
    /// </summary>
    public class RecipeRepository : IRecipeRepository
    {

        public void Load()
        {
            //Skapa lista med referenser till receptobjekt, börja med fyra
            List<Recipe> RecipeList = new List<Recipe>(4);

            try
            {
                // using stänger automatiskt filen efter användning
                using (StreamReader reader = new StreamReader("Recipes.txt"))
                {
                    string line;
                    RecipeReadStatus RRStatus = RecipeReadStatus.Indefinite;

                    // läser tills det inte finns rader att läsa
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!line.Equals(""))       //om innehåll i raden så undersöker vi, annars läs nästa
                        {
                            if (line.Equals("[Recept]"))     //nytt recept på gång i nästa rad
                            {
                                RRStatus = RecipeReadStatus.New;
                            }
                            else if (line.Equals("[Ingredienser]"))  //nya ingrediens i nästa rad
                            {
                                RRStatus = RecipeReadStatus.Ingredient;

                            }
                            else if (line.Equals("[Instruktioner]")) //nya instruktioner i nästa rad
                            {
                                RRStatus = RecipeReadStatus.Instruction;

                            }

                            else     // här är det klart att nästa rad är namn/ingrediens eller instruktion
                            {
                                switch (RRStatus)
                                {
                                    case RecipeReadStatus.New: // Skapa nytt receptobjekt, lägg i listan
                                        Recipe temp = new Recipe(line);
                                        RecipeList.Add(temp);
                                        break;

                                    case RecipeReadStatus.Ingredient: // Raden är en ingrediens och ska delas upp

                                        // Delar upp en sträng till tre delsträngar...om det går...
                                        // lägg sedan in i temporär ref
                                        // lägg in ref i receptets lista med ingredienser (det sista i listan)
                                        try
                                        {
                                            string[] iParts = line.Split(new Char[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                            Ingredient tempIngred = new Ingredient();
                                            tempIngred.Amount = iParts[0];
                                            tempIngred.Measure = iParts[1];
                                            tempIngred.Name = iParts[2];
                                            RecipeList[RecipeList.Count - 1].Add(tempIngred);
                                        }

                                        catch
                                        {
                                            Console.WriteLine("illa, inte tre delar i ingrediens!");
                                        }




                                        break;

                                    case RecipeReadStatus.Instruction:                      // Lägg till raden till receptets lista med instruktioner
                                        RecipeList[RecipeList.Count - 1].Add(line);         //lägg det i det listans sista recept (count-1)
                                        //parameter line är en string vilket anropar rätt metod i Recipe.cs
                                        break;


                                    default: break;
                                }








                            }



                        }



                        Console.WriteLine(line);
  

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("feeel!");
            }
        }



        public void Save()
        {
            //tom
        }


        /// <summary>
        /// Represents the recipe section.
        /// </summary>
        private const string SectionRecipe = "[Recept]";

        /// <summary>
        /// Represents the ingredients section.
        /// </summary>
        private const string SectionIngredients = "[Ingredienser]";

        /// <summary>
        /// Represents the instructions section.
        /// </summary>
        private const string SectionInstructions = "[Instruktioner]";

        /// <summary>
        /// Occurs after changes to the underlying collection of recipes.
        /// </summary>
        public event EventHandler RecipesChangedEvent;

        /// <summary>
        /// Specifies how the next line read from the file will be interpreted.
        /// </summary>
        private enum RecipeReadStatus { Indefinite, New, Ingredient, Instruction };

        /// <summary>
        /// Collection of recipes.
        /// </summary>
        private List<IRecipe> _recipes;

        /// <summary>
        /// The fully qualified path and name of the file with recipes.
        /// </summary>
        private string _path;

        /// <summary>
        /// Indicates whether the collection of recipes has been modified since it was last saved.
        /// </summary>
        public bool IsModified { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the RecipeRepository class.
        /// </summary>
        /// <param name="path">The path and name of the file with recipes.</param>
        public RecipeRepository(string path)
        {
            // Throws an exception if the path is invalid.
            _path = Path.GetFullPath(path);

            _recipes = new List<IRecipe>();
        }

        /// <summary>
        /// Returns a collection of recipes.
        /// </summary>
        /// <returns>A IEnumerable&lt;Recipe&gt; containing all the recipes.</returns>
        public virtual IEnumerable<IRecipe> GetAll()
        {
            // Deep copy the objects to avoid privacy leaks.
            return _recipes.Select(r => (IRecipe)r.Clone());
        }

        /// <summary>
        /// Returns a recipe.
        /// </summary>
        /// <param name="index">The zero-based index of the recipe to get.</param>
        /// <returns>The recipe at the specified index.</returns>
        public virtual IRecipe GetAt(int index)
        {
            // Deep copy the object to avoid privacy leak.
            return (IRecipe)_recipes[index].Clone();
        }

        /// <summary>
        /// Deletes a recipe.
        /// </summary>
        /// <param name="recipe">The recipe to delete. The value can be null.</param>
        public virtual void Delete(IRecipe recipe)
        {
            // If it's a copy of a recipe...
            if (!_recipes.Contains(recipe))
            {
                // ...try to find the original!
                recipe = _recipes.Find(r => r.Equals(recipe));
            }
            _recipes.Remove(recipe);
            IsModified = true;
            OnRecipesChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Deletes a recipe.
        /// </summary>
        /// <param name="index">The zero-based index of the recipe to delete.</param>
        public virtual void Delete(int index)
        {
            Delete(_recipes[index]);
        }

        /// <summary>
        /// Raises the RecipesChanged event.
        /// </summary>
        /// <param name="e">The EventArgs that contains the event data.</param>
        protected virtual void OnRecipesChanged(EventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            EventHandler handler = RecipesChangedEvent;

            // Event will be null if there are no subscribers. 
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }
    }
}
