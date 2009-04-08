using System.Collections.Generic;

namespace FingerUI
{
    public delegate void delMenuClicked();

    public class SideMenuItem
    {
        #region Delegates

        public delegate void delItemExpanded(SideMenuItem sender, bool Opened);

        #endregion

        private readonly delMenuClicked ClickedMethod;

        private readonly SideMenu ParentMenu;
        private string _TextTemplate;
        private bool _Visible = true;
        public bool CanHide;
        public List<SideMenuItem> SubMenuItems = new List<SideMenuItem>();

        public SideMenuItem(delMenuClicked Callback, string TextTemplate, SideMenu Parent)
        {
            _TextTemplate = TextTemplate;
            ClickedMethod = Callback;
            ParentMenu = Parent;
        }

        public bool HasChildren
        {
            get { return SubMenuItems.Count > 0; }
        }

        public bool Expanded { get; set; }

        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (value != _Visible)
                {
                    _Visible = value;
                    ParentMenu.IsDirty = true;
                    ParentMenu.SetMenuHeight();
                }
            }
        }

        public string CorrespondingList { get; set; }

        public string Text
        {
            get
            {
                if(!string.IsNullOrEmpty(CorrespondingList))
                {
                    return _TextTemplate + newItemsText(PockeTwit.LastSelectedItems.GetUnreadItems(CorrespondingList));
                }
                return _TextTemplate;
            }
            set
            {
                if (value != _TextTemplate)
                {
                    ParentMenu.IsDirty = true;
                    _TextTemplate = value;

                }
            }
        }

        private static string newItemsText(int count)
        {
            if (count > 0)
            {
                return " (" + count + ")";
            }
            return "";
        }

        public void ClickMe()
        {
            if (SubMenuItems.Count > 0)
            {
                Expanded = !Expanded;
                if (Expanded)
                {
                    MenuExpandedOrCollapsed(this, Expanded);
                }
                else
                {
                    MenuExpandedOrCollapsed(null, Expanded);
                }
            }
            else
            {
                if (ClickedMethod != null)
                {
                    ClickedMethod();
                }
                DoneWithClick();
            }
        }

        public event delMenuClicked DoneWithClick = delegate { };

        public event delItemExpanded MenuExpandedOrCollapsed = delegate { };
    }
}