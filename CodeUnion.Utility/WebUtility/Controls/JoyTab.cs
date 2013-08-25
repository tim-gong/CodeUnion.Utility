using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CodeUnion.Utility.WebUtility.Controls
{
    [ParseChildren(true, "Items")]
    public class JoyTab : WebControl, IPostBackEventHandler, INamingContainer
    {
        string _activeKey;

        #region Property

        [Category("Behavior"),
        Description("项集合"),
        DesignerSerializationVisibility(
            DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public List<JoyTabItem> Items
        {
            get;
            set;
        }

        #endregion

        #region ControlState

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);
            this.EnableViewState = false;
            base.OnInit(e);
        }

        protected override object SaveControlState()
        {
            return _activeKey;
        }

        protected override void LoadControlState(object savedState)
        {
            _activeKey = (string)(savedState ?? Items.First().Key);
        }

        #endregion

        #region Envent
        // public delegate void TabDelegate(object sender, TabEventArgs e);

        public event EventHandler Change;

        protected virtual void OnChange(EventArgs e)
        {
            if (Change != null)
            {
                Change(this, e);
            }
        }

        #endregion

        #region Render

        string renderhead()
        {
            return string.Format("<div class='{0}' id='{1}' name='{1}'>", string.IsNullOrEmpty(CssClass) ? "tab" : CssClass, this.UniqueID, this.UniqueID);
        }
        string renderitem(JoyTabItem item)
        {
            string postscript = string.Format("javascript:__doPostBack('{0}','{1}')", this.UniqueID, item.Key);
            bool ispostback = Change != null && !item.IsCurrent;
            string clientclick = (Change != null && item.IsCurrent) || string.IsNullOrEmpty(item.OnClientClick) ? string.Empty : "onclick=\"" + item.OnClientClick + "\"";
            return string.Format("<a href=\"{0}\" class=\"{1}\" {3} group=\"{4}\">{2}</a>", ispostback ? postscript : "#", item.IsCurrent ? "current" : "normal", item.Text, clientclick, item.Key);
        }

        string renderfoot()
        {
            return "</div>";
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if (Items == null || Items.Count == 0) return;
            InitialItems();
            writer.Write(renderhead());
            foreach (var item in Items)
            {
                writer.Write(renderitem(item));
            }
            writer.Write(renderfoot());
            base.RenderContents(writer);
        }

        #endregion

        #region Utility

        void InitialItems()
        {
            //验证当前项有且仅有一个
            //int currentcount = Items.Count(c => c.IsCurrent == true);
            //if (currentcount > 1) { Items.ForEach(c => c.IsCurrent = false); Items.First().IsCurrent = true; }
            //else if (currentcount != 1) Items.First().IsCurrent = true;
            Items.ForEach(c => c.IsCurrent = c.Key == GetActiveItemKey() ? true : false);
        }

        void SetActiveItem(string key)
        {
            _activeKey = key;
            InitialItems();
        }

        public string GetActiveItemKey()
        {
            if (string.IsNullOrEmpty(_activeKey)) _activeKey = Items.First().Key;
            return _activeKey;
        }

        #endregion

        #region IPostBackEventHandler 成员

        public void RaisePostBackEvent(string eventArgument)
        {
            if (GetActiveItemKey() != eventArgument)
            {
                SetActiveItem(eventArgument);
                OnChange(new EventArgs());
            }
        }

        #endregion
    }

    public class JoyTabItem
    {
        [Category("Behavior"), DefaultValue("值"), Description("项值"), NotifyParentProperty(true)]
        public string Key { get; set; }
        [Category("Behavior"), DefaultValue("标签"), Description("项文本"), NotifyParentProperty(true)]
        public string Text { get; set; }
        public bool IsCurrent { get; set; }
        public string OnClientClick { get; set; }
    }

    public class TabEventArgs : EventArgs
    {
        public string TabKey { get; set; }
    }
}
