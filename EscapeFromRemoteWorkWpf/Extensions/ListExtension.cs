using System;
using System.Collections.Generic;

namespace EscapeFromRemoteWorkWpf.Extensions
{
    /// <summary>
    /// Listの拡張クラス
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        /// 重複しないように追加する
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="list">リスト</param>
        /// <param name="obj">追加対象</param>
        public static void AddToNotDuplicate<T>(this List<T> list, T obj)
        {
            if (list.Contains(obj))
            {
                return;
            }
            list.Add(obj);
        }

        /// <summary>
        /// 重複を削除する
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="list">リスト</param>
        public static void DropDuplicates<T>(this List<T> list)
        {
            var newList = new List<T>();
            foreach (T item in list)
            {
                newList.AddToNotDuplicate(item);
            }
            list = newList;
        }

        /// <summary>
        /// 要素をランダムに並び替える
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="list">リスト</param>
        /// <returns>並び替えたリスト</returns>
        public static List<T> Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = new Random().Next(0, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
            return list;
        }

        /// <summary>
        /// 指定した要素を取得し、リストから消す
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="list">リスト</param>
        /// <param name="index">インデックス</param>
        /// <returns>要素</returns>
        public static T GetAndRemove<T>(this List<T> list, int index)
        {
            if (list.Count <= index || index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            T target = list[index];
            list.Remove(target);
            return target;
        }

        /// <summary>
        /// 要素を先頭から取り出す
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="list">リスト</param>
        /// <returns>要素</returns>
        public static T Pop<T>(this List<T> list)
        {
            return list.GetAndRemove(list.Count - 1);
        }

        /// <summary>
        /// 要素を末尾から取り出す
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="list">リスト</param>
        /// <returns>要素</returns>
        public static T Dequeue<T>(this List<T> list)
        {
            return list.GetAndRemove(0);
        }

        /// <summary>
        /// 要素をランダムに取得する
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="list">リスト</param>
        /// <returns>要素</returns>
        public static T GetAtRandom<T>(this List<T> list)
        {
            if (list.Count == 0)
            {
                throw new ArgumentException();
            }
            return list[new Random().Next(0, list.Count)];
        }

        /// <summary>
        /// 要素をランダムに取り出す
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="list">リスト</param>
        /// <returns>要素</returns>
        public static T GetAndRemoveAtRandom<T>(this List<T> list)
        {
            T target = list.GetAtRandom();
            list.Remove(target);
            return target;
        }
    }
}
