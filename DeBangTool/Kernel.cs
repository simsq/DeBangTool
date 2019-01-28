using DeBangTool.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeBangTool
{
    public class Kernel
    {
        private List<int> findNumIndex = new List<int>();// 存储符合条件的数组元素下表  
        private bool findon;// 是否可以从数组中找到相加等于“和”的元素  
        private List<ColipuSouceModel> aData;//目标数据
        private DepponSouceModel sumData;// “和”  
        private List<ColipuSouceModel> okData = new List<ColipuSouceModel>(); //匹配成功的结果存放这里
        public Kernel(List<IGrouping<string, ColipuSouceModel>> a, DepponSouceModel sum)
        {
            aData = a[0].OrderBy(x => x.Quantity).ToList();
            sumData = sum;
            okData = new List<ColipuSouceModel>();
        }
        public List<ColipuSouceModel> Start()
        {
            var list = new List<ColipuSouceModel>();
            list = aData.Where(x => x.Price == sumData.Price && x.IsHeXiao == "0").ToList();
            //for (int i = 0; i < aData.Length; i++)
            //{// 把double数组付给list  
            //    list.Add(aData[i]);
            //}
            var flag = true;
            do
            {
                if (list.Count == 0)
                {
                    return okData;
                }
                int min = list.Min(x => int.Parse(x.Quantity));// 当前最小值  
                int max = list.Max(x => int.Parse(x.Quantity));// 当前最大值  
                if (max == int.Parse(sumData.Quantity))
                {
                    // 找到等于“和”的元素,
                    list[list.Count - 1].IsHeXiao = "1";
                    okData.Add(list[list.Count - 1]);//
                    Console.WriteLine("找到了个一模一样的：" + JsonConvert.SerializeObject(list[list.Count - 1]));
                    return okData;
                }
                if (min + max > int.Parse(sumData.Quantity) && flag)
                {
                    // 删除没用的最大值  
                    list.Remove(list.FirstOrDefault(x => int.Parse(x.Quantity) == max));
                }
                else
                {
                    flag = false;
                }
            } while (flag);
            StartMath(list, int.Parse(sumData.Quantity));
            if (!findon)
            {
                okData.Add(new ColipuSouceModel
                {
                    IsHeXiao = "未匹配到",
                    Price = sumData.Price,
                    ProductCode = sumData.PorductCode,
                    Quantity = sumData.Quantity
                });
                Console.WriteLine("未找到符合条件的数组");
            }
            return okData;
        }
        public int[] Maopao(int[] a)
        {
            for (int i = 0; i < a.Length - 1; i++)
            {
                for (int k = 0; k < a.Length - 1 - i; k++)
                {
                    if (a[k] > a[k + 1])
                    {
                        int b = a[k];
                        a[k] = a[k + 1];
                        a[k + 1] = b;
                    }
                }
            }
            return a;
        }
        public void StartMath(List<ColipuSouceModel> list, int sum)
        {
            for (int i = 0; i <= list.Count() - 2; i++)
            {
                findNumIndex.Clear();
                findNumIndex.Add(list.Count - 1 - i);// 记录第一个元素坐标  
                int indexNum = int.Parse(list[list.Count - 1 - i].Quantity);// 从最大的元素开始，依次往前推  
                Action(list, indexNum, list.Count() - 1 - i, sum);
                //找到了就不用找第二次了
                if (findon)
                {
                    return;
                }
            }
        }
        /** 
        * 递归方法 
        *  
        * @param list 
        *            被查询的数组 
        * @param indexsum 
        *            当前元素相加的和 
        * @param index 
        *            下一个元素位置 
        * @param sum 
        *            要匹配的和 
        */
        public void Action(List<ColipuSouceModel> list, int indexsum, int index, int sum)
        {
            if (index == 0)
                return;
            if (indexsum + int.Parse(list[index - 1].Quantity) > sum)
            {
                // 元素【index-1】太大了，跳到下一个元素继续遍历  
                Action(list, indexsum, index - 1, sum);
            }
            else if (indexsum + int.Parse(list[index - 1].Quantity) < sum)
            {
                // 元素【index-1】可能符合条件，继续往下找  
                findNumIndex.Add(index - 1);// 记录此元素坐标  
                indexsum = indexsum + int.Parse(list[index - 1].Quantity);// 更新元素的和  
                Action(list, indexsum, index - 1, sum);
            }
            else if (indexsum + int.Parse(list[index - 1].Quantity) == sum)
            {
                findNumIndex.Add(index - 1);
                findon = true;// 告诉系统找到了  
                Console.WriteLine("相加等于" + sum + "的数组为：");
                for (int i = 0; i < findNumIndex.Count; i++)
                {
                    list[findNumIndex[i]].IsHeXiao = "1";
                    okData.Add(list[findNumIndex[i]]);
                    //list.Remove(list[findNumIndex[i]]);
                    Console.WriteLine(list[findNumIndex[i]].DOrder);
                }
                return;
            }
            return;
        }
    }
}
