﻿using System;

using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace PockeTwit
{
    static class LastSelectedItems
    {
        static LastSelectedItems()
        {
            LoadStoredItems();
        }

        private static Dictionary<string, Library.status> NewestSelectedItemsDictionary = new Dictionary<string, Library.status>();
        private static Dictionary<string, string> LastSelectedItemsDictionary = new Dictionary<string, string>();
        private static RegistryKey StoredItemsRoot;
        public static void SetLastSelected(string ListName, Library.status selectedStatus)
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
            else { return LastSelectedItemsDictionary[ListName]; }
        }
        public static string GetNewestSelected(string ListName)
        {
            if (!NewestSelectedItemsDictionary.ContainsKey(ListName))
            {
                return null;
            }
            else { return NewestSelectedItemsDictionary[ListName].id; }
        }

        private const string StorageRoot = @"\Software\Apps\JustForFun PockeTwit\LastSaved\";

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
                LastSelectedItemsDictionary.Add(StoredItem, (string)StoredItemsRoot.GetValue(StoredItem));
            }
        }
    }
}
