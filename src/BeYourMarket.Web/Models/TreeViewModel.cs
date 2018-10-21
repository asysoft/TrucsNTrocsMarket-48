using BeYourMarket.Model.Models;
using BeYourMarket.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.WebPages;

namespace BeYourMarket.Web.Models
{
    //public class TreeViewModel
    //{
    //}

    //   https://www.c-sharpcorner.com/article/c-treeview-to-mvc-razor-view/
    public static class TreeViewHelper
    {
        /// <summary>  
        /// Create an HTML tree from a recursive collection of items  
        /// </summary>  
        public static TreeView<T> TreeView<T>(this HtmlHelper html, IEnumerable<T> items)
        {
            return new TreeView<T>(html, items);
        }
    }

    /// <summary>  
    /// Create an HTML tree from a resursive collection of items  
    /// </summary>  
   public class TreeView<T> : IHtmlString
    {
        private readonly HtmlHelper _html;
        private readonly IEnumerable<T> _items = Enumerable.Empty<T>();
        private Func<T, string> _displayProperty = item => item.ToString();
        private Func<T, IEnumerable<T>> _childrenProperty;
        private string _emptyContent = "No children";
        private IDictionary<string, object> _htmlAttributes = new Dictionary<string, object>();
        private IDictionary<string, object> _childHtmlAttributes = new Dictionary<string, object>();
        private Func<T, HelperResult> _itemTemplate;

        public TreeView(HtmlHelper html, IEnumerable<T> items)
        {
            if (html == null) throw new ArgumentNullException("html");
            _html = html;
            _items = items;
            // The ItemTemplate will default to rendering the DisplayProperty  
            _itemTemplate = item => new HelperResult(writer => writer.Write(_displayProperty(item)));
        }

        /// <summary>  
        /// The property which will display the text rendered for each item  
        /// </summary>  
        public TreeView<T> ItemText(Func<T, string> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            _displayProperty = selector;
            return this;
        }


        /// <summary>  
        /// The template used to render each item in the tree view  
        /// </summary>  
        public TreeView<T> ItemTemplate(Func<T, HelperResult> itemTemplate)
        {
            if (itemTemplate == null) throw new ArgumentNullException("itemTemplate");
            _itemTemplate = itemTemplate;
            return this;
        }


        /// <summary>  
        /// The property which returns the children items  
        /// </summary>  
        public TreeView<T> Children(Func<T, IEnumerable<T>> selector)
        {
            //  if (selector == null) //throw new ArgumentNullException("selector");  

            _childrenProperty = selector;

            return this;
        }

        /// <summary>  
        /// Content displayed if the list is empty  
        /// </summary>  
        public TreeView<T> EmptyContent(string emptyContent)
        {
            if (emptyContent == null) throw new ArgumentNullException("emptyContent");
            _emptyContent = emptyContent;
            return this;
        }

        /// <summary>  
        /// HTML attributes appended to the root ul node  
        /// </summary>  
        public TreeView<T> HtmlAttributes(object htmlAttributes)
        {
            HtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            return this;
        }

