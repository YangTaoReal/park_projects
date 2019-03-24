using UnityEngine;
using Newtonsoft.Json;

using System.IO;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace QTFramework
{
   
    public class Drop
    {
        
        public static Dictionary<int, int> GetDrop(int cid)
        {
            CS_DropList.DataEntry csData = DBManager.Instance.m_kDropList.GetEntryPtr(cid);
            int count = 0;
            if(csData._ID < 200000)//单次掉落
                count = 1;
            else
                count = CalcCount(csData._DropCount);

            Dictionary<int, int> listItem = new Dictionary<int, int>();
            for (int i = 0; i < count; i++)
            {
                var list = CalcItem(csData._DropItem);
                foreach(var _item in list)
                {
                    if (listItem.ContainsKey(_item.Key))
                        listItem[_item.Key] += _item.Value;
                    else
                        listItem.Add(_item.Key, _item.Value);
                }
            }
            Debug.Log("____count__" + count);
            foreach (var _item in listItem)
            {
                Debug.Log("__Key__" + _item.Key + "__Value__" + _item.Value);    
            }
            return listItem;
       
        }


        static int GetIndex(string str)
        {
            string[] split = str.Split('|');
            List<string> strCount = new List<string>();
            int allWeight = 0;
            for (int i = 0; i < split.Length; i++)
            {
                string[] splitItem = split[i].Split(';');
                string val = "";
                int weight = int.Parse(splitItem[0]);
                val = allWeight + 1 + "," + (allWeight + weight);
                allWeight += weight;
                strCount.Add(val);
            }

            int idx = 0;
            int rate = UnityEngine.Random.Range(1, allWeight + 1);
            for (int i = 0; i < strCount.Count; i++)
            {
                string[] dicsplit = strCount[i].Split(',');
                if (rate >= int.Parse(dicsplit[0]) && rate < int.Parse(dicsplit[1]))
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }

        static int CalcCount(string str)
        {
            string[] split = str.Split('|');
            int idx = GetIndex(str);
            return int.Parse(split[idx].Split(';')[1]);
        }

        static Dictionary<int, int> CalcItem(string str)
        {
            int idx = GetIndex(str);
            string[] split = str.Split('|');
            Dictionary<int, int> dicItem = new Dictionary<int, int>();
            string[] splitItem = split[idx].Split(';');
            string strItem = splitItem[1];
            if (strItem.Contains("&"))//有多个
            {
                string[] splitmulti = strItem.Split('&');
                for (int i = 0; i < splitmulti.Length; i++)
                {
                    string[] _splitItem = splitmulti[i].Split(' ');
                    int cid = int.Parse(_splitItem[0]);
                    int num = int.Parse(_splitItem[1]);
                    if (dicItem.ContainsKey(cid))
                        dicItem[cid] += num;
                    else
                        dicItem.Add(cid, num);
                }
            }
            else
            {
                string[] _splitItem = strItem.Split(' ');
                int cid = int.Parse(_splitItem[0]);
                int num = int.Parse(_splitItem[1]);
                if (dicItem.ContainsKey(cid))
                    dicItem[cid] += num;
                else
                    dicItem.Add(cid, num);
            }
            return dicItem;
        }

    
    
    }
}
