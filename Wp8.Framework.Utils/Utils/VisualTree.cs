using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Wp8.Framework.Utils.Utils
{
    public static class VisualTree
    {
        public static DependencyObject GetParent(DependencyObject currentObject)
        {
            ArgumentValidator.AssertNotNull(currentObject, "currentObject");
            var element = currentObject as FrameworkElement;
            if (element != null)
            {
                return element.Parent;
            }
            return null;
        }

        public static TAncestor GetAncestorOrSelf<TAncestor>(this FrameworkElement childElement)
            where TAncestor : class
        {
            ArgumentValidator.AssertNotNull(childElement, "childElement");
            var parent = childElement;
            while (parent != null)
            {
                var result = parent as TAncestor;
                if (result != null)
                {
                    return result;
                }
                parent = parent.Parent as FrameworkElement;
            }
            return null;
        }

        public static IEnumerable GetChildren(DependencyObject current)
        {
            ArgumentValidator.AssertNotNull(current, "current");
            var element = current as FrameworkElement;
            if (element != null)
            {
                return GetVisualChildren(element);
            }
            return new List<DependencyObject>();
        }

        public static IEnumerable<TChild> GetDescendents<TChild>(
            this FrameworkElement parent) where TChild : class
        {
            ArgumentValidator.AssertNotNull(parent, "parent");

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                var candidate = child as TChild;
                if (candidate != null)
                {
                    yield return candidate;
                }

                var element = child as FrameworkElement;
                if (element != null)
                {
                    /* Can be improved with tail recursion. */
                    IEnumerable<TChild> descendents = element.GetDescendents<TChild>();
                    foreach (TChild descendent in descendents)
                    {
                        yield return descendent;
                    }
                }
            }
        }

        static int _nameCounter;

        internal static IEnumerable<FrameworkElement> GetVisualChildren(
            this FrameworkElement parent)
        {
            ArgumentValidator.AssertNotNull(parent, "parent");

            /* This should be rewritten to avoid naming elements. */
            if (string.IsNullOrEmpty(parent.Name))
            {
                parent.Name = "generatedName_" + _nameCounter++;
            }

            string parentName = parent.Name;
            var children = Enumerable.OfType<FrameworkElement>(parent.GetVisualChildren());
            var stack = new Stack<FrameworkElement>(children);

            while (stack.Count > 0)
            {
                var element = stack.Pop();
                if (element.FindName(parentName) == parent)
                {
                    yield return element;
                }
                else
                {
                    foreach (var child in element.GetVisualChildren())
                    {
                        var visualChild = child;
                        if (visualChild != null)
                        {
                            stack.Push(visualChild);
                        }
                    }
                }
            }
        }

        internal static IEnumerable<DependencyObject> GetVisualChildren(
            this DependencyObject parent)
        {
            ArgumentValidator.AssertNotNull(parent, "parent");

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int counter = 0; counter < childCount; counter++)
            {
                yield return VisualTreeHelper.GetChild(parent, counter);
            }
        }

        /// <summary>
        /// Tries the get ancestor of the specified FrameworkElement 
        /// with the specified generic type. Walks the visual tree.
        /// </summary>
        /// <typeparam name="TAncestor">The type of the ancestor.</typeparam>
        /// <param name="frameworkElement">The child framework element.</param>
        /// <param name="ancestor">The resulting ancestor.</param>
        /// <returns>The <c>true</c> if an ancestor was found of the specified type; 
        /// <c>false</c> otherwise.</returns>
        public static bool TryGetAncestorOrSelf<TAncestor>(
            this FrameworkElement frameworkElement, out TAncestor ancestor)
            where TAncestor : class
        {
            ArgumentValidator.AssertNotNull(frameworkElement, "frameworkElement");
            TAncestor result = null;
            GetAncestorOrSelf(frameworkElement, ref result);
            ancestor = result;
            return ancestor != null;
        }

        static void GetAncestorOrSelf<TAncestor>(
            FrameworkElement frameworkElement, ref TAncestor result)
            where TAncestor : class
        {
            if (frameworkElement == null) /* Terminal condition. */
            {
                return; /* Terminal case. */
            }

            var castedElement = frameworkElement as TAncestor;
            if (castedElement != null) /* Terminal condition. */
            {
                result = castedElement; /* Terminal case. */
                return;
            }

            var parent
                = VisualTreeHelper.GetParent(frameworkElement) as FrameworkElement;
            GetAncestorOrSelf(parent, ref result); /* Tail recursive. */
        }

        /// <summary>
        /// Gets the visual parent of the specified element.
        /// </summary>
        /// <param name="childElement">The child element.</param>
        /// <returns>The visual parent</returns>
        public static FrameworkElement GetVisualParent(
            this FrameworkElement childElement)
        {
            return VisualTreeHelper.GetParent(childElement) as FrameworkElement;
        }

        /// <summary>
        /// Gets the ancestors of the element.
        /// </summary>
        /// <param name="descendentElement">The start element.</param>
        /// <returns>The ancestors of the specified element.</returns>
        public static IEnumerable<FrameworkElement> GetVisualAncestors(
            this FrameworkElement descendentElement)
        {
            ArgumentValidator.AssertNotNull(descendentElement, "descendentElement");
            FrameworkElement parent = descendentElement.GetVisualParent();
            while (parent != null)
            {
                yield return parent;
                parent = parent.GetVisualParent();
            }
        }

        /// <summary>
        /// Gets the ancestors of the element.
        /// </summary>
        /// <param name="descendentElement">The start element.</param>
        /// <returns>The ancestors of the specified element.</returns>
        public static IEnumerable<TAncestor> GetVisualAncestors<TAncestor>(
            this FrameworkElement descendentElement) where TAncestor : class
        {
            ArgumentValidator.AssertNotNull(descendentElement, "descendentElement");
            FrameworkElement parent = descendentElement.GetVisualParent();
            while (parent != null)
            {
                var item = parent as TAncestor;
                if (item != null)
                {
                    yield return item;
                }
                parent = parent.GetVisualParent();
            }
        }
    }
}
