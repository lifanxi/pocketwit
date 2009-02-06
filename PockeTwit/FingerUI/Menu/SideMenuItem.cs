using System;

using System.Collections.Generic;
using System.Text;

namespace FingerUI
{
    public delegate void delMenuClicked();

    public class SideMenuItem
    {
        private SideMenu ParentMenu;
        public List<SideMenuItem> SubMenuItems = new List<SideMenuItem>();

        public bool HasChildren
        {
            get
            {
                return SubMenuItems.Count > 0;
            }
        }
        private bool _Visible = true;
        public bool Visible
        {
            get
            {
                return _Visible;
            }
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
        public bool CanHide = false;


        public delMenuClicked ClickedMethod;
        private string _TextTemplate;
        public string Text
        {
            get
            {
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
        public SideMenuItem(delMenuClicked Callback, string TextTemplate, SideMenu Parent)
        {
            _TextTemplate = TextTemplate;
            ClickedMethod = Callback;
            ParentMenu = Parent;
        }
    }
}
