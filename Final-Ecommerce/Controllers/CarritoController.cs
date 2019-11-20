﻿using Final_Ecommerce.Models;
using Final_Ecommerce.Models.DAL;
using Final_Ecommerce.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Final_Ecommerce.Controllers
{
    [Authorize]
    public class CarritoController : Controller
    {
        public GenericUnitToWork _unitOfWork = new GenericUnitToWork();

        // GET: Carrito
        [AllowAnonymous]
        public ActionResult Carrito()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AgregarProducto(int id)
        {
            CarritoModel item = new CarritoModel();
            if (Session["carrito"] == null)
            {
                List<CarritoModel> carrito = new List<CarritoModel>();
                Productos p = _unitOfWork.GetRepositoryInstance<Productos>().GetFirstOrDefaultByParameter(i => i.id == id);
                if (p != null)
                {
                    item.Id_producto = p.id;
                    item.Img = p.img;
                    item.Precio_venta = p.precio_venta;
                    item.Cantidad = 1;
                    item.Nombre = p.nombre;
                }
                carrito.Add(item);
                Session["carrito"] = carrito;
            }
            else
            {
                List<CarritoModel> carrito = (List<CarritoModel>)Session["carrito"];
                int index = ExisteProducto(id);
                if (index != -1)
                {
                    carrito[index].Cantidad++;
                }
                else
                {
                    Productos p = _unitOfWork.GetRepositoryInstance<Productos>().GetFirstOrDefaultByParameter(i => i.id == id);
                    if (p != null)
                    {
                        item.Id_producto = p.id;
                        item.Img = p.img;
                        item.Precio_venta = p.precio_venta;
                        item.Cantidad = 1;
                        item.Nombre = p.nombre;
                    }
                    carrito.Add(item);
                }
                Session["carro"] = carrito;
            }
            return RedirectToAction("AllProductos", "Productos");
        }

        public int ExisteProducto(int id)
        {
            List<CarritoModel> carrito = (List<CarritoModel>)Session["carrito"];
            for (var i = 0; i < carrito.Count(); i++)
            {
                if (carrito[i].Id_producto == id) return i;
            }
            return -1;
        }

        [AllowAnonymous]
        public ActionResult DropProducto(int id)
        {
            List<CarritoModel> carrito = (List<CarritoModel>)Session["carrito"];
            int index = ExisteProducto(id);
            carrito.RemoveAt(index);
            Session["carrito"] = carrito;
            return RedirectToAction("Carrito");
        }

    }
}