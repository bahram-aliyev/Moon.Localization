using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using Moon.Localization;

namespace Moon.Windows.Localization
{
    /// <summary>
    /// The extension which can be used to localize texts in WPF XAML markup.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If you don't want to specify the category every time, you can add a string resource named
    /// <c>ResourceCategory</c> to root object (Window, UserControl, etc.).
    /// </para>
    /// <para>If you then need to specify sub-category, use <b>category</b> beginning with <c>:</c>.</para>
    /// </remarks>
    [MarkupExtensionReturnType(typeof(string))]
    public class ResourceExtension : MarkupExtension
    {
        readonly string category;
        readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceExtension" /> class.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        public ResourceExtension(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceExtension" /> class.
        /// </summary>
        /// <param name="category">The name of the category.</param>
        /// <param name="name">The name of the resource.</param>
        public ResourceExtension(string category, string name)
        {
            Format = "{0}";
            this.category = category;
            this.name = name;
        }

        /// <summary>
        /// Gets or sets a composite format string used to format the result.
        /// </summary>
        public string Format { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return $"{{{string.Format(Format, name)}}}";
            }

            var value = GetValue(GetRootCategory(serviceProvider));

            if (value != null)
            {
                return string.Format(Format, value);
            }

            return null;
        }

        string GetRootCategory(IServiceProvider serviceProvider)
        {
            var rootProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

            if (rootProvider == null)
            {
                return null;
            }

            var rootElement = rootProvider.RootObject as FrameworkElement;

            if (rootElement == null)
            {
                return null;
            }

            return rootElement.FindResource("ResourceCategory") as string;
        }

        object GetValue(string rootCategory)
        {
            if (category != null)
            {
                if (rootCategory != null && category.StartsWith(":", StringComparison.Ordinal))
                {
                    return Resources.Get($"{rootCategory}{category}", name);
                }

                return Resources.Get(category, name);
            }

            if (rootCategory != null)
            {
                return Resources.Get(rootCategory, name);
            }

            return Resources.Get(name);
        }
    }
}