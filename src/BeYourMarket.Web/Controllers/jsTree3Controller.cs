using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BeYourMarket.Model.Models;
using BeYourMarket.Service;
using BeYourMarket.Web.Models;
using jsTree3.Models;
using Newtonsoft.Json;
using Repository.Pattern.UnitOfWork;

namespace jsTree3.Controllers
{
    public class jsTree3Controller : Controller
    {
        #region Fields
        private readonly ICategoryService _categoryService;
        private readonly IListingService _listingService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        #endregion

        #region Constructors
        public jsTree3Controller(
            IUnitOfWorkAsync unitOfWorkAsync,
            ISettingService settingService,
            ICategoryService categoryService,
            IListingService listingService,
            IPictureService pictureService,
            IListingPictureService ListingPictureservice,
            IOrderService orderService,
            ICustomFieldService customFieldService,
            ICustomFieldCategoryService customFieldCategoryService,
            ICustomFieldListingService customFieldListingService,
            ISettingDictionaryService settingDictionaryService,
            IListingStatService listingStatservice,
            IMessageService messageService,
            IMessageThreadService messageThreadService,
            IMessageParticipantService messageParticipantService,
            IMessageReadStateService messageReadStateService,
            DataCacheService dataCacheService,
            SqlDbService sqlDbService)
        {

            _categoryService = categoryService;
            _listingService = listingService;


            _unitOfWorkAsync = unitOfWorkAsync;
        }
        #endregion

        public ActionResult Demo()
        {
            return View();
        }

        public ActionResult AJAXDemo()
        {
            return View();
        }


        public JsonResult GetJsTree3CategData(string id, string ids = null, string idsProDisPlay = null)
        {

            // le js l'appel au chargement des main view qui utilise le jstree dans des sous view
            //if (id == null && ids == null && idsProDisPlay == null)
            //    return Json("", JsonRequestBehavior.AllowGet);       

        List<string> idsSel = new List<string>();
            if (ids != null)
                idsSel = ids.Split(';').ToList();
            else if (id != null)
                idsSel.Add(id);

            List<Category> categories = CacheHelper.Categories;

            // si Pro on n'affiche que les categories du Pro, pour le listing a traiter
            List<string> idsProCategs = null;
            if (! string.IsNullOrEmpty(idsProDisPlay))
            {
                idsProCategs = new List<string>();
                idsProCategs = idsProDisPlay.Split(';').ToList();
            }

            var root = new JsTree3Node() // Create our root node and ensure it is opened
            {
                id = Guid.NewGuid().ToString(),
                text = "[[[Categories by Groups]]]", 
                state = new State(true, false, false)
            };

            // Create a basic structure of nodes
            var children = new List<JsTree3Node>();
            bool bSelected = false;
            bool bOpened = false;
            bool bcheckAllChilds = false;

            foreach (Category par in categories.Where(x => x.Parent == 0).OrderBy(y => y.ID))
            {
                // reinit pour chaque groupr de categ
                bSelected = false;
                bOpened = false;
                bcheckAllChilds = false;

                var node = JsTree3Node.NewNode(par.ID.ToString());

                // si selection d une categ entire dans le panneau de gauche
                if (idsSel.Contains(par.ID.ToString()))
                {
                    bSelected = true;
                    bOpened = true;
                    bcheckAllChilds = true;
                }

                node.state = new State(bOpened, false, bSelected);
                //node.state.opened = true;  ??
                node.text = par.Name;
                //node.icon = 
                foreach (Category child in categories.Where(x => x.Parent == par.ID))
                {
                    var nodeChild = JsTree3Node.NewNode(child.ID.ToString());

                    if (bcheckAllChilds || (idsSel.Contains( child.ID.ToString() ) ) )
                    {
                        bSelected = true;
                        bOpened = true;
                        node.state.opened = true;
                    }
                    
                    nodeChild.state = new State(bOpened, false, bSelected);
                    nodeChild.text = child.Name;

                    // affiche que les categ du Pro disponibles pour le listing
                    // pour les particuliers on affiche toutes les categ
                    if (idsProCategs == null)
                        node.children.Add(nodeChild);
                    else
                        if (idsProCategs.Contains(child.ID.ToString()))
                        node.children.Add(nodeChild);

                    bSelected = false;
                    bOpened = false;
                }

                // affiche que les categ du Pro disponibles pour le listing
                // pour les particuliers on affiche toutes les categ
                // ajoute le node parent s'il a des fils
                if( idsProCategs == null )
                    children.Add(node);
                else
                    if(node.children.Count > 0 ||  idsProCategs.Contains(par.ID.ToString()) )
                        children.Add(node);
            }

            // Add the sturcture to the root nodes children property
            root.children = children;

            // Return the object as JSON
            return Json(root, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJsTree3LocData(string id, string ids = null)
        {
            List<string> idsSel = new List<string>();
            if (ids != null)
                idsSel = ids.Split(';').ToList();
            else if (id != null)
                idsSel.Add(id);

            List<LocationRef> locationsRef = CacheHelper.LocationsRef;

            var root = new JsTree3Node() // Create our root node and ensure it is opened
            {
                id = Guid.NewGuid().ToString(),
                text = "[[[Cities by Zones]]]",  
                state = new State(true, false, false)
            };
            
            // Create a basic structure of nodes
            var children = new List<JsTree3Node>();
            bool bSelected = false;
            bool bOpened = false;
            //for (int i = 0; i < locationsRef.Count; i++)
            foreach (LocationRef par in locationsRef.Where(x => x.Parent ==0).OrderBy(y => y.ID ) )
            {
                var node = JsTree3Node.NewNode(par.ID.ToString());
                bSelected = false;
                node.state = new State(false, false, bSelected);
                node.text = par.Name + " : " + par.Description;
                //node.icon = 
                foreach (LocationRef child in locationsRef.Where(x => x.Parent == par.ID)  )
                {
                    var nodeChild = JsTree3Node.NewNode(child.ID.ToString());
                    if (idsSel.Contains(child.ID.ToString()))
                    {
                        bSelected = true;
                        bOpened = true;
                        node.state.opened = true;
                    }
                    nodeChild.state = new State(bOpened, false, bSelected);
                    nodeChild.text = child.Name;
                    node.children.Add(nodeChild);

                    bSelected = false;
                    bOpened = false;
                }

                children.Add(node);
            }

            // Add the sturcture to the root nodes children property
            root.children = children;
            
            // Return the object as JSON
            return Json(root, JsonRequestBehavior.AllowGet);
        }

        static bool IsPrime(int n)
        {
            if (n > 1)
            {
                return Enumerable.Range(1, n).Where(x => n % x == 0)
                                 .SequenceEqual(new[] { 1, n });
            }

            return false;
        }
    }
}
