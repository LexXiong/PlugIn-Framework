using System;
using System.Web;
using System.Web.Mvc;
using Boying.DisplayManagement;
using Boying.DisplayManagement.Shapes;
using Boying.Localization;
using Boying.Mvc.Html;
using Boying.Mvc.Spooling;
using Boying.Security;
using Boying.Security.Permissions;
using Boying.UI.Resources;

namespace Boying.Mvc
{
    public class ViewUserControl<TModel> : System.Web.Mvc.ViewUserControl<TModel>, IBoyingViewPage
    {
        private ScriptRegister _scriptRegister;
        private ResourceRegister _stylesheetRegister;

        private object _display;
        private Localizer _localizer = NullLocalizer.Instance;
        private object _layout;
        private WorkContext _workContext;

        public Localizer T { get { return _localizer; } }

        public dynamic Display { get { return _display; } }

        public dynamic New { get { return ShapeFactory; } }

        public dynamic Layout { get { return _layout; } }

        public WorkContext WorkContext { get { return _workContext; } }

        private IDisplayHelperFactory _displayHelperFactory;

        public IDisplayHelperFactory DisplayHelperFactory
        {
            get
            {
                return _displayHelperFactory ?? (_displayHelperFactory = _workContext.Resolve<IDisplayHelperFactory>());
            }
        }

        private IShapeFactory _shapeFactory;

        public IShapeFactory ShapeFactory
        {
            get
            {
                return _shapeFactory ?? (_shapeFactory = _workContext.Resolve<IShapeFactory>());
            }
        }

        private IAuthorizer _authorizer;

        public IAuthorizer Authorizer
        {
            get
            {
                return _authorizer ?? (_authorizer = _workContext.Resolve<IAuthorizer>());
            }
        }

        public ScriptRegister Script
        {
            get
            {
                return _scriptRegister ??
                    (_scriptRegister = new ViewPage.ViewPageScriptRegister(Writer, Html.ViewDataContainer, Html.GetWorkContext().Resolve<IResourceManager>()));
            }
        }

        public ResourceRegister Style
        {
            get
            {
                return _stylesheetRegister ??
                    (_stylesheetRegister = new ResourceRegister(Html.ViewDataContainer, Html.GetWorkContext().Resolve<IResourceManager>(), "stylesheet"));
            }
        }

        public virtual void RegisterLink(LinkEntry link)
        {
            Html.GetWorkContext().Resolve<IResourceManager>().RegisterLink(link);
        }

        public void SetMeta(string name = null, string content = null, string httpEquiv = null, string charset = null)
        {
            var metaEntry = new MetaEntry(name, content, httpEquiv, charset);
            SetMeta(metaEntry);
        }

        public virtual void SetMeta(MetaEntry meta)
        {
            Html.GetWorkContext().Resolve<IResourceManager>().SetMeta(meta);
        }

        public void AppendMeta(string name, string content, string contentSeparator)
        {
            AppendMeta(new MetaEntry { Name = name, Content = content }, contentSeparator);
        }

        public virtual void AppendMeta(MetaEntry meta, string contentSeparator)
        {
            Html.GetWorkContext().Resolve<IResourceManager>().AppendMeta(meta, contentSeparator);
        }

        public override void RenderView(ViewContext viewContext)
        {
            _workContext = viewContext.GetWorkContext();

            _localizer = LocalizationUtilities.Resolve(viewContext, AppRelativeVirtualPath);
            _display = DisplayHelperFactory.CreateHelper(viewContext, this);
            _layout = _workContext.Layout;

            base.RenderView(viewContext);
        }

        public MvcHtmlString H(string value)
        {
            return MvcHtmlString.Create(Html.Encode(value));
        }

        public bool AuthorizedFor(Permission permission)
        {
            return Authorizer.Authorize(permission);
        }

        public bool HasText(object thing)
        {
            return !string.IsNullOrWhiteSpace(Convert.ToString(thing));
        }

        public BoyingTagBuilder Tag(dynamic shape, string tagName)
        {
            return Html.GetWorkContext().Resolve<ITagBuilderFactory>().Create(shape, tagName);
        }

        public IHtmlString DisplayChildren(dynamic shape)
        {
            var writer = new HtmlStringWriter();
            foreach (var item in shape)
            {
                writer.Write(Display(item));
            }
            return writer;
        }

        public IDisposable Capture(Action<IHtmlString> callback)
        {
            return new ViewPage.CaptureScope(Writer, callback);
        }
    }

    public class ViewUserControl : ViewUserControl<dynamic>
    {
    }
}