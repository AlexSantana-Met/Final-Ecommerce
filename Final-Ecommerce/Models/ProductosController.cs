using Ecommerce_Shop.Repository;
using Final_Ecommerce.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Final_Ecommerce.Models
{
    public class ProductosController : Controller
    {
        public GenericUnitToWork _unitOfWork = new GenericUnitToWork();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        // GET: Productos
        public ActionResult Productos()
        {
            List<Productos> all = _unitOfWork.GetRepositoryInstance<Productos>().GetAllRecords().ToList();

            return View(all);
        }

        public ActionResult AllProductos()
        {
            List<Productos> all = _unitOfWork.GetRepositoryInstance<Productos>().GetAllRecords().ToList();
            return View(all);
        }

        public ActionResult InfoProducto(int? id)
        {
            if (id != null)
            {
                Productos producto = _unitOfWork.GetRepositoryInstance<Productos>().GetFirstOrDefaultByParameter(i => i.id == id);
                return View(producto);
            }
            return RedirectToAction("AllProductos");
        }

    }
}