        /// <summary>  
        /// HTML attributes appended to the root ul node  
        /// </summary>  
        public TreeView<T> HtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null) throw new ArgumentNullException("htmlAttributes");
            _htmlAttributes = htmlAttributes;
            return this;
        }

        /// <summary>  
        /// HTML attributes appended to the children items  
        /// </summary>  
        public TreeView<T> ChildrenHtmlAttributes(object htmlAttributes)
        {
            ChildrenHtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            return this;
        }

        /// <summary>  
        /// HTML attributes appended to the children items  
        /// </summary>  
        public TreeView<T> ChildrenHtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null) throw new ArgumentNullException("htmlAttributes");
            _childHtmlAttributes = htmlAttributes;
            return this;
        }

        public string ToHtmlString()
        {
            return ToString();
        }

        public void Render()
        {
            var writer = _html.ViewContext.Writer;
            using (var textWriter = new HtmlTextWriter(writer))
            {
                textWriter.Write(ToString());
            }
        }

        private void ValidateSettings()
        {
            if (_childrenProperty == null)
            {
                return;
            }
        }


        public override string ToString()
        {
            ValidateSettings();
            var listItems = new List<T>();
            if (_items != null)
            {
                listItems = _items.ToList();
            }

            var ul = new TagBuilder("ul");
            ul.MergeAttributes(_htmlAttributes);

            var li = new TagBuilder("li")
            {
                InnerHtml = _emptyContent
            };
            li.MergeAttribute("id", "-1");

            if (listItems.Count > 0)
            {
                var innerUl = new TagBuilder("ul");
                innerUl.MergeAttributes(_childHtmlAttributes);

                foreach (var item in listItems)
                {                  
                    BuildNestedTag(innerUl, item, _childrenProperty);
                }

                li.InnerHtml += innerUl.ToString();
            }

            ul.InnerHtml += li.ToString();

            return ul.ToString();
        }

        private void AppendChildren(TagBuilder parentTag, T parentItem, Func<T, IEnumerable<T>> childrenProperty)
        {
            //  
            if (childrenProperty == null)
            {
                return;
            }

            // ASY
            var children = childrenProperty(parentItem).ToList();
            //var children = childrenProperty(parentItem).Select (i => new { i.ID, i.Name, i.Description, i.Parent }).Where(p => p.ID == parentItem.ID).ToList();

            Type myTypeParent = parentItem.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myTypeParent.GetProperties());
            foreach (PropertyInfo prop in props)
            {
                if (prop.Name.ToLower() == "id")
                    ParentLocationId = int.Parse(prop.GetValue(parentItem, null).ToString());

            }

            int childID = -1;
            string childName = string.Empty;
            string childDes = string.Empty;
            int childParID = -1;
            
            List<T> itemsRemove = new List<T>();
            
            foreach ( var item in children)
            {
                Type myType = item.GetType();
                IList<PropertyInfo> propsChild = new List<PropertyInfo>(myType.GetProperties());

                foreach (PropertyInfo prop in props)
                {
                    if (prop.Name.ToLower() == "id")
                        childID = int.Parse(prop.GetValue(item, null).ToString());

                    if (prop.Name.ToLower() == "name")
                        childName = prop.GetValue(item, null).ToString();

                    if (prop.Name.ToLower() == "description")
                        childDes = prop.GetValue(item, null).ToString();

                    if (prop.Name.ToLower() == "parent")
                        childParID = int.Parse(prop.GetValue(item, null).ToString());

                }
                // si ok on ajoute
                if (childParID != ParentLocationId)
                {
                    itemsRemove.Add(item);
                }

            }

            // Efface 
            foreach (var item in itemsRemove)
                children.Remove(item);

            if (!children.Any())
            {
                return;
            }

            var innerUl = new TagBuilder("ul");
            innerUl.MergeAttributes(_childHtmlAttributes);

            foreach (var item in children)
            {
                BuildNestedTag(innerUl, item, childrenProperty);
            }

            parentTag.InnerHtml += innerUl.ToString();
        }

        //public  IQueryable GetFilteredData<T>(this IEnumerable<T> source)
        //{
        //    IList<T> returnList = new List<T>();
        //    returnList = source.AsQueryable().Where(  p => p.Parent == ParentLocationId ).ToList();
        //    return returnList.AsQueryable();

        //}
        private void BuildNestedTag(TagBuilder parentTag, T parentItem, Func<T, IEnumerable<T>> childrenProperty)
        {
            var li = GetLi(parentItem);
            parentTag.InnerHtml += li.ToString(TagRenderMode.StartTag);
            AppendChildren(li, parentItem, childrenProperty);
            parentTag.InnerHtml += li.InnerHtml + li.ToString(TagRenderMode.EndTag);
        }

        private TagBuilder GetLi(T item)
        {
            var li = new TagBuilder("li")
            {
                InnerHtml = _itemTemplate(item).ToHtmlString()
            };
            Type myType = item.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            foreach (PropertyInfo prop in props)
            {
                if (prop.Name.ToLower() == "id")
                    li.MergeAttribute("id", prop.GetValue(item, null).ToString());
                // Do something with propValue  
                if (prop.Name.ToLower() == "sortorder")
                    li.MergeAttribute("priority", prop.GetValue(item, null).ToString());
            }
            return li;
        }

        // ASY
        public int ParentLocationId { get; set; }

        //public virtual ICollection<TreeView<T>> ChildLocations { get; set; }
        public IEnumerable<T> GetChildLocations (T item)
        {
            //IEnumerable<T> childs = new List<T>();

            Type myType = item.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            foreach (PropertyInfo prop in props)
            {
                if (prop.Name.ToLower() == "id")
                    ParentLocationId = int.Parse(prop.GetValue(item, null).ToString());

            }

             var l = (List < T > ) CacheHelper.LocationsRef.Select( i => new { i.ID, i.Name, i.Description, i.Parent } ).Where(p => p.Parent == ParentLocationId ) ;
            List<T> childs = l.Select(k => (T)k).ToList();

            //foreach (var child in child2s)
            //{
            //    //T elem =  new T ({ id = child.ID, name = child.Name, description = child.Description, parent = child.Parent });

            //    T elem = new T ();

            //    foreach (PropertyInfo prop in props)
            //    {
            //        if (prop.Name.ToLower() == "id")
            //            prop.SetValue(elem, child.ID) ;

            //        if (prop.Name.ToLower() == "Name")
            //            prop.SetValue(elem, child.Name);

            //        if (prop.Name.ToLower() == "description")
            //            prop.SetValue(elem, child.Description);

            //        if (prop.Name.ToLower() == "parent")
            //            prop.SetValue(elem, child.Parent);
            //    }

            //    childs.Add(elem);
            //}

            return childs;
        }
    }

}