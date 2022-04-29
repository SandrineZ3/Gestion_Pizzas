using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gestion_Pizzas.Models;

namespace Gestion_Pizzas.Controllers
{
    public class PizzaController : Controller
    {
        // propriétés "static" pour simuler la persistence des données dans le controller
        public static List<Pizza> pizzas;
        public static List<Ingredient> ingredients;
        public static List<Pate> pates;

        public PizzaController()
        {
            if (pizzas == null)
            {
                pizzas = new List<Pizza>();
            }
            if (ingredients == null)
            {
                ingredients = Pizza.IngredientsDisponibles;
            }
            if (pates == null)
            {
                pates = Pizza.PatesDisponibles;
            }
        }


        // ViewModel de la Pizza
        // id = id de la pizza à modifier
        private PizzaCreateEditVM GetPizzaCreateEditVM(int id)
        {
            // Nous avons beoin d'un viewModel pour porter l'objet pizza à modifier, et la liste de tous les ingrédients et des pâtes
            var pizzaVM = new PizzaCreateEditVM();

            // On récupère la pizza portant l'Id désiré dans la liste des pizzas portée par le controller
            pizzaVM.Pizza = pizzas.FirstOrDefault(p => p.Id == id);

            // On créé l'objet attendu par la méthode ListBoxFor du HtmlHelper, qui nous permettra de choisir plusieurs ingrédients
            pizzaVM.Ingredients = ingredients.Select(i => new SelectListItem { Text = i.Nom, Value = i.Id.ToString() }).ToList();

            // On créé l'objet attendu par la méthode DropDownListFor du HtmlHelper, qui nous permettra de choisir une pâte
            pizzaVM.Pates = pates.Select(i => new SelectListItem { Text = i.Nom, Value = i.Id.ToString() }).ToList();

            // Si la pizza avait déjà une pâte, elle sera selectionnée sur la vue de modification
            if (pizzaVM.Pizza.Pate != null)
            {
                pizzaVM.IdSelectedPate = pizzaVM.Pizza.Pate.Id;
            }

            // Si la pizza avait déjà des ingrédients, ils seront selectionnés sur la vue de modification
            if (pizzaVM.Pizza.Ingredients.Any())
            {
                pizzaVM.IdSelectedIngredients = pizzaVM.Pizza.Ingredients.Select(i => i.Id).ToList();
            }
            return pizzaVM;
        }


        // GET: Default
        public ActionResult Index()
        {
            // Retourne la liste des pizzas
            return View(pizzas);
        }

        // GET: Default/Details/5
        public ActionResult Details(int id)
        {
            // On récupère la pizza dont l'id a été selectionné
            var pizza = pizzas.FirstOrDefault(p => p.Id == id);
            return View(pizza);
        }

        // GET: Default/Create
        public ActionResult Create()
        {
            var pizzaVM = new PizzaCreateEditVM();
            pizzaVM.Ingredients = ingredients.Select(i => new SelectListItem { Text = i.Nom, Value = i.Id.ToString() }).ToList();
            pizzaVM.Pates = pates.Select(i => new SelectListItem { Text = i.Nom, Value = i.Id.ToString() }).ToList();
            return View(pizzaVM);
        }

        // POST: Default/Create
        [HttpPost]
        public ActionResult Create(PizzaCreateEditVM pizzaCreateEditVM)
        {
            if(ModelState.IsValid) {

                    var pizza = pizzaCreateEditVM.Pizza;

                    // On récupère les objets ingrédients de la liste ingredientsDisponibles du controller à partir des ids choisis portés par le ViewModel, puis on les affecte à notre objet pizza
                    pizza.Ingredients = ingredients.Where(i => pizzaCreateEditVM.IdSelectedIngredients.Contains(i.Id)).ToList();

                    // On récupère l'objet pâte de la liste patesDisponibles du controller à partir de l'id porté par le ViewModel, puis on l'affecte à notre objet pizza
                    pizza.Pate = pates.FirstOrDefault(p => p.Id == pizzaCreateEditVM.IdSelectedPate);

                    // on affecte l'Id à partir de l'id max de notre liste de pizzas plus un.
                    // si notre liste est vide, on affecte la valeur 1.
                    pizza.Id = pizzas.Any() ? pizzas.Max(p => p.Id) + 1 : 1;

                    // On ajoute la nouvelle pizza à notre liste statique
                    pizzas.Add(pizza);
                    return RedirectToAction("Index");
                }
                else 
                {

                    // Si nous avons rencontré une erreur, il faut recharger la page de création, de ce fait il nous faut réalimenter le Viewmodel avant de le passer à la vue
                    pizzaCreateEditVM.Ingredients = ingredients.Select(i => new SelectListItem { Text = i.Nom, Value = i.Id.ToString() }).ToList();
                    pizzaCreateEditVM.Pates = pates.Select(i => new SelectListItem { Text = i.Nom, Value = i.Id.ToString() }).ToList();
                    return View(pizzaCreateEditVM);
                }
            
            
        }

        // GET: Default/Edit/5
        public ActionResult Edit(int id)
        {
            // On affiche la pizza concernée par l'édition (récupération par l'id)
            return View(GetPizzaCreateEditVM(id));
        }

        // TODO a revoir
        // POST: Default/Edit/5
        [HttpPost]
        public ActionResult Edit(PizzaCreateEditVM pizzaCreateEditVM)
        {
            try
            {
                var pizza = pizzas.FirstOrDefault(p => p.Id == pizzaCreateEditVM.Pizza.Id);
                pizza.Nom = pizzaCreateEditVM.Pizza.Nom;
                pizza.Ingredients = ingredients.Where(i => pizzaCreateEditVM.IdSelectedIngredients.Contains(i.Id)).ToList();
                pizza.Pate = pates.FirstOrDefault(p => p.Id == pizzaCreateEditVM.IdSelectedPate);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return View(GetPizzaCreateEditVM(pizzaCreateEditVM.Pizza.Id));
            }
        }



        // GET: Default/Delete/5
        public ActionResult Delete(int id)
        {
            var pizza = pizzas.FirstOrDefault(p => p.Id == id);
            return View(pizza);
        }

        // POST: Default/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {

            var pizza = pizzas.FirstOrDefault(p => p.Id == id);
            pizzas.Remove(pizza);
            return RedirectToAction("Index");
        }
    }
}
