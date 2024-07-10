using System.Collections.Generic;
using System.Linq;
using GreatUma.Models;

namespace GreatUma.Utils
{
    public static class EnumerableUtils
    {
        public static IEnumerable<OddsDatum> GetSameTicketOddsData(IEnumerable<OddsDatum> needs, IEnumerable<OddsDatum> froms, TicketType ticketType)
        {
            IEnumerable<(IEnumerable<int>, OddsDatum)> targetNeedList;
            IEnumerable<(IEnumerable<int>, OddsDatum)> targetFromList;

            if (ticketType == TicketType.Win || ticketType == TicketType.Exacta || ticketType == TicketType.Trifecta)
            {
                targetNeedList = needs.Select(_ => (_.HorseData.Select(x => x.Number), _));
                targetFromList = froms.Select(_ => (_.HorseData.Select(x => x.Number), _));

            }
            else
            {
                targetNeedList = needs.Select(_ => (_.HorseData.Select(x => x.Number).OrderBy(x => x).Select(x => x), _));
                targetFromList = froms.Select(_ => (_.HorseData.Select(x => x.Number).OrderBy(x => x).Select(x => x), _));
            }

            foreach (var need in targetNeedList)
            {
                var sameTicketData = targetFromList.FirstOrDefault(_ => _.Item1.SequenceEqual(need.Item1));
                yield return sameTicketData.Item2;
            }
        }

        public static IEnumerable<IEnumerable<T>> GetContainedList<T>(IEnumerable<T> contained, IEnumerable<IEnumerable<T>> checkTargets)
        {
            foreach(var checkTarget in checkTargets){
                if (CombinationalContain(contained, checkTarget))
                {
                    yield return checkTarget;
                }
            }
        }

        public static bool CombinationalContain<T>(IEnumerable<T> contained , IEnumerable<T> checkTarget)
        {
            foreach (var elem in contained)
            {
                if (!checkTarget.Contains(elem))
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 順列を取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> GetPermutation<T>(IEnumerable<T> items, int k, bool withRepetition = false)
        {
            if (items.Count() == 1)
            {
                yield return new List<T> { items.First() };
                yield break;
            }
            if (k == 1)
            {
                foreach (var item in items)
                    yield return new List<T> { item };
                yield break;
            }
            foreach (var item in items)
            {
                var leftside = new T[] { item };
                var unused = withRepetition ? items : items.Except(leftside);

                foreach (var rightside in GetPermutation(unused, k - 1, withRepetition))
                {
                    yield return leftside.Concat(rightside);
                }
            }
        }

        /// <summary>
        /// 組み合わせを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="k"></param>
        /// <param name="withRepetition"></param>
        /// <returns></returns>
        public static IEnumerable<List<T>> GetCombination<T>(IEnumerable<T> items, int k, bool withRepetition = false)
        {
            if (k == 1)
            {
                foreach (var item in items)
                    yield return new List<T> { item };
                yield break;
            }
            foreach (var item in items)
            {
                var leftside = new T[] { item };

                // item よりも前のものを除く （順列と組み合わせの違い)
                // 重複を許さないので、unusedから item そのものも取り除く
                var unused = withRepetition ? items : items.SkipWhile(e => !e.Equals(item)).Skip(1).ToList();

                foreach (var rightside in GetCombination(unused, k - 1, withRepetition))
                {
                    yield return leftside.Concat(rightside).ToList();
                }
            }
        }
    }
}
