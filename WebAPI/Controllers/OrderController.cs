using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class OrderController : ApiController
    {
        private NORTHWNDData db = new NORTHWNDData();
        //private NORTHWNDEntities db = new NORTHWNDEntities();
        // GET api/Orders
        [HttpGet]
        public IEnumerable<Order> GetOrder()
        {
            return db.GetOrder();
        }

        // GET api/Order/5
        public Order GetOrder(Int32 id)
        {
            return db.GetOrder(id);
        }

        //// PUT api/Orders/5
        //public HttpResponseMessage PutOrder(Int32 id, Order order)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }

        //    if (id != order.OrderID)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }

        //    db.Entry(order).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        //// POST api/Orders
        //public HttpResponseMessage PostOrder(Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Orders.Add(order);
        //        db.SaveChanges();

        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, order);
        //        response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = order.OrderID }));
        //        return response;
        //    }
        //    else
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }
        //}

        //// DELETE api/Orders/5
        //public HttpResponseMessage DeleteMOrder(Int32 id)
        //{
        //    Order order = db.Orders.Find(id);
        //    if (order == null)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound);
        //    }

        //    db.Orders.Remove(order);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, order);
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    db.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}
