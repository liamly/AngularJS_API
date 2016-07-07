using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace WebAPI.Models
{
    class NORTHWNDData
    {
        private NORTHWNDEntities db = new NORTHWNDEntities();

        public IEnumerable<Order> GetOrder()
        {
            List<Order> listOrders = new List<Order>();
            foreach (Order item in db.Orders.AsEnumerable())
            {
                //item.Customer = null;
                //item.Employee = null;
                //item.Order_Details = null;
                //item.Shipper = null;

                Order order = new Order();
                order.OrderID = item.OrderID;
                order.CustomerID = item.CustomerID;
                order.EmployeeID = item.EmployeeID;
                order.OrderDate = item.OrderDate;
                order.RequiredDate = item.RequiredDate;
                order.ShippedDate = item.ShippedDate;
                order.ShipVia = item.ShipVia;
                order.Freight = item.Freight;
                order.ShipName = item.ShipName;
                order.ShipAddress = item.ShipAddress;
                order.ShipCity = item.ShipCity;
                order.ShipRegion = item.ShipRegion;
                order.ShipPostalCode = item.ShipPostalCode;
                order.ShipCountry = item.ShipCountry;
                listOrders.Add(item);
            }
            return listOrders;
        }

        public Order GetOrder(Int32 id)
        {
            Order item = db.Orders.Find(id);
            //item.Customer = null;
            //item.Employee = null;
            //item.Order_Details = null;
            //item.Shipper = null;
            Order order = new Order();
            order.OrderID = item.OrderID;
            order.CustomerID = item.CustomerID;
            order.EmployeeID = item.EmployeeID;
            order.OrderDate = item.OrderDate;
            order.RequiredDate = item.RequiredDate;
            order.ShippedDate = item.ShippedDate;
            order.ShipVia = item.ShipVia;
            order.Freight = item.Freight;
            order.ShipName = item.ShipName;
            order.ShipAddress = item.ShipAddress;
            order.ShipCity = item.ShipCity;
            order.ShipRegion = item.ShipRegion;
            order.ShipPostalCode = item.ShipPostalCode;
            order.ShipCountry = item.ShipCountry;
            return item;
        }

        public IEnumerable<Product> GetProduct()
        {
            //return db.Products.AsEnumerable();
            List<Product> listProducts = new List<Product>();
            foreach (Product item in db.Products.AsEnumerable())
            {
                Product _item = new Product();
                _item.ProductID = item.ProductID;
                _item.ProductName = item.ProductName;
                _item.SupplierID = item.SupplierID;
                _item.CategoryID = item.CategoryID;
                _item.QuantityPerUnit = item.QuantityPerUnit;
                _item.UnitPrice = item.UnitPrice;
                _item.UnitsInStock = item.UnitsInStock;
                _item.UnitsOnOrder = item.UnitsOnOrder;
                _item.ReorderLevel = item.ReorderLevel;
                _item.Discontinued = item.Discontinued;
                listProducts.Add(_item);
            }
            return listProducts;
        }

        public Product GetProduct(Int32 id)
        {
            Product item = db.Products.Find(id);
            Product _item = new Product();
            _item.ProductID = item.ProductID;
            _item.ProductName = item.ProductName;
            _item.SupplierID = item.SupplierID;
            _item.CategoryID = item.CategoryID;
            _item.QuantityPerUnit = item.QuantityPerUnit;
            _item.UnitPrice = item.UnitPrice;
            _item.UnitsInStock = item.UnitsInStock;
            _item.UnitsOnOrder = item.UnitsOnOrder;
            _item.ReorderLevel = item.ReorderLevel;
            _item.Discontinued = item.Discontinued;
            return _item;
        }

        public void PutProduct(Product product)
        {
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void PostProduct(Product product)
        {
            db.Products.Add(product);
            db.SaveChanges();
        }

        public void DeleteProduct(Product product)
        {
            //db.Products.SqlQuery("delete dbo.Products where ProductID = @ProductID", new SqlParameter("@ProductID", product.ProductID));
            //db.Products.Remove(product);
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
