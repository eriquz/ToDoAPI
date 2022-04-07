using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.API.Models;
using ToDoAPI.DATA.EF;
using System.Web.Http.Cors;

namespace ToDoAPI.API.Controllers
{
    //GET = READ
    //POST = CREATE
    //PUT = EDIT
    //DELETE = DELETE
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CategoriesController : ApiController
    {
        ToDoEntities db = new ToDoEntities();

        //GET - /api/Categories
        public IHttpActionResult GetCats()
        {
            //Below we create a list of EF Category objects. In an API, it's best practice to install EF to the API layer when needing to accomplish this task.
            List<CategoryViewModel> cats = db.Categories.Include("Category").Select(c => new CategoryViewModel()
            {
                //Assign the columns of the Resources db table to the ResourceViewModel object, so we can use the data (send the data back to requesting app)
                CategoryID = c.CategoryID,
                Name = c.Name,
                Description = c.Description

            }).ToList<CategoryViewModel>();

            //Check the results and handle accordingly below
            if (cats.Count == 0)
            {
                return NotFound();
            }
            //Everything is good, return the data
            return Ok(cats);//Cats are being passed in the response back to the requesting app.
        }//end GetCats()

        //GET - api/Categories/id
                //ONE CATEGORY
        public IHttpActionResult GetCat(int id)
        {
            //Create a new REsourceViewModel object and assign it to the appropriate resource from the db
            CategoryViewModel cat = db.Categories.Where(c => c.CategoryID == id).Select(c =>
                new CategoryViewModel()
                {
                    //Coopy the assignments from the GetResources() and paste below

                    CategoryID = c.CategoryID,
                    Name = c.Name,
                    Description = c.Description

                }).FirstOrDefault();
            //scopeless if - once the return executes the scopes are closed.
            if (cat == null)
                return NotFound();

            return Ok(cat);

        }//end GetCat

        //POST - api/Categories (HttpPost)
                //ONE CATEGORY
        public IHttpActionResult PostCat(CategoryViewModel cat)
        {
            //1. Check to validate the object - we need to know that all the data necessary to create a Category is there.
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            Category newCat = new Category()
            {
                CategoryID = cat.CategoryID,
                Name = cat.Name,
                Description = cat.Description
            };

            //add a record and save changes
            db.Categories.Add(newCat);
            db.SaveChanges();

            return Ok(newCat);

        }

        //PUT - api/Categories (HTTPPut)
                //ONE CATEGORY
        public IHttpActionResult PutCat(CategoryViewModel cat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            //We get the category from the db so we can modify it
            Category existingCat = db.Categories.Where(c => c.CategoryID == cat.CategoryID).FirstOrDefault();

            if (existingCat != null)
            {
                existingCat.CategoryID = cat.CategoryID;
                existingCat.Name = cat.Name;
                existingCat.CategoryID = cat.CategoryID;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }

        //DELETE - api/Categories/id (HTTPDelete)
                //ONE CATEGORY
        public IHttpActionResult DeleteCat(int id)
        {
            //Get category from the api to make sure theres a Category with this id
            Category cat = db.Categories.Where(c => c.CategoryID == id).FirstOrDefault();

            if (cat != null)
            {
                db.Categories.Remove(cat);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
