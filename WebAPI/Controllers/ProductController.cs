using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class ProductsController : ApiController
    {
        private NORTHWNDData db = new NORTHWNDData();

        // GET api/Product
        [HttpGet]
        public IEnumerable<Product> GetProduct()
        {
            return db.GetProduct();
        }

        // GET api/Product/5
        public Product GetProduct(Int32 id)
        {
            return db.GetProduct(id);
        }

        // PUT api/Product/5
        public HttpResponseMessage PutProduct(Int32 id, Product product)
        { 
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != product.ProductID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                db.PutProduct(product);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Product
        public HttpResponseMessage PostProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                db.PostProduct(product);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, product);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = product.ProductID }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Product/5
        public HttpResponseMessage DeleteProduct(Int32 id)
        {
            Product product = db.GetProduct(id);
            if (product == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            try
            {
                db.DeleteProduct(product);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, product);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
