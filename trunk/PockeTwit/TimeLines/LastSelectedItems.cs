using System.Collections.Generic;
using Microsoft.Win32;
using PockeTwit.Library;

namespace PockeTwit
{
    internal static class LastSelectedItems
    {
        private const string StorageRoot = @"\Software\Apps\JustForFun PockeTwit\LastSaved\";

        private static readonly Dictionary<string, string> LastSelectedItemsDictionary =
            new Dictionary<string, string>();

        private static readonly Dictionary<string, status> NewestSelectedItemsDictionary =
            new Dictionary<string, status>();

        private static RegistryKey StoredItemsRoot;

        static LastSelectedItems()
        {
            LoadStoredItems();
        }

        public static void SetLastSelected(string ListName, status selectedStatus)
        {
            if (!LastSelectedItemsDictionary.ContainsKey(ListName))
            {
                LastSelectedItemsDictionary.Add(ListName, "");
            }
            LastSelectedItemsDictionary[ListName] = selectedStatus.id;
            if (!NewestSelectedItemsDictionary.ContainsKey(ListName))
            {
                NewestSelectedItemsDictionary.Add(ListName, selectedStatus);
            }
            else
            {
                if (NewestSelectedItemsDictionary[ListName].createdAt < selectedStatus.createdAt)
                {
                    NewestSelectedItemsDictionary[ListName] = selectedStatus;
                }
            }
            StoreSelectedItem(ListName, selectedStatus.id);
        }

        public static string GetLastSelected(string ListName)
        {
            if (!LastSelectedItemsDictionary.ContainsKey(ListName))
            {
                return null;
            }
            else
            {
                return LastSelectedItemsDictionary[ListName];
            }
        }

        public static string GetNewestSelected(string ListName)
        {
            if (!NewestSelectedItemsDictionary.ContainsKey(ListName))
            {
                return null;
            }
            else
            {
                return NewestSelectedItemsDictionary[ListName].id;
            }
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
                StoredItemsRoot = ParentKey.CreateSubKey("JustForFun PockeTwit\\LastSaved");
            }
            string[] StoredItems = StoredItemsRoot.GetValueNames();
            foreach (string StoredItem in StoredItems)
            {
                LastSelectedItemsDictionary.Add(StoredItem, (string) StoredItemsRoot.GetValue(StoredItem));
            }
        }
    }
}