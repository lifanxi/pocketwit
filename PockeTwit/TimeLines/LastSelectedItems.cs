using System.Collections.Generic;
using Microsoft.Win32;
using PockeTwit.Library;

namespace PockeTwit.TimeLines
{
    internal static class LastSelectedItems
    {
        public delegate void delUnreadCountChanged(string TimeLine, int Count);

        public static event delUnreadCountChanged UnreadCountChanged = delegate { };

        private const string StorageRoot = @"\Software\Apps\JustForFun PockeTwit\LastSaved\";

        private static readonly Dictionary<string, string> LastSelectedItemsDictionary =
            new Dictionary<string, string>();

        private static readonly Dictionary<string, status> NewestSelectedItemsDictionary =
            new Dictionary<string, status>();

        private static readonly Dictionary<string, int> UnreadItemCount =
            new Dictionary<string, int>();

        private static RegistryKey StoredItemsRoot;

        static LastSelectedItems()
        {
            LoadStoredItems();
        }

        public static void SetLastSelected(string ListName, status selectedStatus)
        {
            SetLastSelected(ListName,selectedStatus,null);
        }

        public static void SetLastSelected(string ListName, status selectedStatus, SpecialTimeLine specialTime)
        {
            if (!LastSelectedItemsDictionary.ContainsKey(ListName))
            {
                LastSelectedItemsDictionary.Add(ListName, "");
            }
            LastSelectedItemsDictionary[ListName] = selectedStatus.id;
            
            if (!NewestSelectedItemsDictionary.ContainsKey(ListName))
            {
                lock(NewestSelectedItemsDictionary)
                {
                    NewestSelectedItemsDictionary.Add(ListName, selectedStatus);
                }
                SetUnreadCount(ListName, selectedStatus.id, specialTime);
            }
            else
            {
                if (NewestSelectedItemsDictionary[ListName].createdAt <= selectedStatus.createdAt)
                {
                    NewestSelectedItemsDictionary[ListName] = selectedStatus;
                    SetUnreadCount(ListName, selectedStatus.id, specialTime);
                }
            }
            
            StoreSelectedItem(ListName, selectedStatus.id);
        }

        public static int GetUnreadItems(string ListName)
        {
            if(UnreadItemCount.ContainsKey(ListName))
            {
                return UnreadItemCount[ListName];
            }
            return 0;
        }

        public static void UpdateUnreadCounts()
        {
            lock (NewestSelectedItemsDictionary)
            {
                foreach (var ListName in NewestSelectedItemsDictionary.Keys)
                {
                    SetUnreadCount(ListName, NewestSelectedItemsDictionary[ListName].id, null);
                }
            }
        }

        public static void SetUnreadCount(string ListName, string selectedStatus, SpecialTimeLine specialTime)
        {
            TimelineManagement.TimeLineType t = TimelineManagement.TimeLineType.Friends;
            switch (ListName)
            {
                case "Messages_TimeLine":
                    t = TimelineManagement.TimeLineType.Messages;
                    break;
            }

            string Constraints = null;
            if (specialTime != null) 
            {
                Constraints = specialTime.GetConstraints();
            }
            int updatedCount = LocalStorage.DataBaseUtility.CountItemsNewerThan(t, selectedStatus, Constraints);
            if(!UnreadItemCount.ContainsKey(ListName))
            {
                UnreadItemCount.Add(ListName, updatedCount);
            }
            else
            {
                UnreadItemCount[ListName] = updatedCount;
            }
            UnreadCountChanged(ListName, updatedCount);
        }

        public static string GetLastSelected(string ListName)
        {
            if (!LastSelectedItemsDictionary.ContainsKey(ListName))
            {
                return null;
            }
            return LastSelectedItemsDictionary[ListName];
        }

        public static string GetNewestSelected(string ListName)
        {
            if (!NewestSelectedItemsDictionary.ContainsKey(ListName))
            {
                return null;
            }
            return NewestSelectedItemsDictionary[ListName].id;
        }

        private static void StoreSelectedItem(string ListName, string ID)
        {
            StoredItemsRoot.SetValue(ListName, ID, RegistryValueKind.String);
        }


        private static void LoadStoredItems()
        {
            StoredItemsRoot = Registry.CurrentUser.OpenSubKey(StorageRoot, true);
            if (StoredItemsRoot == null)
            {
                RegistryKey ParentKey = Registry.LocalMachine.OpenSubKey(@"\Software\Apps\", true);
                if (ParentKey != null) StoredItemsRoot = ParentKey.CreateSubKey("JustForFun PockeTwit\\LastSaved");
            }
            if (StoredItemsRoot != null)
            {
                string[] StoredItems = StoredItemsRoot.GetValueNames();
                foreach (string StoredItem in StoredItems)
                {
                    LastSelectedItemsDictionary.Add(StoredItem, (string) StoredItemsRoot.GetValue(StoredItem));
                }
            }
        }
    }
}